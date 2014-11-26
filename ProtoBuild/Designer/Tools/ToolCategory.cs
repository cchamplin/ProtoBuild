using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild.Designer.Tools
{
    public class ToolCategory
    {
        private List<Tool> _tools;
        public ToolCategory(string name)
        {
            this.Name = name;
            _tools = new List<Tool>();
        }
        public string Name { get; set; }
        public List<Tool> Tools { get { return _tools; } set { _tools = value; } }
    }
}
