using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild.XBDMarkup
{
    internal interface IVariableReplacer
    {
        string ReplaceVariables(string search, int lineContext, bool includeDefined = true);
    }
}
