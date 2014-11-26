using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild.XBDMarkup.Errors
{
    public class FileNotFoundError : ErrorBase
    {
        public override string ToString()
        {
            return string.Format("Error ({1}): File {0} not found", Message, SourceLine);
        }
    }
}
