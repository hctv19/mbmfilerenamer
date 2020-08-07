using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MbmFilenameFixer
{
    public static class Program
    {
        private const string ReportFileName = "FilenameUpdateReport.html";

        private static readonly IDictionary<string, string> PrioritySubstitionMap = new Dictionary<string, string>
        {
            ["200228 EY Novartis SimuLyve axSpA Screening Tool Steering Committee Meeting (Esther Yi)"] = string.Empty,
            ["200306 DM Merz SimuLyve Virtual Sponsor CRO Kickoff Meeting (Dave Matthews) NOT CONTRACTED"] = string.Empty,
            ["200306 MC Paris - Chicago, PRG Meetings - Events (Tango, Carson) NOT CONTRACTED"] =string.Empty,
            ["191007 MM US Medical IHD Secukinumab Protocol Roundtable (Gomez, McMorrow, Tandon)"] =string.Empty,
            ["191025-C M930121002 KG JR Merz SimuLyve Inv Mtg Belotero IOH Pivotal Study (K Gregg J Rangnow)"]=string.Empty,
            ["191126 RM Novartis CINC424H212201 SimuLyve Virtual IM (Ross Merryweather)"]=string.Empty,
            ["190826 WS Boehringer Ingelheim SimuLyve Filming Graphic Arts (Scherres) NOT CONTRACTED"]=string.Empty,
            ["190722 WS Novartis SimuLyve MS Musical & Physical Therapy, Film (Wendy Su Mary Sheridan)"]=string.Empty,
            ["190607 Ashfield Novartis AV, Generation Program Update AAIC (Eastburn, Duff) - CANCELLED"]=string.Empty,
            ["190312 SB Novartis LTW888A12401 SimuLyve® Film for Sim Virtual Clinical Trial Train (Burmaster)"]=string.Empty,
            ["151101 AD Novartis eLearning QVM149B2301 (Askin)"]=string.Empty,

            ["140923 Novartis SimuLyve On Demand"] = "140923 Novartis SOD",
            ["092314 Contracts, Addendums and Task Orders with Client"]= "092314 Contract Client",
            ["2014 Task Order 1035 Amendment A.02 to the Initial Task Order"]= "2014 TO 1035 Amendment A.02",
            ["Task Order 1035 Amendment A.02 Internal Development Documents"] = "TO 1035 Amendment A.02 Int Devel",

            ["170606 MQ Novartis CNP520A2202J eLearning (Matt Quinn)"]= "170606 MQ Novartis (Quinn)",
            ["Participant Recruitment, Participant Centric"]= "Recruitment,Participant",
            ["Films Edits UTILIZED for Distribution"]= "Distribution Edits",
            ["File that was provided for publication to YouTube"]="YouTube",
            ["Do Something Amazing End Alzheimer's Now"]="Do Something Amazing",
            ["Do Something Amazing, End Alzheimers Now"] = "Do Something Amazing",
            ["Do Something Amazing, End Alzheimer's Now"] = "Do Something Amazing",
           
            ["170606 MQ Novartis CNP520A2202J eLearning (Matt Quinn)\\170606 Edit\\Participant Recruitment, Participant Centric\\Do Something Amazing\\Edit Participant Recruitment Film BACKUP OF HARDRIVE\\Exports Orginized by Steve\\Films Edits UTILIZED for Distribution\\Do Something Amazing, End Alzheimer's Now (full film)\\Do Something Amazing, End Alzheimers Now.19\\File that was provided for publication to YouTube"] = "170606 MQ Novartis (Quinn)\\170606 Edit\\Recruitment,Participant\\Do Something Amazing\\Edit Participant BU OF HD\\Export Steve\\Distribution Edits \\Do Something Amazing,(full film)\\Do Something Amazing.19\\YouTube",
            ["180108 VP Boehringer Ingelheim 1280.0022 SimuLyve Virtual, Film, On Dem, AV, F2F (K Crossley)\\180108 Hotel Site Selection\\Hotel Correspondence, RFPs & Availabilty, Frankfurt & U.S. Meetings\\Correspondence, Hotels (including RFP responses)\\Marriott\\Frankfurt Marriott (COPY)\\Photos of Event Space\\Photos and Videos of Frankfurt Marriott Hotel during site visit\\Frankfurt Marriott Selects\\Champions Bar Photos"] = @"180108 VP BI 1280.0022 SimuLyve, Film, OD, AV, F2F (Crossley)\\180108 Hotel \\Correspondence, RFPs, Frankfurt US Mtg\\Corr, Hotels \\Marriott\\Frankfurt\\Photos\\Photos Videos\\Frankfurt Marriott\\Champions Bar",
            ["140923 Novartis SimuLyve On Demand\\092314 Contracts, Addendums and Task Orders with Client\\2014 Task Order 1035 Amendment A.02 to the Initial Task Order\\Task Order 1035 Amendment A.02 Internal Development Documents\\Main Contracts Novartis\\General Services Agreement & Afilliate Agreement between Novartis and MBM\\Copy of all agreements (clarified in folders)\\Novartis Agreements\\Agreement through Rita and Marie\\Signed Amendments to Agreement through Rita & Marie"] = @"140923 Novartis SOD\\092314 Contract Client\\2014 TO 1035 Amendment A.02\\TO 1035 Amendment A.02 Int Devel\\Main Contract Novartis\\GSA & Afilliate Agmt \\Copy agreements\\Novartis Agreements\\Rita Marie\\Signed",

            ["170305 AP Novartis eLearning AIN457F2366 (Ana Prado)\\170305 Reference Materials-prior project 110515\\110515 Contract, Client\\Contract Templates\\Novartis\\Novartis SimuLyve Virtual Meeting Contract\\MBM Webcast Proposal Template.24-38\\MBM Webcast Proposal Template.28\\Novartis Internal Email to Clients re SimuLyve\\Graphics for use with Novartis email to clients"] = string.Empty,
            ["170305 AP Novartis eLearning AIN457F2366 (Ana Prado)\\170305 Reference Materials-prior project 110515\\110515 Contract, Client\\Contract Templates\\Novartis\\Novartis SimuLyve Virtual Meeting Contract\\MBM Webcast Proposal Template.24-38\\MBM Webcast Proposal Template.27\\Novartis Internal Email to Clients re SimuLyve\\Graphics for use with Novartis email to clients"] = string.Empty,
            ["170305 AP Novartis eLearning AIN457F2366 (Ana Prado)\\170305 Reference Materials-prior project 110515\\110515 Contract, Client\\Contract Templates\\Novartis\\Novartis SimuLyve Virtual Meeting Contract\\MBM Webcast Proposal Template.24-38\\MBM Webcast Proposal Template.26\\Novartis Internal Email to Clients re SimuLyve\\Graphics for use with Novartis email to clients"] = string.Empty,
            ["170305 AP Novartis eLearning AIN457F2366 (Ana Prado)\\170305 Reference Materials-prior project 110515\\110515 Contract, Client\\Contract Templates\\Novartis\\Novartis SimuLyve Virtual Meeting Contract\\MBM Webcast Proposal Template.24-38\\MBM Webcast Proposal Template.25\\Novartis Internal Email to Clients re SimuLyve\\Graphics for use with Novartis email to clients"] = string.Empty,



            ["Patient Recruitment SimuLyve Virtual Meeting, 161105 Novartis SG LCZ696B2320 eLearning.02 COUNTERSIGNED"] = "COUNTERSIGNED",
            [" as that's the full guiding document for this project"] = string.Empty,
            ["Contract Versions Sent to MBM from Client with Client Cover Pages "] = "CTRCT Versions Sent to MBM from client",
            [" to create the final signed by both parties"] = string.Empty,
            [" (initially between hotel and Boehringer, then between hotel and MBM)"] = string.Empty,
            [", including cloud computing certification questions"] = string.Empty,
            ["170405 MS Novartis EMA401A2201 SimuLyve Broadcast Live of  Hybrid F2F Meeting, Addendum B.02 COUNTERSIGNED"] = "COUNTERSIGNED",
            ["Metric Tools, Case Studies, Research for additions to Contract"] = "Metric Tools, Case Studies, Research",
            ["Latest from Daniel - confusing"] = "Daniel",
            ["Change Order.1.02C to Contract.04 180108 VP Boehringer Ingelheim 1280.0022.02"] = string.Empty, // Talk to Steve
            ["Contract.05-10 was not signed by BI, instead these modifictions are incorporated in change order.1.02)"] = string.Empty, // Talk to Steve
            ["Contract.05 (will not be signed by BI, instead these modifictions will be incorporated in change order.1.02)"] = string.Empty, // Talk to Steve
            [" during site visit"] = string.Empty,

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

        static void Main(string[] args)
        {
            var maxPathLength = 350;

            var reportOnly = true;
            var path = args[0];
            if (args.Length > 1)
            {
                if (int.TryParse(args[1], out var userSpecifiedMaxPathLength))
                {
                    maxPathLength = userSpecifiedMaxPathLength;
                }

                if (args.Length > 2)
                {
                    if (args[2] == "--apply")
                    {
                        reportOnly = false;
                    }
                }

            }

            if (reportOnly)
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

            var timer = System.Diagnostics.Stopwatch.StartNew();
            List<FileRenameOperationSummary> operations;

            try
            {
                //operations = ProcessFileOperations(reportOnly, path, maxPathLength);
            }
            finally
            {
                timer.Stop();
            }

            //GenerateReport(reportOnly, TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds), operations, maxPathLength);
            Console.WriteLine("File Renaming operation complete");
            Console.WriteLine("Press Enter key to exit...");
            Console.ReadLine();
        }

        public static List<DirectoryRenameOperationSummary> ProcessDirectories(string startingPath, bool reportOnly, int maxPathLength)
        {
            var directoryOperations = new List<DirectoryRenameOperationSummary>();
            var allDirectories = Directory.EnumerateDirectories(startingPath, "*.*", SearchOption.AllDirectories);
            foreach (var dir in allDirectories)
            {
                var fixedDirectoryName = FixDirectoryName(dir, maxPathLength);
                directoryOperations.Add(new DirectoryRenameOperationSummary(dir, fixedDirectoryName));

                if (reportOnly == false)
                {
                    Directory.Move(dir, fixedDirectoryName);
                }
            }

            return directoryOperations;
        }

        public static string FixDirectoryName(string directoryName, int maxPathLength)
        {
            // Priority Substitutions First
            if (directoryName.Length <= maxPathLength) return directoryName;
            foreach (var (key, value) in PrioritySubstitionMap)
            {
                directoryName = directoryName.Replace(key, value, StringComparison.InvariantCultureIgnoreCase);
                if (directoryName.Length <= maxPathLength) break;
            }

            // Generic substitutions second
            if (directoryName.Length <= maxPathLength) return directoryName;
            foreach (var (key, value) in FileNameSubstitutionMap)
            {
                directoryName = directoryName.Replace(key, value, StringComparison.InvariantCultureIgnoreCase);
                if (directoryName.Length <= maxPathLength) break;
            }

            return directoryName;
        }

        //public static List<FileRenameOperationSummary> ProcessFileOperations(bool reportOnly, string dir, int maxPathLength)
        //{
        //    var allFiles = Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories);
        //    var operations = new List<FileRenameOperationSummary>();
        //    foreach (var file in allFiles)
        //    {
        //        var fileInfo = new FileInfo(file);

        //        try
        //        {

        //            var newFilePath = GetFixedPath(maxPathLength, fileInfo);
        //            if (newFilePath == null) continue;

        //            // Add transformation to report history
        //            operations.Add(new FileRenameOperationSummary(currentFilePath, newFilePath, fileInfo.DirectoryName.Length));

        //            // Apply the file rename operation if reportOnly == false
        //            if (reportOnly == false)
        //            {
        //                try
        //                {
        //                    Console.WriteLine($"Renaming {currentFilePath} to {newFilePath}");
        //                    Directory.Move(currentFilePath, newFilePath);
        //                }
        //                catch (Exception e)
        //                {
        //                    Console.WriteLine($"[Error] Unable to rename file {currentFilePath} to {newFilePath}");
        //                    Console.WriteLine($"[Error] {e.Message}");
        //                }
        //            }
        //        }
        //        catch (PathTooLongException)
        //        {
        //            operations.Add(new FileRenameOperationSummary(currentFilePath, string.Empty, fileInfo.DirectoryName.Length) { PathTooLong = true });
        //        }
        //        catch (FilenameShorteningNotEnoughException)
        //        {
        //            operations.Add(new FileRenameOperationSummary(currentFilePath, string.Empty, fileInfo.DirectoryName.Length) { ShortenedFilenameTooLong = true });
        //        }
        //    }

        //    return operations;
        //}

        public static string GetFixedPath(int maxPathLength, FileInfo fileInfo, Dictionary<string, string> map)
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

        private static string FixDirectoryName(string directoryName, Dictionary<string, string> map)
        {
            foreach (var (key, value) in map)
            {
                directoryName = directoryName.Replace(key, value);
            }

            return directoryName;
        }

        public class PathTooLongException : Exception
        {
        }

        public class FilenameShorteningNotEnoughException : Exception
        {
        }

        public static void GenerateReport(bool reportOnly, TimeSpan timeToRun, IList<FileRenameOperationSummary> operations, int maxPathLength)
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
            reportLines.Add("<tr><th>Current Filename</th><th>Current Length</th><th>New Filename</th><th>New Length</th></tr>");
            reportLines.AddRange(pathTooLongFiles.Select(operation => $"<tr><td>{operation.CurrentFileName}</td><td>{operation.CurrentFileName.Length}</td><td>{operation.NewFileName}</td><td>{operation.NewFileName.Length}</td></tr>"));
            reportLines.Add("</table>");

            // Unfixable Files
            var unfixableFiles = operations.Where(o => o.ShortenedFilenameTooLong).ToList();
            reportLines.Add($"<h1>Filename Unfixable ({unfixableFiles.Count})</h1><br><table>");
            reportLines.Add("<tr><th>Current Filename</th><th>Current Length</th><th>New Filename</th><th>New Length</th></tr>");
            reportLines.AddRange(unfixableFiles.Select(operation => $"<tr><td>{operation.CurrentFileName}</td><td>{operation.CurrentFileName.Length}</td><td>{operation.NewFileName}</td><td>{operation.NewFileName.Length}</td></tr>"));
            reportLines.Add("</table>");

            // Fixable Files
            var fixableFiles = operations.Where(o => !o.PathTooLong && !o.ShortenedFilenameTooLong).ToList();
            reportLines.Add($"<h1>Fixable Files ({fixableFiles.Count})</h1><br><table>");
            reportLines.Add("<tr><th>Current Filename</th><th>Current Length</th><th>New Filename</th><th>New Length</th></tr>");
            reportLines.AddRange(fixableFiles.Select(operation => $"<tr><td>{operation.CurrentFileName}</td><td>{operation.CurrentFileName.Length}</td><td>{operation.NewFileName}</td><td>{operation.NewFileName.Length}</td></tr>"));
            reportLines.Add("</table>");

            reportLines.Add("</body></html>");

            Console.WriteLine($"Writing report to {ReportFileName}");
            File.WriteAllLines($"{ReportFileName}", reportLines);
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
}
