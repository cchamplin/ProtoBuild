using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild.XBDMarkup.Errors
{
    public abstract class ErrorBase
    {
        public string Message { get; set; }
        public int SourceLine { get; set; }
        public XBDDef Source { get; set; }
        public override string ToString()
        {
            return string.Format("Error ({1}): {0}", Message, SourceLine);
        }
    }
}
