using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Threading.Tasks;

namespace BryanPorter.Web.VirtualFS
{
    /// <summary>
    ///  Interface defining the data repository
    /// </summary>
    public interface IVFSData
    {
        /// <summary>
        /// Determines if a file at the specified path exists in the virtual file system.
        /// </summary>
        /// <param name="virtualPath">The path of the file to check for existence of.</param>
        /// <returns>true if the file exists in the virtual file system; otherwise, false.</returns>
        bool FileExists(string virtualPath);

        /// <summary>
        /// Determines if a directory at the specified path exists in the virtual file system.
        /// </summary>
        /// <param name="virtualPath">The path of the file to check for existence of.</param>
        /// <returns>true if the file exists in the virtual file system; otherwise, false.</returns>
        bool DirectoryExists(string virtualPath);

        /// <summary>
        /// Gets the paths of all children of the specified virtual path in the virtual file system.
        /// </summary>
        /// <param name="virtualPath">The path to the directory to retrieve the children of.</param>
        /// <returns>An IEnumerable of VirtualFileBase instances representing the children of the specified path.</returns>
        IEnumerable<VirtualFileBase> GetChildren(string virtualPath);

        IEnumerable<VirtualFileSystemFile> GetFiles(string virtualPath);

        IEnumerable<VirtualFileSystemDirectory> GetDirectories(string virtualPath);

        byte[] GetFileContents(string virtualPath);

        VirtualFileSystemDirectory GetDirectory(string virtualPath);

        VirtualFileSystemFile GetFile(string virtualPath);

        IVFSData ParentRepository
        {
            get;
            set;
        }
    }
}
