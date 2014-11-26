using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ProtoBuild.XBDMarkup.Elements.Interfaces;

namespace ProtoBuild.XBDMarkup.Elements
{
    [ChildTypes(new Type[] { }, typeof(RegistryLookup))]
    internal class VariableDefinition : XBDElement, IXBDParentElement, IValueProvider
    {
        public string RawValue { get; set; }
        public List<XBDElement> Children
        {
            get;
            set;
        }

        public string ProvideValue()
        {
            if (string.IsNullOrEmpty(this.RawValue))
            {
                foreach (object child in Children)
                {
                    ((XBDElement)child).Build(this);
                    if (((IValueProvider)child).HasValue())
                    {
                     return ((IValueProvider)child).ProvideValue();
                    
                    }
                }
                // Todo should this be valid to return no value?
                return "";
            }
            else
            {
                return this.XBDDef.Builder.ReplaceVariables(this.RawValue, base.LineContext, false);
            }
        }
        public bool HasValue()
        {
            if (!string.IsNullOrEmpty(this.RawValue))
            {
                return true;
            }
            foreach (object child in Children)
            {
                ((XBDElement)child).Build(this);
                if (((IValueProvider)child).HasValue())
                {
                    return true;

                }
            }
            return false;
        }

        private VariableDefinition()
            : base(null)
        {
        }
        public VariableDefinition(XBDDef def)
            : base(def)
        {
            Children = new List<XBDElement>();
        }

        protected override void Parse(XBDElement parent, XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "name")
                {
                    this.Name = reader.Value;
                }
                if (reader.Name == "value")
                {
                    this.RawValue = reader.Value;
                }
            }
            base.XBDDef.Builder.AddVariable(this.Name, this);
            this.ParseElement(this.XBDDef, reader);
        }
        public override void Build(XBDElement parent)
        {
            foreach (object child in Children)
            {
                ((XBDElement)child).Build(this);
            }
        }
    }
}
