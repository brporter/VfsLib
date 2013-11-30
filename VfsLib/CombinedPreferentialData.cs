using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace BryanPorter.Web.VirtualFS
{
    public class CombinedPreferentialData
        : IVFSData
    {
        private readonly IVFSData m_virtualData;
        private readonly IVFSData m_physicalData;
        private readonly FileSystemPrecedence m_precedence;

        public CombinedPreferentialData(IVFSData virtualData, IVFSData physicalData, FileSystemPrecedence precedence)
        {
            m_virtualData = virtualData;
            m_physicalData = physicalData;

            m_virtualData.ParentRepository = this;
            m_physicalData.ParentRepository = this;

            m_precedence = precedence;
        }

        private static void OrderAssignment<T>(Func<T> first, Func<T> second, Func<T> physical, Func<T> virt, FileSystemPrecedence precedence)
        {
            if (precedence == FileSystemPrecedence.Virtual)
            {
                first = virt;
                second = physical;
            }
            else
            {
                first = physical;
                second = virt;
            }
        }

        private static T ExecuteInOrder<T>(Func<T> phys, Func<T> virt, FileSystemPrecedence precedence, Func<T, bool> predicate)
        {
            Func<T> first = null, second = null;

            OrderAssignment(first, second, phys, virt, precedence);

            var firstResult = first();

            if (predicate(firstResult))
                return firstResult;

            var secondResult = second();

            if (predicate(secondResult))
                return secondResult;

            Debug.Assert(false, "Unreachable code... reached. Snap!");
            throw new InvalidOperationException();
        }

        private static T ExecuteInOrder<T>(Func<T> phys, Func<T> virt, FileSystemPrecedence precedence, Func<T, T, T> processor)
        {
            Func<T> first = null, second = null;
            OrderAssignment(first, second, phys, virt, precedence);

            var firstResult = first();
            var secondResult = second();

            return processor(firstResult, secondResult);
        }

        public bool FileExists(string virtualPath)
        {
            Func<bool> physicalExists = () => (m_physicalData != null && m_physicalData.FileExists(virtualPath));
            Func<bool> virtualExists = () => (m_virtualData != null && m_virtualData.FileExists(virtualPath));

            return ExecuteInOrder(physicalExists, virtualExists, m_precedence, (b) => b);
        }

        public bool DirectoryExists(string virtualPath)
        {
            Func<bool> physicalExists = () => (m_physicalData != null && m_physicalData.DirectoryExists(virtualPath));
            Func<bool> virtualExists = () => (m_virtualData != null && m_virtualData.DirectoryExists(virtualPath));

            return ExecuteInOrder(physicalExists, virtualExists, m_precedence, (b) => b);
        }

        private static IEnumerable<T> GetThings<T>(string virtualPath, Func<IEnumerable<T>> phys, Func<IEnumerable<T>> virt, FileSystemPrecedence precedence)
            where T : VirtualFileBase
        {
            return ExecuteInOrder(phys, virt, precedence, (fresult, sresult) => { return fresult.Union(sresult, new VirtualFileComparer<T>()); });
        }

        public IEnumerable<VirtualFileBase> GetChildren(string virtualPath)
        {
            Func<IEnumerable<VirtualFileBase>> phys = () => (m_physicalData != null ? m_physicalData.GetChildren(virtualPath) : null);
            Func<IEnumerable<VirtualFileBase>> virt = () => (m_virtualData != null ? m_virtualData.GetChildren(virtualPath) : null);
            
            // Programmers are lazy. :)
            return GetThings<VirtualFileBase>(virtualPath, phys, virt, m_precedence);
        }

        public IEnumerable<VirtualFileSystemFile> GetFiles(string virtualPath)
        {
            Func<IEnumerable<VirtualFileSystemFile>> phys = () => (m_physicalData != null ? m_physicalData.GetFiles(virtualPath) : null);
            Func<IEnumerable<VirtualFileSystemFile>> virt = () => (m_virtualData != null ? m_virtualData.GetFiles(virtualPath) : null);

            return GetThings<VirtualFileSystemFile>(virtualPath, phys, virt, m_precedence);
        }

        public IEnumerable<VirtualFileSystemDirectory> GetDirectories(string virtualPath)
        {
            Func<IEnumerable<VirtualFileSystemDirectory>> phys = () => { return (m_physicalData != null ? m_physicalData.GetDirectories(virtualPath) : null); };
            Func<IEnumerable<VirtualFileSystemDirectory>> virt = () => { return (m_virtualData != null ? m_virtualData.GetDirectories(virtualPath) : null); };

            return GetThings<VirtualFileSystemDirectory>(virtualPath, phys, virt, m_precedence);
        }

        public byte[] GetFileContents(string virtualPath)
        {
            Func<byte[]> phys = () => { return (m_physicalData != null ? m_physicalData.GetFileContents(virtualPath) : null); };
            Func<byte[]> virt = () => { return (m_virtualData != null ? m_virtualData.GetFileContents(virtualPath) : null); };

            return ExecuteInOrder(phys, virt, m_precedence, (result) => { if (result == null) { return false; } return true; });
        }

        public VirtualFileSystemDirectory GetDirectory(string virtualPath)
        {
            Func<VirtualFileSystemDirectory> phys = () => { return (m_physicalData != null ? m_physicalData.GetDirectory(virtualPath) : null); };
            Func<VirtualFileSystemDirectory> virt = () => { return (m_virtualData != null ? m_virtualData.GetDirectory(virtualPath) : null); };

            return ExecuteInOrder(phys, virt, m_precedence, (result) => { if (result == null) { return false; } return true; });
        }

        public VirtualFileSystemFile GetFile(string virtualPath)
        {
            Func<VirtualFileSystemFile> phys = () => { return (m_physicalData != null ? m_physicalData.GetFile(virtualPath) : null); };
            Func<VirtualFileSystemFile> virt = () => { return (m_virtualData != null ? m_virtualData.GetFile(virtualPath) : null); };

            return ExecuteInOrder(phys, virt, m_precedence, (result) => { if (result == null) { return false; } return true; });
        }

        public IVFSData ParentRepository
        {
            get { return this; }
            set { }
        }

        private class VirtualFileComparer<T>
            : IEqualityComparer<T> where T : VirtualFileBase
        {
            public bool Equals(T x, T y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
