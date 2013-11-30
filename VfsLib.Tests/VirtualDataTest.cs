using System;
using BryanPorter.Web.VirtualFS;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VfsLib.Tests
{
    [TestClass]
    public class VirtualDataTest
    {
        private const string ConnectionString = "Data Source=(localdb)\v11.0;Initial Catalog=VFS;Integrated Security=true;";

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
        public void VirtualDataParentRepositoryTest()
        {
            // Arrange
            var c = new VirtualData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility), ConnectionString
                );

            // Assert
            Assert.IsNotNull(c);
            Assert.IsTrue(c.ParentRepository == c);

            // Act
            IVFSData empty = null;
            c.ParentRepository = empty;

            // Assert
            Assert.IsNull(c.ParentRepository);
        }

        [TestMethod]
        public void VirtualDataFileExistsTest()
        {
            // Arrange
            var c = new VirtualData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility), ConnectionString
            );

            // Act
            var resultOne = c.FileExists("/FooBar/FooBar.txt");
            var resultTwo = c.FileExists("/FooBar/NotThere.txt");

            // Assert
            Assert.IsTrue(resultOne);
            Assert.IsFalse(resultTwo);
        }

        [TestMethod]
        public void VirtualDataDirectoryExistsTest()
        { 
            // Arrange
            var c = new VirtualData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility), ConnectionString
            );

            // Act
            var resultOne = c.DirectoryExists("/FooBar/");
            var resultTwo = c.DirectoryExists("/FooBar");
            var resultThree = c.DirectoryExists("/");
            var resultFour = c.DirectoryExists("/Foo");

            // Assert
            Assert.IsTrue(resultOne);
            Assert.IsTrue(resultTwo);
            Assert.IsTrue(resultThree);
            Assert.IsFalse(resultFour);
        }

        [TestMethod]
        public void VirtualDataGetChildrenTest()
        {
            // Arrange
            var c = new VirtualData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility), ConnectionString
            );

            // Act
            var children = c.GetChildren("/FooBar/");

            // Assert
            foreach (var item in children)
            {
                Assert.IsInstanceOfType(item, typeof(System.Web.Hosting.VirtualFileBase));

                var f = item as VirtualFileSystemFile;
                var d = item as VirtualFileSystemDirectory;

                if (f != null)
                {
                    Assert.IsFalse(f.IsDirectory);
                    Assert.IsNotNull(f.Name);
                }
                else if (d != null)
                {
                    Assert.IsTrue(d.IsDirectory);
                    Assert.IsNotNull(d.Name);
                }
            }
        }

        [TestMethod]
        public void VirtualDataGetFilesTest()
        {
            // Arrange
            var c = new VirtualData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility), ConnectionString
            );

            // Act
            var children = c.GetFiles("/FooBar/");

            // Assert
            foreach (var item in children)
            {
                Assert.IsInstanceOfType(item, typeof(System.Web.Hosting.VirtualFileBase));

                var f = item as VirtualFileSystemFile;

                Assert.IsNotNull(f);
                Assert.IsFalse(f.IsDirectory);
                Assert.IsNotNull(f.Name);
            }
        }

        [TestMethod]
        public void VirtualDataGetDirectoriesTest()
        {
            // Arrange
            var c = new VirtualData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility), ConnectionString
            );

            // Act
            var children = c.GetDirectories("/");

            // Assert
            foreach (var item in children)
            {
                Assert.IsInstanceOfType(item, typeof(System.Web.Hosting.VirtualFileBase));

                var d = item as VirtualFileSystemDirectory;

                Assert.IsNotNull(d);
                Assert.IsTrue(d.IsDirectory);
                Assert.IsNotNull(d.Name);
            }
        }

        [TestMethod]
        public void VirtualDataGetDirectoryTest()
        {
            // Arrange
            var c = new VirtualData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility), ConnectionString
            );

            // Act
            var dir = c.GetDirectory("/FooBar/");

            // Assert
            Assert.IsNotNull(dir);
            Assert.IsTrue(dir.Name == "FooBar");
            Assert.IsTrue(dir.IsDirectory);
        }

        [TestMethod]
        public void VirtualDataGetFileTest()
        {
            // Arrange
            var c = new VirtualData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility), ConnectionString
            );

            // Act
            var file = c.GetFile("/FooBar/FooBar.txt");

            // Assert
            Assert.IsNotNull(file);
            Assert.IsTrue(file.Name == "FooBar.txt");
            Assert.IsFalse(file.IsDirectory); 
        }

        [TestMethod]
        public void VirtualDataGetFileContentsTest()
        {
            // Arrange
            var c = new VirtualData(
                new System.Web.HttpServerUtilityWrapper(_serverUtility), ConnectionString
            );

            // Act
            var fileContents = c.GetFileContents("/FooBar/FooBar.txt");
            var expectedFileContents = "Hello, World";
            var actualFileContents = System.Text.Encoding.Unicode.GetString(fileContents);

            // Assert
            Assert.IsNotNull(actualFileContents);
            Assert.IsTrue(actualFileContents == expectedFileContents);
        }
    }
}
