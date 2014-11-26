using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild.XBDMarkup
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class ChildTypes : Attribute
    {
        public Type[] TypeChain;
        public Type[] Types;
        public ChildTypes(Type[] typeChain, params Type[] types)
        {
            this.TypeChain = typeChain;
            this.Types = types;
        }
    }
}
