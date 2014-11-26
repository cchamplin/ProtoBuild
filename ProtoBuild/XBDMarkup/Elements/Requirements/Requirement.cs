using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ProtoBuild.XBDMarkup.Elements.Interfaces;
using ProtoBuild.XBDMarkup.Errors;

namespace ProtoBuild.XBDMarkup.Elements.Requirements
{
    [ChildTypes(new Type[] { typeof(Task) }, typeof(FileRequirement), typeof(RegistryRequirement), typeof(ProcessRequirement), typeof(OSRequirement))]
    internal class Requirement : XBDElement, IXBDParentElement, IRequirement
    {
       
        public List<XBDElement> Children
        {
            get;
            set;
        }
        public bool Negate { get; set; }
        private Requirement()
            : base(null)
        {
        }
        public Requirement(XBDDef def)
            : base(def)
        {
            Children = new List<XBDElement>();
        }
        public bool Validate(List<Errors.RequirementError> errors)
        {
            List<Errors.RequirementError> childErrors = new List<Errors.RequirementError>();
 	        foreach (IRequirement requirement in Children.Cast<IRequirement>())
            {
                if (requirement.Validate(childErrors) || Negate)
                {
                    return true;
                }
            }
            foreach (RequirementError error in childErrors)
                errors.Add(error);
            errors.Add(new RequirementError { Message = "Could not satisfy requirement", SourceLine = base.LineContext});
            return false;
        }
        protected override void Parse(XBDElement parent, XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "name")
                {
                    this.Name = reader.Value;
                }
                if (reader.Name == "negate")
                {
                    this.Negate = bool.Parse(reader.Value);
                }
            }
            this.ParseElement(this.XBDDef,reader);
        }
        public override void Build(XBDElement parent)
        {
            foreach (XBDElement child in Children)
            {
                child.Build(this);
            }
        }
    }
}
