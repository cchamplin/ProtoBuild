using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ProtoBuild.Types;
using ProtoBuild.XBDMarkup.Elements.Interfaces;

namespace ProtoBuild.XBDMarkup.Elements
{
    [ChildTypes(new Type[] { typeof(Task) })]
    internal class CreateRegistryKey : XBDElement
    {

        public String SubKey { get; set; }
        public RegistryItem RegistryItem { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        private CreateRegistryKey()
            : base(null)
        {
        }
        public CreateRegistryKey(XBDDef def)
            : base(def)
        {

        }

        protected override void Parse(XBDElement parent, XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "key")
                {
                    this.RegistryItem = new RegistryItem(reader.Value, this.XBDDef.Builder);
                }
                if (reader.Name == "name")
                {
                    this.SubKey = reader.Value;
                }
                if (reader.Name == "value")
                {
                    this.Value = reader.Value;
                }
                if (reader.Name == "type")
                {
                    this.Type = reader.Value;
                }
            }
        }
        public override void Build(XBDElement parent)
        {
        }
    }
}
