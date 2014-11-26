using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ProtoBuild.XBDMarkup;
using ProtoBuild.XBDMarkup.Elements;

namespace ProtoBuild.XBDMarkup.Elements
{
    [ChildTypes(new Type[] { typeof(Task) })]
    internal class MSBuild : XBDElement
    {
      
        public List<XBDElement> Children
        {
            get;
            set;
        }
        private MSBuild()
            : base(null)
        {
        }
        public MSBuild(XBDDef def)
            : base(def)
        {
            Children = new List<XBDElement>();
        }

        protected override void Parse(XBDElement parent, XmlReader reader)
        {
        }
        public override void Build(XBDElement parent)
        {
        }
    }
}
