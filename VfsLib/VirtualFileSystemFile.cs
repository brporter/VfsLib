using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Threading.Tasks;

namespace BryanPorter.Web.VirtualFS
{
    public class VirtualFileSystemFile
        : VirtualFile
    {
        private readonly IVFSData m_data;
        private readonly HttpServerUtilityBase m_utility;

        public VirtualFileSystemFile(string virtualPath, IVFSData data, HttpServerUtilityBase utility)
            : base(virtualPath)
        {
            m_data = data;
            m_utility = utility;
        }

        public override System.IO.Stream Open()
        {
            var data = m_data.GetFileContents(VirtualPath);

            if (data != null)
            {
                var ms = new System.IO.MemoryStream(data);
                ms.Seek(0, System.IO.SeekOrigin.Begin);

                return ms;
            }

            return null;
        }
    }
}
