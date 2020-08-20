using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MbmFilenameFixer.Unit.Tests
{
    [TestClass]
    public class GetFixedPathTest
    {
        private const int MaxPathLength = 370;

        //[TestMethod]
        //public void GetFixedPath_ValidPath_ReturnsNull()
        //{
        //    // Arrange
        //    var path = "/my/test/file.name";
        //    var fileInfo = new FileInfo(path);

        //    // Act
        //    var result = Program.GetFixedPath(MaxPathLength, path, fileInfo);

        //    // Assert
        //    Assert.IsNull(result);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(Program.PathTooLongException))]
        //public void GetFixedPath_PathTooLong_ReturnsNull()
        //{
        //    // Arrange
        //    var pathTooLong = GetPathOfLength(400);
        //    var path = $"{pathTooLong}/file.name";
        //    var fileInfo = new FileInfo(path);

        //    // Act
        //    var result = Program.GetFixedPath(MaxPathLength, path, fileInfo);

        //    // Assert -- Expect Exception
        //}

        //[TestMethod]
        //public void GetFixedPath_FixableFilename_ReturnsFixedFilename()
        //{
        //    // Arrange
        //    var fileName = "16Characters.exe";
        //    var path = GetPathOfLength(MaxPathLength - 16)+$"/{fileName}";

        //    var fileInfo = new FileInfo(path);

        //    // Act
        //    var result = Program.GetFixedPath(MaxPathLength, path, fileInfo);

        //    // Assert
        //    Assert.IsNotNull(result);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(Program.FilenameShorteningNotEnoughException))]
        //public void GetFixedPath_UnFixableFilename_ReturnsFixedFilename()
        //{
        //    // Arrange
        //    var fileName = "18Characters08.exe";
        //    var path = GetPathOfLength(MaxPathLength - 8) + $"/{fileName}";

        //    var fileInfo = new FileInfo(path);

        //    // Act
        //    var result = Program.GetFixedPath(MaxPathLength, path, fileInfo);

        //    // Assert
        //    // Assert -- Expect Exception
        //}


        [TestMethod]
        public async Task GetFixedPath_AllFiles_AbleToFix()
        {
            var currentDir = Environment.CurrentDirectory;
            var configurationPath = new DirectoryInfo(
                Path.GetFullPath(Path.Combine(currentDir, "Resources", "Filenames.txt")));

            string path;
            using var filePaths = new StreamReader(configurationPath.ToString());
            var line = 0;
            while ((path = await filePaths.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;

                var fileInfo = new FileInfo(path);
                var dirName = fileInfo.DirectoryName;
                var fixedDirectoryName = Program.ApplyDirectoryRenamingRules(dirName, MaxPathLength);
                Assert.IsTrue(fixedDirectoryName.Length <= MaxPathLength, $"Could not fix {dirName}\n New Name: {fixedDirectoryName}\n Old Length: {dirName.Length} Length: {fixedDirectoryName.Length} \n Line: {line}");
                line++;
            }
        }

        private static string GetPathOfLength(int length)
        {
            var pathBuilder = new StringBuilder(length);
            pathBuilder.Append("/");
            for (var i = 1; i < length-1; i++)
            {
                pathBuilder.Append("a");
            }
            pathBuilder.Append("/");
            return pathBuilder.ToString();
        }
    }
}
