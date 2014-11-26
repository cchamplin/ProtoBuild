using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ProtoBuild.XBDMarkup.Elements.Interfaces;
using ProtoBuild.XBDMarkup.Elements.Requirements;
using ProtoBuild.XBDMarkup.Errors;

namespace ProtoBuild.XBDMarkup.Elements
{
    [ChildTypes(new Type[] { }, typeof(RemoveFile), typeof(CopyFile), typeof(CreateRegistryKey), typeof(ExecuteTask), typeof(ExecuteProcess), typeof(MSBuild), typeof(Requirement))]
    internal class Task : XBDElement, IXBDParentElement
    {
       
        public List<XBDElement> Children
        {
            get;
            set;
        }
       
        private Task() : base(null)
        {
        }
        public Task(XBDDef def)
            : base(def)
        {
            Children = new List<XBDElement>();
        }

        public string RunPlan
        {
            get
            {
                if (_runAt == ElementRunPlan.BUILD_INSTALL)
                {
                    return "BUILD_INSTALL";
                }
                else if (_runAt == ElementRunPlan.BUILD_ONLY)
                {
                    return "BUILD_ONLY";
                }
                else
                {
                    return "INSTALL_ONLY";
                }
            }
            set
            {
                switch (value.ToUpper())
                {
                    case "INSTALL_ONLY":
                    case "INSTALL":
                        _runAt = ElementRunPlan.INSTALL_ONLY;
                        break;
                    case "BUILD_ONLY":
                    case "BUILD":
                        _runAt = ElementRunPlan.BUILD_ONLY;
                        break;
                    default:
                        _runAt = ElementRunPlan.BUILD_INSTALL;
                        break;

                }
            }
        }

        protected override void Parse(XBDElement parent, XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "name")
                {
                    this.Name = reader.Value;
                }
                if (reader.Name == "runat")
                {
                    this.RunPlan = reader.Value;
                }
            }
            if (string.IsNullOrEmpty(this.Name))
            {
                this.XBDDef.Failed = true;
                this.XBDDef.PushError(new ParseError() { Message = "Task must be named", SourceLine = base.LineContext });
            }
            else
            {
                this.ParseElement(this.XBDDef, reader);
                if (this.XBDDef.Tasks.ContainsKey(this.Name.ToLower()))
                {
                    this.XBDDef.Failed = true;
                    this.XBDDef.PushError(new ParseError() { Message = "Duplicate task name: " + this.Name, SourceLine = base.LineContext });
                }
                this.XBDDef.Tasks.Add(this.Name.ToLower(), this);
            }
        }
        public override void Build(XBDElement parent)
        {
            foreach (XBDElement element in Children)
            {
                element.Build(this);
            }
        }
    }
}
