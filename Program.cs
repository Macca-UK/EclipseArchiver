using DocumentArchiver.Persistence.LiquidLogic;  // Added for LiquidLogicFolderStorageStrategy
using DocumentArchiver.Rendering;
using DocumentArchiver.Source;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NPoco;
using System.Runtime;
using System.Collections.Generic;  // Added for HashSet
using System.IO;  // Added for Directory
using System.Text.RegularExpressions;

namespace DocumentArchiver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("====================");
            Console.WriteLine("Assessment Archiver ");
            Console.WriteLine("====================");
            Console.WriteLine();

            var configRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            string rootFolder = configRoot.GetSection("AppSettings")["RootFolder"] ?? "";
            string migrationDbConnectionString = configRoot?.GetConnectionString("Migration") ?? "";


            // Parse command-line arguments to extract DocID
            string? specificDocId = null;
            string? specificPersonId = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-int", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    specificDocId = args[i + 1];
                    Console.WriteLine($"Specific DocID provided: {specificDocId}");
                    break;
                }
                else if (args[i].Equals("-PER", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    specificPersonId = args[i + 1];
                    Console.WriteLine($"Specific PersonID provided: {specificPersonId}");
                    break;
                }
            }

            // Initialize LiquidLogicFolderStorageStrategy like first solution
            var storageStrategy = new LiquidLogicFolderStorageStrategy(rootFolder);
            var archiveSource = new AssessmentArchiveDataSource(configRoot);
            var htmlRenderer = new RazorLightHtmlRenderer();
            var pdfRenderer = new WkHtmlToPdfRenderer();

            using (var migrationContext = new Database(migrationDbConnectionString, DatabaseType.SqlServer2012, SqlClientFactory.Instance))
            {
                // Define base SQL query
                string baseSql = "SELECT DocID, ExtractionDocID, OriginalFileName, FileType AS FileType, FullExtractionPath AS FullExtractionPath, Extracted AS Extracted, FailedToExtract AS FailedToExtract, MovedToLLStructure AS MovedToLLStructure, ExtractionErrorMessage " +
                                 "FROM [dbo].[DocumentSourceEclipseForms] " +
                                 "WHERE Extracted = 'N' " +
                                 "AND FailedToExtract = 'N' " +
                                 "AND ISNULL(Exclude, 'N') = 'N'";

                // Fetch documents based on command-line argument
                List<DocumentMetadata> documents;
                if (!string.IsNullOrEmpty(specificDocId))
                {
                    // Query for specific DocID
                    string specificSql = baseSql + " AND DocID = @0";
                    documents = migrationContext.Fetch<DocumentMetadata>(specificSql, specificDocId);
                    Console.WriteLine($"Archiving Single Document");
                }
                else if (!string.IsNullOrEmpty(specificPersonId))
                {
                    // Query for specific PersonID
                    string personSql = baseSql + " AND PersonID = @0";
                    documents = migrationContext.Fetch<DocumentMetadata>(personSql, specificPersonId);
                    Console.WriteLine($"Fetched {documents.Count} document(s) for PersonID = {specificPersonId}");
                }
                else
                {
                    // Original query for all eligible documents (up to 50,000)
                    string defaultSql = baseSql + " ORDER BY DocID";
                    documents = migrationContext.Fetch<DocumentMetadata>(defaultSql + " OFFSET 0 ROWS FETCH NEXT 50000 ROWS ONLY");
                    Console.WriteLine($"Fetched {documents.Count} document(s) for batch processing");
                }

                // Pre-create all folders 
                CreateAllFolders(documents, storageStrategy, rootFolder, specificPersonId);

                int documentCount = 0;

                foreach (var document in documents)
                {
                    // Determine file path based on mode
                    string filePath;
                    if (!string.IsNullOrEmpty(specificPersonId))
                    {
                        // Use PER<PersonID> folder under rootFolder with sanitized OriginalFileName
                        string personFolder = Path.Combine(rootFolder, $"PER{specificPersonId}");
                        if (string.IsNullOrEmpty(document.OriginalFileName))
                        {
                            throw new InvalidOperationException($"OriginalFileName is null or empty for DocID {document.DocId}");
                        }
                        // Sanitize OriginalFileName: replace spaces and invalid characters with underscores
                        string sanitizedFileName = Regex.Replace(document.OriginalFileName, @"[<>:""/\\|?*\s]+", "_");
                        filePath = Path.Combine(personFolder, sanitizedFileName);
                    }
                    else
                    {
                        // Use LiquidLogicFolderStorageStrategy
                        filePath = Path.Combine(storageStrategy.GetDocumentPath(document),
                                                storageStrategy.GetFileName(document));
                    }

                    Console.WriteLine($"Archiving {document.DocId} to {filePath}");

                    try
                    {
                        // Get assessment record
                        var assessment = archiveSource.GetArchiveData(document.DocId);

                        // Render to HTML
                        var html = htmlRenderer.Render($"ASM.cshtml", assessment);

                        // Convert to PDF
                        var pdf = pdfRenderer.ConvertHtmlToPdf(html, "Eclipse");

                        File.WriteAllBytes(filePath, pdf);

                        document.Extracted = "Y";
                        document.MovedToLLStructure = "Y";
                        document.FailedToExtract = "N";
                        document.ExtractionErrorMessage = null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing assessment {document.DocId}: {ex.Message}");
                        document.Extracted = "N";
                        document.FailedToExtract = "Y";
                        document.MovedToLLStructure = "N";
                        document.ExtractionErrorMessage = ex.Message.Length > 500 ?
                            ex.Message.Substring(0, 500) : ex.Message;  
                    }
                    finally
                    {
                        migrationContext.Update(document);
                        documentCount++;

                        if (documentCount % 1000 == 0)
                        {
                            Console.WriteLine($"Clearing LOH ({documentCount} documents processed)");
                            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                            GC.Collect();
                        }
                    }
                }
            }

            Console.WriteLine("Finished. Press any key to exit.");
            Console.ReadKey();
        }

        // Added for pre-creating folders
        private static void CreateAllFolders(IList<DocumentMetadata> docs, LiquidLogicFolderStorageStrategy storageStrategy, string rootFolder, string? specificPersonId)
        {
            var folderPaths = new HashSet<string>();
            foreach (var doc in docs)
            {
                if (!string.IsNullOrEmpty(specificPersonId))
                {
                    // Use PER<PersonID> folder
                    folderPaths.Add(Path.Combine(rootFolder, $"PER{specificPersonId}"));
                }
                else
                {
                    // Use LiquidLogicFolderStorageStrategy
                    folderPaths.Add(storageStrategy.GetDocumentPath(doc));
                }
            }
            foreach (var folderPath in folderPaths)
            {
                if (!Directory.Exists(folderPath))
                {
                    Console.WriteLine($"Creating folder {folderPath}");
                    Directory.CreateDirectory(folderPath);
                }
            }
            Console.WriteLine($"Prepared {folderPaths.Count} folder(s) for document extraction");
        }
    }
}