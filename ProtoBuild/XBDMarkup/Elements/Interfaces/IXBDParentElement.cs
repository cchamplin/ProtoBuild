using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild.XBDMarkup.Elements.Interfaces
{
    public interface IXBDParentElement
    {
        List<XBDElement> Children { get; set; }
    }
}
