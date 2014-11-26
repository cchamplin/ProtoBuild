using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProtoBuild.XBDMarkup.Elements
{
    [ChildTypes(new Type[] { typeof(Task) })]
    internal class ExecuteProcess : XBDElement
    {
        private ExecuteProcess()
            : base(null)
        {
        }
        public ExecuteProcess(XBDDef def)
            : base(def)
        {
            
        }

        protected override void Parse(XBDElement parent, XmlReader reader)
        {
        }
        public override void Build(XBDElement parent)
        {
        }
    }
}
