using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild.XBDMarkup.Errors
{
    public class DirectoryNotFoundWarning : ErrorBase
    {
        public override string ToString()
        {
            return string.Format("Warning ({1}): Directory {0} not found", Message, SourceLine);
        }
    }
}
