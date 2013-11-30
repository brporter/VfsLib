using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BryanPorter.Web.VirtualFS;

namespace VfsLib.Tests
{
    [TestClass]
    public class PhysicalDataTest
    {
        private System.Web.Fakes.ShimHttpServerUtility _fakeUtility;
        private System.Web.HttpServerUtility _serverUtility;
        private IDisposable _context;

        [TestInitialize]
        public void InitializeFakeUtility()
        {
            _context = ShimsContext.Create();
            _fakeUtility = new System.Web.Fakes.ShimHttpServerUtility();

            _fakeUtility.MapPathString = (path) => path;

            _serverUtility = _fakeUtility.Instance;
        }

        [TestCleanup()]
        public void CleanupFakeUtility()
        {
            _context.Dispose();
        }

        [TestMethod]
        public void PhysicalDataParentRepositoryTest()
        {
            // Arrange
            var c = new PhysicalData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility)
                );

            // Assert
            Assert.IsNotNull(c);
            Assert.IsTrue(c.ParentRepository == c);

            c.ParentRepository = null;

            // Assert
            Assert.IsNull(c.ParentRepository);
        }

        [TestMethod]
        public void PhysicalDataFileExistsTest()
        {
            // Arrange
            var c = new PhysicalData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility)
                );

            // Create a temp file to check for file existence
            var tmpFilePath = System.IO.Path.GetTempFileName();
            var neTmpFilePath = System.IO.Path.GetTempFileName();

            System.IO.File.Delete(neTmpFilePath); // Delete the ne file to test the false case

            // Act
            var tmpResult = c.FileExists(tmpFilePath);
            var neTmpResult = c.FileExists(neTmpFilePath);

            // Assert
            Assert.IsFalse(neTmpResult);
            Assert.IsTrue(tmpResult);
        }

        [TestMethod]
        public void PhysicalDataDirectoryExistsTest()
        {
            // Arrange
            var c = new PhysicalData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility)
                );

            // Create a temp directory to check existence of
            var tmpDirPath = string.Format("{0}\\{1}", System.IO.Path.GetTempPath(), Guid.NewGuid());
            var neTmpDirPath = string.Format("{0}\\{1}", System.IO.Path.GetTempPath(), Guid.NewGuid());

            System.IO.Directory.CreateDirectory(tmpDirPath);

            // Act
            var tmpResult = c.DirectoryExists(tmpDirPath);
            var neTmpResult = c.DirectoryExists(neTmpDirPath);

            // Assert
            Assert.IsFalse(neTmpResult);
            Assert.IsTrue(tmpResult);
        }

        [TestMethod]
        public void PhysicalDataGetChildrenTest()
        { 
            // Arrange
            var c = new PhysicalData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility)
                );

            var tmpDirPath = Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            var expectedChildren = new List<string>();

            Directory.CreateDirectory(tmpDirPath);

            // Create some child files and some child directories
            for (int i = 0; i < 10; i++) {
                var file = Path.Combine(tmpDirPath, Guid.NewGuid().ToString());
                expectedChildren.Add(file);
                File.Create(file);
            }

            for (int i = 0; i < 4; i++) {
                var dir = Path.Combine(tmpDirPath, Guid.NewGuid().ToString());
                expectedChildren.Add(dir);
                Directory.CreateDirectory(dir);
            }

            // Act
            var children = c.GetChildren(tmpDirPath);

            // Assert
            Assert.IsTrue(children.Count() == expectedChildren.Count);

            foreach (var child in children)
            {
                // Note: VirtualPath always returns using a forward slash path seperator; for testing purposes, we change that
                // to a backslash.
                
                Assert.IsTrue(expectedChildren.Contains(child.VirtualPath.Replace("/", "\\").TrimEnd('\\')));
            }
        }

        [TestMethod]
        public void PhysicalDataGetFilesTest()
        {
            // Arrange
            var c = new PhysicalData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility)
                );

            var tmpDirPath = Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            var expectedChildren = new List<string>();

            Directory.CreateDirectory(tmpDirPath);

            // Create some child files
            for (int i = 0; i < 10; i++)
            {
                var file = Path.Combine(tmpDirPath, Guid.NewGuid().ToString());
                expectedChildren.Add(file);
                File.Create(file);
            }

            // Act
            var children = c.GetFiles(tmpDirPath);

            // Assert
            Assert.IsTrue(children.Count() == expectedChildren.Count);

            foreach (var child in children)
            {
                // Note: VirtualPath always returns using a forward slash path seperator; for testing purposes, we change that
                // to a backslash.

                Assert.IsTrue(expectedChildren.Contains(child.VirtualPath.Replace("/", "\\").TrimEnd('\\')));
            }
        }

        [TestMethod]
        public void PhysicalDataGetDirectoriesTest()
        {
            // Arrange
            var c = new PhysicalData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility)
                );

            var tmpDirPath = Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            var expectedChildren = new List<string>();

            Directory.CreateDirectory(tmpDirPath);

            // Create some child directories
            for (int i = 0; i < 4; i++)
            {
                var dir = Path.Combine(tmpDirPath, Guid.NewGuid().ToString());
                expectedChildren.Add(dir);
                Directory.CreateDirectory(dir);
            }

            // Act
            var children = c.GetDirectories(tmpDirPath);

            // Assert
            Assert.IsTrue(children.Count() == expectedChildren.Count);

            foreach (var child in children)
            {
                // Note: VirtualPath always returns using a forward slash path seperator; for testing purposes, we change that
                // to a backslash.

                Assert.IsTrue(expectedChildren.Contains(child.VirtualPath.Replace("/", "\\").TrimEnd('\\')));
            }
        }

        [TestMethod]
        public void PhysicalDataGetDirectoryTest()
        {             
            // Arrange
            var c = new PhysicalData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility)
                );

            var tmpDirPath = Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tmpDirPath);

            // Act
            var dir = c.GetDirectory(tmpDirPath);

            // Assert
            Assert.IsNotNull(dir);
            Assert.IsInstanceOfType(dir, typeof(VirtualFileSystemDirectory));
            Assert.IsTrue(dir.VirtualPath.Replace("/", "\\").TrimEnd('\\') == tmpDirPath);
            Assert.IsTrue(dir.IsDirectory);
        }

        [TestMethod]
        public void PhysicalDataGetFileTest()
        {
            // Arrange
            var c = new PhysicalData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility)
                );

            var filePath = Path.GetTempFileName();
            var expectedfileContents = Guid.NewGuid().ToString();

            File.WriteAllText(filePath, expectedfileContents);

            // Act
            var file = c.GetFile(filePath);

            // Assert
            Assert.IsNotNull(file);
            Assert.IsInstanceOfType(file, typeof(VirtualFileSystemFile));
            Assert.IsTrue(file.VirtualPath.Replace("/", "\\").TrimEnd('\\') == filePath);
            Assert.IsFalse(file.IsDirectory);

            using (var sr = new StreamReader(file.Open()))
            {
                var contents = sr.ReadToEnd();

                Assert.IsTrue(expectedfileContents == contents);
            }
        }

        [TestMethod]
        public void PhysicalDataGetFileContentsTest()
        {
            // Arrange
            var c = new PhysicalData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility)
                );

            var filePath = Path.GetTempFileName();
            var expectedfileContents = Guid.NewGuid().ToString();

            File.WriteAllText(filePath, expectedfileContents);

            // Act
            var fileContents = c.GetFileContents(filePath);

            // Assert
            Assert.IsNotNull(fileContents);
            var expectedBytes = System.Text.UnicodeEncoding.UTF8.GetBytes(expectedfileContents);

            Assert.IsTrue(fileContents.Length == expectedBytes.Length);

            for (int i = 0; i < fileContents.Length; i++)
            {
                Assert.IsTrue(fileContents[i] == expectedBytes[i]);
            }
        }
    }
}
