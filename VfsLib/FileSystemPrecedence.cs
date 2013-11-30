using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryanPorter.Web.VirtualFS
{
    public enum FileSystemPrecedence
    {
        Default = Virtual,
        Physical = 1,
        Virtual = 2
    }
}
