using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuild.XBDMarkup.Errors;

namespace ProtoBuild.XBDMarkup.Elements.Interfaces
{
    public interface IRequirement
    {
        bool Validate(List<RequirementError> errors);
    }
}
