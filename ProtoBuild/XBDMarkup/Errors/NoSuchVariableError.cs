using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild.XBDMarkup.Errors
{
    public class NoSuchVariableError : ErrorBase
    {
        public override string ToString()
        {
            return string.Format("Warning ({1}): No such variable {0} ", Message, SourceLine);
        }
    }
}
