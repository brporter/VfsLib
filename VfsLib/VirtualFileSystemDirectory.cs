using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Threading.Tasks;

namespace BryanPorter.Web.VirtualFS
{
    public class VirtualFileSystemDirectory
        : VirtualDirectory
    {
        private readonly IVFSData m_data;
        private readonly HttpServerUtilityBase m_utility;

        public VirtualFileSystemDirectory(string virtualPath, IVFSData data, HttpServerUtilityBase utility)
            : base(virtualPath)
        {
            m_data = data;
            m_utility = utility;
        }

        public override System.Collections.IEnumerable Children
        {
            get { return m_data.GetChildren(VirtualPath); }
        }

        public override System.Collections.IEnumerable Directories
        {
            get 
            {
                return m_data.GetDirectories(VirtualPath);
            }
        }

        public override System.Collections.IEnumerable Files
        {
            get
            {
                return m_data.GetFiles(VirtualPath);
            }
        }
    }
}
