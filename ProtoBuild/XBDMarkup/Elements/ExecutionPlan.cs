using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ProtoBuild.XBDMarkup.Elements.Interfaces;
using ProtoBuild.XBDMarkup.Errors;

namespace ProtoBuild.XBDMarkup.Elements
{
    [ChildTypes(new Type[] { }, typeof(ExecuteTask))]
    internal class ExecutionPlan : XBDElement, IXBDParentElement
    {
        
        public List<XBDElement> Children
        {
            get;
            set;
        }
        private ExecutionPlan()
            : base(null)
        {
        }
        public ExecutionPlan(XBDDef def)
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
            }
            if (this.XBDDef.Builder.ExecutionPlan == null)
            {
                this.XBDDef.Builder.ExecutionPlan = this;
            }
            else
            {
                this.XBDDef.Failed = true;
                this.XBDDef.PushError(new ParseError() { Message = "XBD cannot contain multiple execution plans", SourceLine = base.LineContext });
            }
            this.ParseElement(this.XBDDef, reader);
        }
        public override void Build(XBDElement parent)
        {
        }
    }
}
