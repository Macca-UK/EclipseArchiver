using System;
using System.Collections.Generic;
using System.IO;
using DocumentArchiver;

namespace DocumentArchiver.Persistence.LiquidLogic
{
    public class LiquidLogicFolderStorageStrategy
    {
        private readonly string _rootPath;

        public LiquidLogicFolderStorageStrategy(string rootPath)
        {
            _rootPath = rootPath;
        }

        public string GetDocumentPath(DocumentMetadata document)
        {
            // Build an array of path elements, starting with the root folder
            var pathElements = new List<string>() { _rootPath };

            // Add subfolders by parsing the document id
            pathElements.AddRange(GetFoldersFromDocumentId(document.ExtractionDocID));

            // Join path elements together into a full path string
            var documentPath = Path.Combine(pathElements.ToArray());

            return documentPath;
        }

        public string GetFileName(DocumentMetadata document)
        {
            string fileType = document.FileType.Trim('.');

            // Get filename = ID + type
            return $"{document.ExtractionDocID}.{fileType}";
        }

        /// <summary>
        /// Calculate the elements of the folder path from the document id
        /// per the LiquidLogic data load specification
        /// e.g. For document with id 100000 -> store in folder path \\rootFolder\10\00\
        /// and this function would return folder list: { "10", "00" }
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        private IEnumerable<string> GetFoldersFromDocumentId(int fileId)
        {
            // Id < 100000 is an error according to the LL spec
            if (fileId < 100000)
                throw new ArgumentOutOfRangeException("fileId cannot be less than 100000");

            // Convert the document id to a string
            string fileIdString = fileId.ToString();

            // Get the length of the string
            int fileIdLength = fileIdString.Length;

            // Need to consider consecutive pairs of digits in the ID:
            // If length is odd, include pairs up to (but not including) the last digit e.g. 1234567 -> {12}, {34}, {56}
            // If length is even, include all pairs except the final one e.g. 123456 -> {12}, {34}
            // This statement calculates the offset of the final pair
            int maxOffset = fileIdLength % 2 == 0 ? fileIdLength - 2 : fileIdLength - 1;

            // Return each substring pair up to maximum offset
            for (int i = 0; i < maxOffset; i += 2)
            {
                yield return fileIdString.Substring(i, 2);
            }
        }
    }
}