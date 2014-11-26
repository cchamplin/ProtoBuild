using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProtoBuild.XBDMarkup.Elements.Interfaces;
using ProtoBuild.XBDMarkup.Errors;

namespace ProtoBuild.XBDMarkup.Elements
{
    [ChildTypes(new Type[] { })]
    internal class Include : XBDElement
    {
        
        public string InclusionPath { get; set; }
        public string InclusionNamespace { get; set; }
        private Include()
            : base(null)
        {
        }
        public Include(XBDDef def)
            : base(def)
        {
        }

        protected override void Parse(XBDElement parent, System.Xml.XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "path")
                {
                    this.InclusionPath = reader.Value;
                }
                if (reader.Name == "ns" || reader.Name == "namespace")
                {
                    if (Regex.IsMatch(reader.Value, "([a-zA-Z0-9_-]+)"))
                    {
                        this.InclusionNamespace = reader.Value;
                    }
                    else
                    {
                        base.XBDDef.Failed = true;
                        base.XBDDef.PushError(new ParseError() { Message = "Inclusion namespace contains invalid characters" , SourceLine = base.LineContext });
                        return;
                    }
                    
                }
            }
            XBDDef newDef = new XBDDef(base.XBDDef.Builder,this.InclusionNamespace, this.XBDDef);
            var absolutePath = base.XBDDef.Builder.ResolvePath(InclusionPath);
            if (!System.IO.File.Exists(absolutePath))
            {
                absolutePath = base.XBDDef.Builder.ResolvePath(InclusionPath,"%config-directory%");
                if (!System.IO.File.Exists(absolutePath))
                {
                    base.XBDDef.Failed = true;
                    base.XBDDef.PushError(new FileNotFoundError() { Message = this.InclusionPath, SourceLine = base.LineContext });
                    return;
                }
            }
            newDef.Parse(absolutePath);
            XBDDef.AddInclusion(newDef);
            //this.ParseElement(XBDDef, reader);
        }

        public override void Build(XBDElement parent)
        {
            throw new NotImplementedException();
        }
    }
}
