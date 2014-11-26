using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuild.XBDMarkup.Elements.Interfaces;

namespace ProtoBuild.XBDMarkup.Elements.Requirements
{
    [ChildTypes(new Type[] { typeof(Requirement) })]
    internal class OSRequirement : XBDElement, IRequirement
    {
        
        public string Platform { get; set; }
        public string MinVersion { get; set; }
        public string MaxVersion { get; set; }

        private OSRequirement()
            : base(null)
        {
        }
        public OSRequirement(XBDDef def)
            : base(def)
        {
        }

        protected override void Parse(XBDElement parent, System.Xml.XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "platform")
                {
                    this.Platform = reader.Value;
                }
                if (reader.Name == "min-version")
                {
                    this.MinVersion = reader.Value;
                }
                if (reader.Name == "max-version")
                {
                    this.MaxVersion = reader.Value;
                }
            }
        }

        public bool Validate(List<Errors.RequirementError> errors)
        {
            bool windows = false;
            bool unix = false;
            bool linux = false;
            bool macos = false;
            bool validationResult = false;
            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                if (p == 6)
                {
                    macos = true;
                }
                else
                {
                    linux = true;
                }
            }
            else
            {
                windows = true;
            }

            switch (this.Platform.ToLower())
            {
                case "win":
                case "windows":
                case "win32":
                    validationResult = windows;
                    break;
                case "unix":
                    validationResult = unix;
                    break;
                case "linux":
                    validationResult = linux;
                    break;
                case "macos":
                    validationResult = macos;
                    break;
            }
            if (!validationResult)
            {
                errors.Add(new Errors.RequirementError() { Message = "Platform did not match", SourceLine = base.LineContext });
                return false;
            }
            if (!string.IsNullOrEmpty(MinVersion))
            {
                Version minV = new Version(MinVersion);
                if (minV.CompareTo(Environment.OSVersion.Version) > 0)
                {
                    validationResult = false;
                    errors.Add(new Errors.RequirementError() { Message = "Operating System does not meet minumum version", SourceLine = base.LineContext });
                }
            }
            if (validationResult && !string.IsNullOrEmpty(MaxVersion))
            {
                Version maxV = new Version(MaxVersion);
                if (maxV.CompareTo(Environment.OSVersion.Version) < 0)
                {
                    validationResult = false;
                    errors.Add(new Errors.RequirementError() { Message = "Operating System exceeds maximum version", SourceLine = base.LineContext });
                }
            }
            return validationResult;

           

        }

        public override void Build(XBDElement parent)
        {
           
        }
    }
}
