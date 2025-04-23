using NPoco;

namespace DocumentArchiver
{
    [TableName("dbo.DocumentSourceEclipseForms")]  // Specify the actual table name
    [PrimaryKey("ExtractionDocID")]  // Applied at class level
    public class DocumentMetadata
    {
        // The documents.ID value 
        public string DocId { get; set; } = string.Empty;

        // The Original File Name
        public string OriginalFileName { get; set; } = string.Empty;

        // The target LL output folder
        [Column(Name = "ExtractionDocID")]  // Maps DocId property to ExtractionDocID column
        public int ExtractionDocID { get; set; }

        // The file suffix e.g. "pdf"
        public string FileType { get; set; } = string.Empty;

        // Extracted flag
        public string? Extracted { get; set; }

        // MovedToLLStructure flag
        public string? MovedToLLStructure { get; set; }

        // Extracted flag
        public string? FailedToExtract { get; set; }


        // Extracted flag
        public string? FullExtractionPath { get; set; }

        // Extracted flag
        public string? ExtractionErrorMessage { get; set; }
    }
}
