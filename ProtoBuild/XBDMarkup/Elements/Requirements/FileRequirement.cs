using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuild.XBDMarkup.Elements.Interfaces;

namespace ProtoBuild.XBDMarkup.Elements.Requirements
{
    [ChildTypes(new Type[] { typeof(Requirement) })]
    class FileRequirement : XBDElement, IRequirement
    {
       
        private string _fileName;
        public string FileName { 
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        protected string ResolvedFileName
        {
            get
            {
                return base.XBDDef.Builder.ReplaceVariables(_fileName, base.LineContext);
            }
            set
            {
                _fileName = value;
            }
        }
        private FileRequirement()
            : base(null)
        {
        }
        public FileRequirement(XBDDef def) 
            : base(def)
        {
        }
        


        protected override void Parse(XBDElement parent, System.Xml.XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "path")
                {
                    this.FileName = reader.Value;
                }

            }
        }

        public bool Validate(List<Errors.RequirementError> errors)
        {
            bool validationResult = true;

            if (File.Exists(ResolvedFileName))
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
