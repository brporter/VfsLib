using System;
using BryanPorter.Web.VirtualFS;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VfsLib.Tests
{
    [TestClass]
    public class CombinedPreferentialDataTest
    {
        private static readonly string m_connectionString = "Data Source=(local)\\SQLEXPRESS;Initial Catalog=VFS;Integrated Security=true;";

        private System.Web.Fakes.ShimHttpServerUtility m_fakeUtility;
        private System.Web.HttpServerUtility m_serverUtility;
        private IDisposable m_context;

        [TestInitialize]
        public void InitializeFakeUtility()
        {
            m_context = ShimsContext.Create();
            m_fakeUtility = new System.Web.Fakes.ShimHttpServerUtility();

            m_fakeUtility.MapPathString = (path) => path;

            m_serverUtility = m_fakeUtility.Instance;
        }

        [TestCleanup()]
        public void CleanupFakeUtility()
        {
            m_context.Dispose();
        }

        [TestMethod]
        public void ConstructorTest()
        {
            // Arrange
            var v = new VirtualData(
                new System.Web.HttpServerUtilityWrapper(m_serverUtility),
                m_connectionString
                );

            var p = new PhysicalData(
                new System.Web.HttpServerUtilityWrapper(m_serverUtility)
                );


            var c = new CombinedPreferentialData(v, p, FileSystemPrecedence.Default);

            v.ParentRepository = c;
            p.ParentRepository = c;

            // Act

            // Assert
            Assert.IsNotNull(c);
            Assert.IsInstanceOfType(c, typeof(CombinedPreferentialData));
        }

        [TestMethod]
        public void ParentRepositoryTest()
        {

            // Arrange
            var v = new VirtualData(
                new System.Web.HttpServerUtilityWrapper(m_serverUtility),
                m_connectionString
                );

            var p = new PhysicalData(
                new System.Web.HttpServerUtilityWrapper(m_serverUtility)
                );


            var c = new CombinedPreferentialData(v, p, FileSystemPrecedence.Default);

            v.ParentRepository = c;
            p.ParentRepository = c;

            // Assert
            Assert.IsTrue(v.ParentRepository == c);
            Assert.IsTrue(p.ParentRepository == c);
            Assert.IsTrue(c.ParentRepository == c);
            
        }

        [TestMethod]
        public void FileExistsTest()
        { 
        }

        [TestMethod]
        public void DirectoryExistsTest()
        { 
        }

        [TestMethod]
        public void GetChildrenTest()
        { 
        }

        [TestMethod]
        public void GetFilesTest()
        { 
        }

        [TestMethod]
        public void GetDirectoriesTest()
        { 
        }

        [TestMethod]
        public void GetDirectoryTest()
        { }

        [TestMethod]
        public void GetFileTest()
        { }

        [TestMethod]
        public void GetFileContentsTest()
        { }
    }
}
