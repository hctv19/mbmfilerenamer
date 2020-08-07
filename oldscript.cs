using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MbmFilenameFixer
{
    public static class Program
    {
        private const int MaxFileNameLength = 30;
        private const string ReportFileName = "FilenameUpdateReport.html";

        static void Main(string[] args)
        {
            var reportOnly = true;
            var path = args[0];
            if (args.Length > 1)
            {
                if (args[1] == "--apply")
                {
                    reportOnly = false;
                } 
            }

            if (reportOnly == true)
            {
                Console.WriteLine("Running File Renaming utility in Read-Only mode");
                Console.WriteLine("A report of proposed renaming operations will be generated, but no files will be modified");
            }
            else
            {
                Console.WriteLine("Running File Renaming utility in Write mode");
                Console.WriteLine("File names will be modified as part of this operation. Type 'yes' to proceed:");
                var promptResponse = Console.ReadLine();
                if (promptResponse != "yes") return;
            }

            var operations = new List<FileRenameOperationSummary>();
            var allFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            foreach (var file in allFiles)
            {
                var info = new FileInfo(file);
                var fileName = info.Name;
                var fixedFileName = GetFixedFilename(fileName);
                if (fixedFileName == null) continue;

                var existingFileFullPath = Path.Combine(info.DirectoryName, info.Name);
                var fixedFileFullPath = Path.Combine(info.DirectoryName, fixedFileName);

                // Add transformation to report history
                operations.Add(new FileRenameOperationSummary(existingFileFullPath, fixedFileFullPath));

                // Apply the file rename operation if reportOnly == false
                if (reportOnly == false)
                {
                    try
                    {
                        Console.WriteLine($"Renaming {existingFileFullPath} to {fixedFileFullPath}");
                        File.Move(existingFileFullPath, fixedFileFullPath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[Error] Unable to rename file {existingFileFullPath} to {fixedFileFullPath}");
                        Console.WriteLine($"[Error] {e.Message}");
                    }
                }
            }

            GenerateReport(operations);
            Console.WriteLine("File Renaming operation complete");
            Console.WriteLine("Press Enter key to exit...");
            Console.ReadLine();
        }

        public static string GetFixedFilename(string fileName)
        {
            if (fileName.Length <= MaxFileNameLength) return null;

            var fixedFileName = new string(fileName);
            var pattern = $"([^.\\d]){{{MaxFileNameLength},}}";
            var match = Regex.Match(fileName, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                fixedFileName = fixedFileName.Replace(match.Value, match.Value.Substring(0, 8));
            }

            return fixedFileName;
        }

        public static void GenerateReport(IList<FileRenameOperationSummary> operations)
        {
            var reportLines = new List<string>
            {
                "<!DOCTYPE html>",
                "<html>",
                "<head>",
                "<style>",
                "table {",
                "font-family: arial, sans-serif;",
                "border-collapse: collapse;",
                "width: 100%;",
                "}",
                "td, th {",
                "border: 1px solid #dddddd;",
                "text-align: left;",
                "padding: 8px;",
                "}",
                "tr:nth-child(even)",
                "{",
                    "background-color: #dddddd;",
                "}",
                "</style>",
                "</head>",
                "<body><h1>File Rename Report</h1><br/>",
                $"<p><b>Found {operations.Count} files to rename</p>",
                "<table>",
                "<tr><th>Current Filename</th><th>New Filename</th></tr>"
            };
            reportLines.AddRange(operations.Select(operation => $"<tr><td>{operation.CurrentFileName}</td><td>{operation.NewFileName}</td></tr>"));

            reportLines.Add($"</table></body></html>");

            Console.WriteLine($"Writing report to {ReportFileName}");
            File.WriteAllLines($"{ReportFileName}", reportLines);
        }

        public class FileRenameOperationSummary
        {
            public FileRenameOperationSummary(string cName, string nName)
            {
                CurrentFileName = cName;
                NewFileName = nName;
            }
            public string CurrentFileName { get; set; }
            public string NewFileName { get; set; }
        }
    }
}
