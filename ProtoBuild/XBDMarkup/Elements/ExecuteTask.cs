using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProtoBuild.XBDMarkup.Elements
{
    [ChildTypes(new Type[] { typeof(Task) })]
    [ChildTypes(new Type[] { typeof(ExecutionPlan) })]
    internal class ExecuteTask : XBDElement
    {

        public string TaskName { get; set; }
        private ExecuteTask()
            : base(null)
        {
        }
        public ExecuteTask(XBDDef def)
            : base(def)
        {

        }

        protected override void Parse(XBDElement parent, XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "name")
                {
                    this.Name = reader.Value;
                }
                if (reader.Name == "task")
                {
                    this.TaskName = reader.Value;
                }
            }
        }
        public override void Build(XBDElement parent)
        {
        }
    }
}
