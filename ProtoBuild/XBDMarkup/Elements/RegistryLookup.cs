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
    [ChildTypes(new Type[] { typeof(VariableDefinition) })]
    internal class RegistryLookup : XBDElement, IValueProvider
    {
        public String SubKey { get; set; }
        public RegistryItem RegistryItem { get; set; }
        private RegistryLookup()
            : base(null)
        {
        }
        public RegistryLookup(XBDDef def)
            : base(def)
        {
        }

        protected override void Parse(XBDElement parent, XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "key")
                {
                    this.RegistryItem = new RegistryItem(reader.Value, this.XBDDef.Builder, false);
                }
                if (reader.Name == "subkey")
                {
                    this.SubKey = reader.Value;
                }
            }
        }
        public override void Build(XBDElement parent)
        {
            
        }

        public string ProvideValue()
        {
            if (RegistryItem.IsNull(SubKey))
                return null;
            else
                return (string)RegistryItem.GetValue(this.SubKey);
        }
        public bool HasValue()
        {
            return !(RegistryItem.IsNull(SubKey));
        }
    }
}
