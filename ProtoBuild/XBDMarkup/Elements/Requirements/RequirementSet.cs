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
    [ChildTypes(new Type[] { typeof(object) }, typeof(object))]
    internal class RequirementSet : XBDElement, IXBDParentElement
    {
       public List<XBDElement> Children
        {
            get;
            set;
        }
       private RequirementSet()
            : base(null)
        {
        }
       public RequirementSet(XBDDef def)
            : base(def)
        {
            Children = new List<XBDElement>();
        }

        protected override void Parse(XBDElement parent, XmlReader reader)
        {
        }
        public override void Build(XBDElement parent)
        {
            List<RequirementError> errors = new List<RequirementError>();
            foreach (IRequirement req in Children.Cast<IRequirement>())
            {
                if (!req.Validate(errors))
                {
                    base.XBDDef.Failed = true;
                }
            }
            if (errors.Count > 0)
            {
                base.XBDDef.PushErrors(errors.Cast<ErrorBase>());
            }
        }
    }
}
