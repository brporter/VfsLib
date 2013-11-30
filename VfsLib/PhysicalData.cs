using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Threading.Tasks;

namespace BryanPorter.Web.VirtualFS
{
    public class PhysicalData
        : IVFSData
    {
        private readonly HttpServerUtilityBase m_utility;

        public PhysicalData(HttpServerUtilityBase utility)
            : this(utility, null)
        {
        }

        public PhysicalData(HttpServerUtilityBase utility, IVFSData parent)
        {
            m_utility = utility;

            if (parent == null)
                ParentRepository = this;
            else
                ParentRepository = parent;
        }

        public IVFSData ParentRepository
        {
            get;
            set;
        }

        public bool FileExists(string virtualPath)
        {
            var path = m_utility.MapPath(virtualPath);

            return File.Exists(path);
        }

        public bool DirectoryExists(string virtualPath)
        {
            var path = m_utility.MapPath(virtualPath);

            return Directory.Exists(path);
        }

        public IEnumerable<System.Web.Hosting.VirtualFileBase> GetChildren(string virtualPath)
        {
            var files = (IEnumerable<VirtualFileBase>)GetFiles(virtualPath);
            var dirs = (IEnumerable<VirtualFileBase>)GetDirectories(virtualPath);

            return files.Concat(dirs);
        }

        public IEnumerable<VirtualFileSystemFile> GetFiles(string virtualPath)
        {
            var di = new DirectoryInfo(m_utility.MapPath(virtualPath));

            if (di.Exists)
            {
                var results = from item in di.GetFiles() select new VirtualFileSystemFile(string.Format("{0}/{1}", virtualPath, item.Name), ParentRepository, m_utility);

                return results;
            }

            return new List<VirtualFileSystemFile>();
        }

        public IEnumerable<VirtualFileSystemDirectory> GetDirectories(string virtualPath)
        {
            var di = new DirectoryInfo(m_utility.MapPath(virtualPath));

            if (di.Exists)
            {
                var results = from item in di.GetDirectories() select new VirtualFileSystemDirectory(string.Format("{0}/{1}", virtualPath, item.Name), ParentRepository, m_utility);

                return results;
            }

            return new List<VirtualFileSystemDirectory>();
        }

        public VirtualFileSystemDirectory GetDirectory(string virtualPath)
        {
            var di = new DirectoryInfo(m_utility.MapPath(virtualPath));

            if (di.Exists)
            {
                return new VirtualFileSystemDirectory(virtualPath, ParentRepository, m_utility);
            }

            return null;
        }

        public VirtualFileSystemFile GetFile(string virtualPath)
        {
            var fi = new FileInfo(m_utility.MapPath(virtualPath));

            if (fi.Exists)
            {
                return new VirtualFileSystemFile(virtualPath, ParentRepository, m_utility);
            }

            return null;
        }

        public byte[] GetFileContents(string virtualPath)
        {
            if (File.Exists(virtualPath))
            {
                return File.ReadAllBytes(virtualPath);
            }

            return null;
        }
    }
}
