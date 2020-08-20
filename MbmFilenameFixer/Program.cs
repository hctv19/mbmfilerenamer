using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MbmFilenameFixer
{
    public static class Program
    {
        private const string FileReportFileName = "FilenameUpdateReport.html";
        private const string DirectoryReportFileName = "DirectoryUpdateReport.html";

        private static readonly IDictionary<string, string> PrioritySubstitionMap = new Dictionary<string, string>
        {
            ["Graphics for use with Novartis email to clients"] = "Graphics",
            ["200228 EY Novartis SimuLyve axSpA Screening Tool Steering Committee Meeting (Esther Yi)"] = "200228 EY NVS SimuLyve axSpA MTG (Yi)",
            ["200228 Contracts and Pricing, Client"] = "200228 Contracts",

            ["200306 DM Merz SimuLyve Virtual Sponsor CRO Kickoff Meeting (Dave Matthews) NOT CONTRACTED"] = "200306 DM Merz SimuLyve CRO Meeting (Matthews) NOT CONTRACTED",
            ["200306 Contracts and Pricing, Client"] = "200306 Contracts",

            ["180108 VP Boehringer Ingelheim 1280.0022 SimuLyve Virtual, Film, On Dem, AV, F2F (K Crossley)"] = "180108 VP BI 1280.0022 SimuLyve, Film, OD, AV, F2F (Crossley)",
            ["180108 Hotel Site Selection"] = "Hotel",
            ["180108 Contracts and Pricing, Client"] = "180108 Contracts",
            ["Hotel Correspondence, RFPs & Availabilty, Frankfurt & U.S. Meetings"] = "Correspondence, RFPs, Frankfurt US Mtg",
            ["Frankfurt Marriott (COPY)"] = "Frankfurt",
            ["Photos and Videos of Frankfurt Marriott Hotel during site visit"] = "Photos Videos",

            ["110115 Contract, Client"] = "110115 Contract",
            ["Contract and Addendum or Amendments with client"] = "Contract",
            ["Addendum G (never signed)-VOID"] = "Addendum G not signed,VOID",
            ["Create Addendum for Nadia \"110115 AD  eLearning QVM149B2301(Amy Lita de los Reyes)\""] = "Create Addendum for Nadia 110115 AD eLearning QVM149B2301",

            ["191025-C M930121002 KG JR Merz SimuLyve Inv Mtg Belotero IOH Pivotal Study (K Gregg J Rangnow)"] = "191025-C M930121002 KG JR Merz SimuLyve Inv Mtg Belotero IOH Pivotal Study (Gregg, Rangnow)",
            ["191025-C Contracts and Pricing, Client"] = "191025-C Contracts",
            ["Contract Templates.00"] = "Templates.00",
            ["MBM Webcast Proposal prior Templates"] = "MBM Templates",
            ["MBM Webcast Proposal Template.28"] = "MBM Template.28",

            ["170405 MS Novartis EMA401A2201 SimuLyve Broadcast Live of  Hybrid F2F Meeting, Addendum B.02 COUNTERSIGNED"] = "COUNTERSIGNED",
            ["Patient Recruitment SimuLyve Virtual Meeting, 161105 Novartis SG LCZ696B2320 eLearning.02 COUNTERSIGNED"] = "COUNTERSIGNED",


            ["140923 Novartis SimuLyve On Demand"] = "140923 Novartis SOD",
            ["092314 Contracts, Addendums and Task Orders with Client"] = "092314 Contract Client",
            ["2014 Task Order 1035 Amendment A.02 to the Initial Task Order"] = "2014 TO 1035 Amendment A.02",
            ["Task Order 1035 Amendment A.02 Internal Development Documents"] = "TO 1035 Amendment A.02 Int Devel",

            ["170606 MQ Novartis CNP520A2202J eLearning (Matt Quinn)"] = "170606 MQ Novartis (Quinn)",
            ["Participant Recruitment, Participant Centric"] = "Recruitment,Participant",
            ["Films Edits UTILIZED for Distribution"] = "Distribution Edits",
            ["File that was provided for publication to YouTube"] = "YouTube",
            ["Do Something Amazing End Alzheimer's Now"] = "Do Something Amazing",
            ["Do Something Amazing, End Alzheimers Now"] = "Do Something Amazing",
            ["Do Something Amazing, End Alzheimer's Now"] = "Do Something Amazing",

            ["Contract.05-10 was not signed by BI, instead these modifictions are incorporated in change order.1.02)"] = "Contract.05-10 not signed by BI", // Talk to Steve

        };

        private static readonly IDictionary<string, string> FileNameSubstitutionMap = new Dictionary<string, string>
        {
            ["  "] = " ",
            [" \\"] = "\\",
            ["including"] = "incl",
            ["responses"] = "resp",
            ["DoubleTree"] = "DBL",
            ["Protocol"] = "PRCTL",
            ["Roundtable"] = "Rndtbl",
            ["Report"] = "Rpt",
            ["Management"] = "MGT",
            ["Meeting"] = "Mtg",
            ["addition"] = "Addtn",
            ["Novartis"] = "NVS",
            ["version "] = "v",
            ["Program"] = "Prgrm",
            ["Backup"] = "BU",
            ["Amendment"] = "Amdmt",
            ["Services"] = "Svcs",
            ["Approved"] = "APPRVD",
            ["Translation"] = "TRANS",
            ["Investigator"] = "PI",
            ["Contract, Client"] = "CONTRACT",
            ["Cover Letter"] = "Cvr",
            ["Agreement"] = "AGRMT",
            ["Material"] = "MTRL",
            ["Training"] = "TRAIN",
            ["Presentation"] = "PRES",
            [" and "] = " & ",
            ["Cover"] = "Cvr",
            ["Webcast"] = "WEBCST",
            ["Questionnaire"] = "QSTNR",
            ["Questionanaires"] = "QSTNRS",
            ["COUNTERSIGNED"] = "SIGNED",
            ["Informtion"] = "Info",
            ["Information"] = "Info",
            ["Orginized"] = "Organized",
            ["Participant"] = "Ptcpnt",
            ["Digital"] = "Dgtl",
            ["Page"] = "Pg",
            ["Procedures"] = "Procs",
            ["Packet"] = "Pkt",
            ["PowerPoint"] = "PPT",
            ["Communication"] = "Comm",
            ["Education"] = "Edu",
            ["Standardized"] = "Std",
            ["Recruitment"] = "RECRUIT",
            ["Portal"] = "Prtl",
            ["Hotel Site Selection"] = "HOTEL",
            ["R & D"] = "RD",
            ["Development"] = "Dev",
            ["SimuLyve on Demand"] = "SoD",
            ["Contracts and Pricing, Client"] = "CONTRACT CLNT",
            ["Financial Procedures, Task order, invoicing, amendments"] = "FP,TO,I,A",
            ["Financial Procedures. Overview, Task Orders and Invoicing"] = "FP,TO,I",
            ["Master Service Agreement"] = "MSA",
            ["Allergy Answers  Live. Eat. Breathe"] = "Allergy",
            ["Documents"] = "Docs",
            ["Department"] = "Dept",
            ["creative treatment"] = "CT",
            ["Contracts and Addenda"] = "CONTRACT",
            ["Contract with Client"] = "CONTRACT CLNT",
            ["Virtual Advisory Board"] = "VAB.",
            ["SimuLyve Advisory Board"] = "VAB",
            ["Correspondence"] = "CORR",
            ["Correspodnence"] = "CORR",
            ["Task Order"] = "TO",
            ["Work Order"] = "WO",
            ["Change Order"] = "CO",
            ["Contracts and Pricing, Client"] = "CONTRACT CLNT",
            ["Contracts, Amendments and Pricing, Client"] = "CONTRACT CLNT",
            ["Contract and Amendments with Client"] = "CONTRACT CLNT",
            ["with"] = "w/"
        };


        // Arg 0: Path
        // Arg 1: Operation
        // Arg 3: Max Length
        // Arg 4: --apply 
        static void Main(string[] args)
        {
            // Config Params
            string path="";
            string operation = "fix-filenames";
            int maxPathLength = 350;
            bool reportOnly = true;

            for (var i = 0; i < args.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        path = args[i];
                        break;
                    case 1:
                        operation = args[i];
                        break;
                    case 2:
                        if (int.TryParse(args[i], out var userSpecifiedMaxPathLength))
                        {
                            maxPathLength = userSpecifiedMaxPathLength;
                        }
                        break;
                    case 3:
                        if (args[i] == "--apply")
                        {
                            reportOnly = false;
                        }
                        break;
                }
            }

            if (operation == "fix-directories")
            {
                if (reportOnly)
                {
                    Console.WriteLine("Running File Renaming utility in fix-directories Read-Only mode");
                    Console.WriteLine("A report of proposed renaming operations will be generated, but no directories will be modified");
                    ProcessDirectories(path, reportOnly, maxPathLength);
                }
                else
                {
                    Console.WriteLine("Running File Renaming utility in fix-directories Apply mode");
                    Console.WriteLine("Directory names will be modified as part of this operation. Type 'yes' to proceed:");
                    var dirProceedPrompt = Console.ReadLine();
                    if (dirProceedPrompt != "yes") return;
                    ProcessDirectories(path, reportOnly, maxPathLength);
                }
            }
            else if (operation == "fix-filenames")
            {
                if (reportOnly)
                {
                    Console.WriteLine("Running File Renaming utility in Read-Only mode");
                    Console.WriteLine("A report of proposed renaming operations will be generated, but no files will be modified");
                    ProcessFileOperations(reportOnly, path, maxPathLength);
                }
                else
                {
                    Console.WriteLine("Running File Renaming utility in Write mode");
                    Console.WriteLine("File names will be modified as part of this operation. Type 'yes' to proceed:");
                    var promptResponse = Console.ReadLine();
                    if (promptResponse != "yes") return;
                    ProcessFileOperations(reportOnly, path, maxPathLength);
                }
            }

            Console.WriteLine("File Renaming operation complete");
            Console.WriteLine("Press Enter key to exit...");
            Console.ReadLine();
        }

        // Directory Fix Operations

        public static void ProcessDirectories(string startingPath, bool reportOnly, int maxPathLength)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var directoryOperations = new List<DirectoryRenameOperationSummary>();

            try
            {
                DirectoryRenameOperationSummary directoryFixResult;
                var reportModeFixedDirectoryCache = new Dictionary<string, string>();
                while ((directoryFixResult = EnumerateAndFixNextDirectory(startingPath, maxPathLength, reportOnly, reportModeFixedDirectoryCache)) != null)
                {
                    directoryOperations.Add(directoryFixResult);
                    reportModeFixedDirectoryCache[directoryFixResult.StartName] = directoryFixResult.EndName;
                }
            }
            finally
            {
                timer.Stop();
                GenerateDirectoryRenameReport(timer.Elapsed, directoryOperations, maxPathLength);
            }
        }

        public static DirectoryRenameOperationSummary EnumerateAndFixNextDirectory(string startingPath, int maxPathLength, bool reportOnly, IDictionary<string,string> reportModeFixedDirectoryCache)
        {
            DirectoryRenameOperationSummary operationSummary = null;
            var allDirectories = Directory.EnumerateDirectories(startingPath, "*.*", SearchOption.AllDirectories);
            foreach (var dir in allDirectories)
            {
                string workingDir = (string)dir.Clone();
                if (reportOnly)
                {
                    foreach (var (key, value) in reportModeFixedDirectoryCache)
                    {
                        workingDir = workingDir.Replace(key, value);
                    }
                }

                // Iterate through directories and try to fix the first one with a name that's too long
                // Returns the first directory that it has a suggested fix for
                var fixedDirectoryName = TryFixDirectoryName(workingDir, maxPathLength);

                if (fixedDirectoryName != workingDir)
                {
                    if (reportOnly == false)
                    {
                        if (Directory.Exists(fixedDirectoryName) == false)
                        {
                            Directory.CreateDirectory(fixedDirectoryName);
                        }
                        
                        Directory.Move(workingDir, fixedDirectoryName);
                    }
                    
                    operationSummary = fixedDirectoryName.Length <= maxPathLength
                        ? new DirectoryRenameOperationSummary(workingDir, fixedDirectoryName)
                        : new DirectoryRenameOperationSummary(workingDir, fixedDirectoryName) { PathTooLong = true };
                    break;
                }
            }
            return operationSummary;
        }


        public static string TryFixDirectoryName(string directoryName, int maxPathLength)
        {
            // Priority Substitutions First
            if (directoryName.Length <= maxPathLength) return directoryName;
            var updatedDirectoryName = (string)directoryName.Clone();
            foreach (var (key, value) in PrioritySubstitionMap)
            {
                updatedDirectoryName = updatedDirectoryName.Replace(key, value, StringComparison.InvariantCultureIgnoreCase);
                if (updatedDirectoryName.Length <= maxPathLength) break;
            }

            // Generic substitutions second
            if (updatedDirectoryName.Length <= maxPathLength) return updatedDirectoryName;
            foreach (var (key, value) in FileNameSubstitutionMap)
            {
                updatedDirectoryName = updatedDirectoryName.Replace(key, value, StringComparison.InvariantCultureIgnoreCase);
                if (updatedDirectoryName.Length <= maxPathLength) break;
            }

            return updatedDirectoryName;
        }

        public static void GenerateDirectoryRenameReport(TimeSpan timeToRun, IList<DirectoryRenameOperationSummary> operations, int maxPathLength)
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
                "<body><h1>Directory Rename Report</h1><br/>",
                $"<p><b>Execution Time: {timeToRun}</p>",
                $"<p><b>Max Configured Path Length: {maxPathLength}</p>",
                $"<p><b>Found {operations.Count} directories needing to be renamed</p>",
                $"<p><b>Found {operations.Count(o => o.PathTooLong)} directories with paths longer than {maxPathLength} characters </p>",
                $"<p><b>Found {operations.Count(o => o.EndName.Length > 360)} directories with directory names longer than 360 characters </p>",
                $"<p><b>Found {operations.Count(o => o.EndName.Length > 370)} directories with directory names longer than 370 characters </p>",
                $"<p><b>Found {operations.Count(o => o.EndName.Length > 380)} directories with directory names longer than 380 characters </p>",
                $"<p><b>Found {operations.Count(o => o.EndName.Length > 390)} directories with directory names longer than 390 characters </p>",
                $"<p><b>Found {operations.Count(o => o.EndName.Length > 400)} directories with directory names longer than 400 characters </p>",
                $"<p><b>Found {operations.Count(o => o.PathTooLong)} directories with names that could not be automatically shortened to fit</p>"
            };


            // Path Length Too Long
            var pathTooLongDirectories = operations.Where(o => o.PathTooLong).ToList();
            reportLines.Add($"<h1>Path Too Long ({pathTooLongDirectories.Count})</h1><br><table>");
            reportLines.Add("<tr><th>Current Directory</th><th>Current Length</th><th>Shortened Directory</th><th>Shortened Length</th></tr>");
            reportLines.AddRange(pathTooLongDirectories.Select(operation => $"<tr><td>{operation.StartName}</td><td>{operation.StartName.Length}</td><td>{operation.EndName}</td><td>{operation.EndName.Length}</td></tr>"));
            reportLines.Add("</table>");

            // Fixable Directories
            var fixableDirectories = operations.Where(o => !o.PathTooLong).ToList();
            reportLines.Add($"<h1>Fixable Directories ({fixableDirectories.Count})</h1><br><table>");
            reportLines.Add("<tr><th>Current Directory</th><th>Current Length</th><th>New Directory</th><th>New Length</th></tr>");
            reportLines.AddRange(fixableDirectories.Select(operation => $"<tr><td>{operation.StartName}</td><td>{operation.StartName.Length}</td><td>{operation.EndName}</td><td>{operation.EndName.Length}</td></tr>"));
            reportLines.Add("</table>");

            reportLines.Add("</body></html>");

            Console.WriteLine($"Writing report to {DirectoryReportFileName}");
            File.WriteAllLines($"{DirectoryReportFileName}", reportLines);
        }


        // Filename Fix Operations
        public static void ProcessFileOperations(bool reportOnly, string dir, int maxPathLength)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var operations = new List<FileRenameOperationSummary>();

            try
            {
                var allFiles = Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories);
                foreach (var file in allFiles)
                {
                    var fileInfo = new FileInfo(file);

                    try
                    {

                        var newFilePath = GetFixedPath(maxPathLength, fileInfo, FileNameSubstitutionMap);
                        if (newFilePath == null) continue;

                        // Add transformation to report history
                        operations.Add(new FileRenameOperationSummary(file, newFilePath, fileInfo.DirectoryName.Length));

                        // Apply the file rename operation if reportOnly == false
                        if (reportOnly == false)
                        {
                            try
                            {
                                Console.WriteLine($"Renaming {file} to {newFilePath}");
                                Directory.Move(file, newFilePath);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"[Error] Unable to rename file {file} to {newFilePath}");
                                Console.WriteLine($"[Error] {e.Message}");
                            }
                        }
                    }
                    catch (PathTooLongException)
                    {
                        operations.Add(new FileRenameOperationSummary(file, string.Empty, fileInfo.DirectoryName.Length) { PathTooLong = true });
                    }
                    catch (FilenameShorteningNotEnoughException)
                    {
                        operations.Add(new FileRenameOperationSummary(file, string.Empty, fileInfo.DirectoryName.Length) { ShortenedFilenameTooLong = true });
                    }
                }

            }
            finally
            {
                timer.Stop();
                GenerateFileRenameReport(reportOnly, timer.Elapsed, operations, maxPathLength);
            }

        }

        public static string GetFixedPath(int maxPathLength, FileInfo fileInfo, IDictionary<string, string> map)
        {
            var directory = fileInfo.DirectoryName;
            if (fileInfo.DirectoryName.Length > 350)
            {
                directory = FixDirectoryName(fileInfo.DirectoryName, map);
            }

            var fullPath = Path.Combine(directory, fileInfo.Name);
            if (fullPath.Length < maxPathLength) return fullPath;

            // If full path is under max length, do nothing
            if (fileInfo.FullName.Length <= maxPathLength) return null;

            // If directory path itself is too long (not including the filename), this is going to require manual manipulation
            // Add 6 characters of buffer since a 1 character filename plus a file extensions plus a path separator would be at least another 6 chars
            var charsAvailableForFilename = maxPathLength - fileInfo.DirectoryName.Length - 1;
            if (charsAvailableForFilename < 5) throw new PathTooLongException();

            // Shorten the filename and check if this is enough to meet the length requirements
            var fixedFileName = new string(fileInfo.Name);

            // Make multiple passes over the filename trying to shorten
            // Reducing the allowed contiguous string length in the filename each pass until we reach the limit of 3 characters
            // If we go lower than 4 chars, we could start truncating extensions
            var findTextBlocksLongerThan = 4;
            while (true)
            {
                var startingFileNameLength = fixedFileName.Length;
                var pattern = $"([^.\\d]){{{findTextBlocksLongerThan},}}";
                var match = Regex.Match(fixedFileName, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    fixedFileName = fixedFileName.Replace(match.Value, match.Value.Substring(0, match.Value.Length - 1));
                }

                // Did this operation get the filename short enough?
                if (fixedFileName.Length <= charsAvailableForFilename) break;

                // If not, did this operation result in any change?
                if (fixedFileName.Length == startingFileNameLength)
                {
                    // If not, then we're out of room to shorten the filename
                    break;
                }
            }

            var newFullPath = Path.Combine(fileInfo.DirectoryName, fixedFileName);
            if (newFullPath.Length > maxPathLength) throw new FilenameShorteningNotEnoughException();

            // If the filename shortening allowed us to get inside the bounds, then return the updated file path
            return newFullPath;
        }

        private static string FixDirectoryName(string directoryName, IDictionary<string, string> map)
        {
            foreach (var (key, value) in map)
            {
                directoryName = directoryName.Replace(key, value);
            }

            return directoryName;
        }

        public static void GenerateFileRenameReport(bool reportOnly, TimeSpan timeToRun, IList<FileRenameOperationSummary> operations, int maxPathLength)
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
                $"<p><b>{(reportOnly ? "Mode: ReadOnly (no files were updated during this run)" : "Mode: Write (file renaming operations were applied during this run" + ")")}</p>",
                $"<p><b>Execution Time: {timeToRun}</p>",
                $"<p><b>Max Configured Path Length: {maxPathLength}</p>",
                $"<p><b>Found {operations.Count} files needing to be renamed</p>",
                $"<p><b>Found {operations.Count(o => o.PathTooLong)} files with directory names longer than {maxPathLength} characters </p>",
                $"<p><b>Found {operations.Count(o => o.PathLength > 360)} files with directory names longer than 360 characters </p>",
                $"<p><b>Found {operations.Count(o => o.PathLength > 370)} files with directory names longer than 370 characters </p>",
                $"<p><b>Found {operations.Count(o => o.PathLength > 380)} files with directory names longer than 380 characters </p>",
                $"<p><b>Found {operations.Count(o => o.PathLength > 390)} files with directory names longer than 390 characters </p>",
                $"<p><b>Found {operations.Count(o => o.PathLength > 400)} files with directory names longer than 400 characters </p>",
                $"<p><b>Found {operations.Count(o => o.ShortenedFilenameTooLong)} files with names that could not be automatically shortened to fit</p>"
            };


            // Path Length Too Long
            var pathTooLongFiles = operations.Where(o => o.PathTooLong).ToList();
            reportLines.Add($"<h1>Path Too Long ({pathTooLongFiles.Count})</h1><br><table>");
            reportLines.Add("<tr><th>Current Filename</th><th>Current Length</th></tr>");
            reportLines.AddRange(pathTooLongFiles.Select(operation => $"<tr><td>{operation.CurrentFileName}</td><td>{operation.CurrentFileName.Length}</td></tr>"));
            reportLines.Add("</table>");

            // Unfixable Files
            var unfixableFiles = operations.Where(o => o.ShortenedFilenameTooLong).ToList();
            reportLines.Add($"<h1>Filename Unfixable ({unfixableFiles.Count})</h1><br><table>");
            reportLines.Add("<tr><th>Current Filename</th><th>Current Length</th><th>Shortened Filename</th><th>Shortened Length</th></tr>");
            reportLines.AddRange(unfixableFiles.Select(operation => $"<tr><td>{operation.CurrentFileName}</td><td>{operation.CurrentFileName.Length}</td><td>{operation.NewFileName}</td><td>{operation.NewFileName.Length}</td></tr>"));
            reportLines.Add("</table>");

            // Fixable Files
            var fixableFiles = operations.Where(o => !o.PathTooLong && !o.ShortenedFilenameTooLong).ToList();
            reportLines.Add($"<h1>Fixable Files ({fixableFiles.Count})</h1><br><table>");
            reportLines.Add("<tr><th>Current Filename</th><th>Current Length</th><th>New Filename</th><th>New Length</th></tr>");
            reportLines.AddRange(fixableFiles.Select(operation => $"<tr><td>{operation.CurrentFileName}</td><td>{operation.CurrentFileName.Length}</td><td>{operation.NewFileName}</td><td>{operation.NewFileName.Length}</td></tr>"));
            reportLines.Add("</table>");

            reportLines.Add("</body></html>");

            Console.WriteLine($"Writing report to {FileReportFileName}");
            File.WriteAllLines($"{FileReportFileName}", reportLines);
        }
    }

    public class PathTooLongException : Exception
    {
    }

    public class FilenameShorteningNotEnoughException : Exception
    {
    }

    public class DirectoryRenameOperationSummary
    {
        public DirectoryRenameOperationSummary(string startName, string endName)
        {
            StartName = startName;
            EndName = endName;
        }

        public string StartName { get; set; }
        public string EndName { get; set; }
        public bool PathTooLong { get; set; }
    }

    public class FileRenameOperationSummary
    {
        public FileRenameOperationSummary(string cName, string nName, int pathLength)
        {
            CurrentFileName = cName;
            NewFileName = nName;
            PathLength = pathLength;
        }

        public int PathLength { get; set; }
        public bool PathTooLong { get; set; }
        public bool ShortenedFilenameTooLong { get; set; }
        public string CurrentFileName { get; set; }
        public string NewFileName { get; set; }
    }
}
