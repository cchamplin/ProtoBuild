using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using ProtoBuild;
using ProtoBuild.Assembly;
using ProtoBuild.XBDMarkup.Elements;
using ProtoBuild.XBDMarkup.Elements.Interfaces;
using ProtoBuild.XBDMarkup.Elements.Requirements;
using ProtoBuild.XBDMarkup.Errors;

namespace ProtoBuild.XBDMarkup
{
    public class XBDDef
    {
        private bool _failed = false;
        public bool Failed
        {
            get
            {
                return _failed;
            }
            set
            {
                if (value == true && this.ParentDef != null)
                {
                    GetRoot().Failed = true;
                }
            }
        }

        static XBDDef()
        {
            // Todo implement a better way to do this...
            ElementExtensions.RegisterElement("copy", typeof(CopyFile));
            ElementExtensions.RegisterElement("add-key", typeof(CreateRegistryKey));
            ElementExtensions.RegisterElement("execute-process", typeof(ExecuteProcess));
            ElementExtensions.RegisterElement("execute-task", typeof(ExecuteTask));
            ElementExtensions.RegisterElement("execution-plan", typeof(ExecutionPlan));
            ElementExtensions.RegisterElement("include", typeof(Include));
            ElementExtensions.RegisterElement("msbuild", typeof(MSBuild));
            ElementExtensions.RegisterElement("registry", typeof(RegistryLookup));
            ElementExtensions.RegisterElement("delete-tree", typeof(RemoveFile));
            ElementExtensions.RegisterElement("task", typeof(XBDMarkup.Elements.Task));
            ElementExtensions.RegisterElement("define", typeof(VariableDefinition));
            ElementExtensions.RegisterElement("file", typeof(FileRequirement));
            ElementExtensions.RegisterElement("os", typeof(OSRequirement));
            ElementExtensions.RegisterElement("running-process", typeof(ProcessRequirement));
            ElementExtensions.RegisterElement("registry", typeof(RegistryRequirement));
            ElementExtensions.RegisterElement("requirement", typeof(Requirement));

        }

      
        internal Dictionary<string, XBDMarkup.Elements.Task> Tasks;
        protected Dictionary<string, XBDDef> Inclusions;
        protected XBDDef ParentDef;
        // Todo switch this to an interface
        public Builder Builder { get; set; }
        internal string Namespace { get; set; }
        internal string File { get; set; }


        public XBDDef(Builder builder, string name = "__root", XBDDef parent = null)
        {
            Namespace = name;
            ParentDef = parent;

            
            Tasks = new Dictionary<string, Elements.Task>();
            Inclusions = new Dictionary<string, XBDDef>();
            this.Builder = builder;
        }

        public void PushErrors(IEnumerable<ErrorBase> errors)
        {
            foreach (ErrorBase error in errors)
            {
                if (error.Source == null)
                    error.Source = this;
                Builder.PushError(error);
            }
        }
        public void PushError(ErrorBase error)
        {
            if (error.Source == null)
                error.Source = this;
            Builder.PushError(error);
        }

        public XBDDef GetRoot()
        {
            XBDDef def = this;
            while (def.ParentDef != null)
            {
                def = def.ParentDef;
            }
            return def;
        }
        public XBDDef LocateDef(string ns)
        {
            if (ns.Contains('.'))
            {
                string searchName = ns.Substring(0, ns.IndexOf('.'));
                if (Inclusions.ContainsKey(searchName))
                    return Inclusions[searchName].LocateDef(ns.Substring(ns.IndexOf('.') + 1));
                else
                    return null;
            }
            else
                return this;
        }
        
       
        public void AddInclusion(XBDDef inclusion)
        {
            Inclusions.Add(inclusion.Namespace, inclusion);
        }


        
        

       
        public void Parse(string url)
        {
            Uri uri;
            if (this.File != null && this.File != "")
            {
                Builder.PushError(new ParseError() { Message = "Definition in use", SourceLine = 0, Source = this });
                return;
            }
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
            {
                Builder.PushError(new FileNotFoundError() { Message = url, SourceLine = 0, Source = this });
                return;
            }
            if (uri.IsFile)
            {
                var pDirectory = System.IO.Path.GetDirectoryName(url);
                Builder.AddStaticVariable("config-directory", pDirectory);
            }
            else
            {
                // Todo we should try and parse out if we can pull more files from the same Url
                Builder.AddStaticVariable("config-directory", System.Environment.CurrentDirectory);
            }
            this.File = url;

            using (XmlReader reader = new XmlTextReader(url))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.ToLower().Contains("build"))
                        {
                            parseSchema(reader.ReadSubtree());
                        }
                        else
                        {
                            throw new Exception("root element in XBD must be a build element");
                        }
                    }
                }
            }
            return;
        }
       

        private void parseSchema(XmlReader reader)
        {
            reader.Read();
                       
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                   
                        parseElement(reader.ReadSubtree());
                    
                }
            }
          
        }

       /* public List<BuildDef> Compile()
        {
            
            CompileElements(_elementDefinitions);
            return _classDefinitions.Values.Where(def => def.Create == true).ToList();
        }*/
       
        private void parseElement(XmlReader reader)
        {
            reader.Read();
            var element = ElementExtensions.ResolveElementType(null, reader.Name, this);
            if (element != null)
            {
                element.DoParse(null, reader);
            }
            else
            {
                GetRoot().Failed = true;
                this.Failed = true;
                Builder.PushError(new ParseError() { Message = "Unexpected element type: " + reader.Name, SourceLine = ((IXmlLineInfo)reader).LineNumber });
            }
            /*switch (reader.Name)
            {
                
                case "":
                    var newType = new ComplexType(this);
                    newType.DoParse(null, reader);
                    this._typeDefinitions.Add(newType);
                    break;
                case "element":
                    var newElement = new Element(this);
                    newElement.DoParse(null, reader);
                    this._elementDefinitions.Add(newElement);
                    break;
                case "simpletype":
                    var newSType = new SimpleType(this);
                    newSType.DoParse(null, reader);
                    this._typeDefinitions.Add(newSType);
                    break;
                case "complexContent":
                case "annotation":
                case "appinfo":
                case "GenericType":
                case "GenericParameter":
                case "sequence":
                case "list":
                case "pattern":
                case "restriction":
                case "extension":
                case "simplecontent":
                    throw new Exception("Unexpected element type: " + reader.Name);

            }*/
        }
        public void Build()
        {
            foreach (var def in Inclusions.Values)
            {
                def.Build();
            }
            foreach (XBDMarkup.Elements.Task task in this.Tasks.Values)
            {
                task.Build(null);
            }           
            
            return;
        }
        
    }
}
