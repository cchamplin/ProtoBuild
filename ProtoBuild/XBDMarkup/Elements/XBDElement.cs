using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ProtoBuild.XBDMarkup;

namespace ProtoBuild.XBDMarkup.Elements
{
    public abstract class XBDElement
    {
        protected XBDDef XBDDef { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        protected int LineContext { get; set; }
        public enum ElementRunPlan
        {
            BUILD_ONLY,
            INSTALL_ONLY,
            BUILD_INSTALL
        }
        protected ElementRunPlan _runAt;

        internal ElementRunPlan Plan
        {
            get {
                return _runAt;
            }
            set {
                _runAt = value;
            }
        }
        public XBDElement(XBDDef def)
        {
            XBDDef = def;
        }
        public void DoParse(XBDElement parent, XmlReader reader)
        {
            LineContext = ((IXmlLineInfo)reader).LineNumber;
            Parse(parent, reader);
        }
        public abstract void Build(XBDElement parent);

        protected abstract void Parse(XBDElement parent, XmlReader reader);
    }
}
