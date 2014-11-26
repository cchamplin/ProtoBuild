using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuild.XBDMarkup.Elements.Interfaces;

namespace ProtoBuild.XBDMarkup.Elements.Requirements
{
    [ChildTypes(new Type[] { typeof(Requirement) })]
    class ProcessRequirement : XBDElement, IRequirement
    {
       
        string _procName;
        public string ProcessName
        {
            get
            {
                return _procName;
            }
            set
            {
                _procName = value;
            }
        }
        protected string ResolvedProcessName
        {
            get
            {
                return base.XBDDef.Builder.ReplaceVariables(_procName, base.LineContext);
            }
            set
            {
                _procName = value;
            }
        }
        private ProcessRequirement()
            : base(null)
        {
        }
        public ProcessRequirement(XBDDef def) 
            : base(def)
        {
        }


        protected override void Parse(XBDElement parent, System.Xml.XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "process" || reader.Name == "executable")
                {
                    this.ResolvedProcessName = reader.Value;
                }

            }
        }

        public bool Validate(List<Errors.RequirementError> errors)
        {
            bool validationResult = true;

            if (Process.GetProcessesByName(ResolvedProcessName).Length > 0)
            {
                validationResult = true;
            }
            else
            {
                validationResult = false;
            }
            return validationResult;
        }

        public override void Build(XBDElement parent)
        {
        }
    }
}
