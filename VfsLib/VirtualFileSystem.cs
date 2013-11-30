using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace BryanPorter.Web.VirtualFS
{
    /// <summary>
    /// An implementation of a Virtual Path Provider that merges the physical file system with a virtual, database based one.
    /// </summary>
    public class VirtualFileSystem
        : VirtualPathProvider
    {
        private readonly IVFSData m_data;
        private readonly HttpServerUtilityBase m_utility;

        #region Constructors
        public VirtualFileSystem(IVFSData data, HttpServerUtilityBase serverUtility, FileSystemPrecedence precendence)
            : base()
        {
            m_data = data;
            m_utility = serverUtility;

            Precedence = precendence;
        }


        public VirtualFileSystem(IVFSData data, HttpServerUtilityBase serverUtility)
            : this(data, serverUtility, FileSystemPrecedence.Default)
        { }
        #endregion

        /// <summary>
        /// Gets or sets the file system precendence to use when resolving conflicts.
        /// </summary>
        /// <remarks>
        /// The view presented to the hosting runtime is a merged view of both the virtual and physical filesystems. When both the virtual file system and the physical system, contain a file with
        /// the same name and path, one file system must take precendence to satisfy the request. This property allows the specification of the precedence. The default is to prefer the virtual
        /// file system.
        /// </remarks>
        public FileSystemPrecedence Precedence
        {
            get;
            set;
        }

        public override string CombineVirtualPaths(string basePath, string relativePath)
        {
            return base.CombineVirtualPaths(basePath, relativePath);
        }

        public override bool DirectoryExists(string virtualDir)
        {
            return m_data.DirectoryExists(virtualDir);
        }

        public override bool FileExists(string virtualPath)
        {
            return m_data.FileExists(virtualPath);
        }

        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        public override string GetCacheKey(string virtualPath)
        {
            return base.GetCacheKey(virtualPath);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return m_data.GetDirectory(virtualDir);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return m_data.GetFile(virtualPath);
        }

        //public override string GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies)
        //{
        //    return base.GetFileHash(virtualPath, virtualPathDependencies);
        //}
    }
}
