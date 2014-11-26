using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProtoBuild.Assembly;
using ProtoBuild.XBDMarkup;
using ProtoBuild.XBDMarkup.Elements;
using ProtoBuild.XBDMarkup.Elements.Interfaces;
using ProtoBuild.XBDMarkup.Errors;

namespace ProtoBuild
{
    public class Builder : IVariableReplacer
    {
        internal Dictionary<string, VariableDefinition> DefinedVariables;
        protected Dictionary<string, string> InternalVariables;

        internal List<IDatagram> Datagrams;
        internal ExecutionPlan ExecutionPlan;

        protected List<ErrorBase> _errors;

        internal XBDDef rootDef;

        public Builder()
        {
            _errors = new List<ErrorBase>();
            DefinedVariables = new Dictionary<string, VariableDefinition>();
            InternalVariables = new Dictionary<string, string>();
            Datagrams = new List<IDatagram>();
            PopulateVariables();
        }

        public XBDDef GetRoot()
        {
            return rootDef;
        }

        public void AddStaticVariable(string name, string value)
        {
            if (InternalVariables.ContainsKey(name))
                InternalVariables[name] = value;
            else
                InternalVariables.Add(name, value);

        }
        protected void PopulateVariables()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows)
            {
                InternalVariables.Add("user-docs", System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                InternalVariables.Add("program-files", System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
                InternalVariables.Add("system", System.Environment.GetFolderPath(Environment.SpecialFolder.System));
                InternalVariables.Add("desktop", System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                InternalVariables.Add("appdata", System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                InternalVariables.Add("data-directory", System.Environment.CurrentDirectory);
            }
        }
        internal void RegisterDatagram(IDatagram datagram)
        {
            Datagrams.Add(datagram);
        }
        internal void AddVariable(string name, VariableDefinition definition)
        {
            if (DefinedVariables.ContainsKey(name.ToLower()))
            {
                DefinedVariables[name.ToLower()] = definition;
            }
            else
            {
                DefinedVariables.Add(name.ToLower(), definition);
            }
        }
        internal VariableDefinition GetVariable(string name)
        {
            return DefinedVariables[name];
        }
        public string GetInternalVariableValue(string name)
        {
            if (InternalVariables.ContainsKey(name.ToLower()))
            {
                return InternalVariables[name.ToLower()];
            }
            return null;
        }
        public string GetVariableValue(string name, bool preferInternal = true)
        {
            if (preferInternal)
            {
                if (InternalVariables.ContainsKey(name.ToLower()))
                {
                    return InternalVariables[name.ToLower()];
                }
                else if (DefinedVariables.ContainsKey(name.ToLower()))
                {
                    return DefinedVariables[name].ProvideValue();
                }
            }
            else
            {
                if (DefinedVariables.ContainsKey(name.ToLower()))
                {
                    return DefinedVariables[name].ProvideValue();
                }
                else if (InternalVariables.ContainsKey(name.ToLower()))
                {
                    return InternalVariables[name.ToLower()];
                }
            }
            return null;
        }

        public string[] GetErrors()
        {
            var results = new List<String>();
            for (int x = 0; x < _errors.Count; x++)
                results.Add(_errors[x].ToString());
            return results.ToArray();

        }


        public void PushError(ErrorBase error)
        {
            this._errors.Add(error);
        }

        public string ReplaceVariables(string search, int lineContext, bool includeDefined = true)
        {
            var matches = Regex.Matches(search, "(%([a-zA-Z0-9_-]+)%)");
            if (!includeDefined)
            {
                foreach (Match match in matches)
                {
                    var replacement = GetInternalVariableValue(match.Groups[2].Value);
                    if (replacement != null)
                    {
                        search = search.Replace(match.Groups[1].Value, replacement);
                    }
                    else
                    {
                        this.PushError(new NoSuchVariableError() { Message = match.Groups[1].Value, SourceLine = lineContext });
                    }
                }
            }
            else
            {
                foreach (Match match in matches)
                {
                    var replacement = GetVariableValue(match.Groups[2].Value);
                    if (replacement != null)
                    {
                        search = search.Replace(match.Groups[1].Value, replacement);
                    }
                    else
                    {
                        this.PushError(new NoSuchVariableError() { Message = match.Groups[1].Value, SourceLine = lineContext });
                    }
                }
            }
            return search;
        }

        public string ResolvePath(string p, string basePath = "%data-directory%")
        {
            Uri uri;
            if (!Uri.TryCreate(p, UriKind.RelativeOrAbsolute, out uri))
            {
                // We couldn't parse this as a path/uri
                // Possibly generate an error here?
                return p;
            }
            else if (!uri.IsAbsoluteUri)
            {
                if (p[0] == System.IO.Path.DirectorySeparatorChar)
                {
                    var currentWorkingPath = ReplaceVariables(basePath, 0, false);
                    if (currentWorkingPath[currentWorkingPath.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                    {
                        return currentWorkingPath + System.IO.Path.DirectorySeparatorChar + p;
                    }
                    else
                    {
                        return currentWorkingPath + p;
                    }
                }
                else
                {
                    var currentWorkingPath = ReplaceVariables(basePath, 0, false);
                    if (currentWorkingPath[currentWorkingPath.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                    {
                        return currentWorkingPath + p.Substring(1);
                    }
                    else
                    {
                        return currentWorkingPath + System.IO.Path.DirectorySeparatorChar + p;
                    }
                }
            }
            else
            {
                return p;
            }
        }
        public void ImportTasks(Stream s)
        {
            var serializer = new FastSerialize.Serializer(typeof(FastSerialize.JsonSerializerGeneric));
            var tasks = serializer.Deserialize<Dictionary<string, XBDMarkup.Elements.Task>>(s);
            if (rootDef == null) {
                rootDef = new XBDDef(this);
            }
            GetRoot().Tasks = tasks;
        }

        public void Parse(string url)
        {
            if (rootDef == null)
            {
                rootDef = new XBDDef(this);

            }
            GetRoot().Parse(url);
        }

        public void Compile()
        {
            if (!GetRoot().Failed)
            {

                var tmpPath = Path.GetTempPath() + "ProtoBuild" + Path.DirectorySeparatorChar;


                FileStream fs = new FileStream(tmpPath + "filedata.pack", FileMode.Create,FileAccess.Write,FileShare.Read);
                int totalPosition = 0;
                foreach (var datagram in Datagrams)
                {
                    int index = 0;
                    Action<int> indexSetter = (s => index = s);
                    foreach (var stream in datagram.GetDataStreams(indexSetter))
                    {
                        var len = stream.CopyStream(fs);
                        datagram.SetDataPosition(index, totalPosition, len);
                        totalPosition += len;
                    }
                }
                fs.Flush();
                fs.Close();

                // Todo possibly only serialize tasks that will be executed through the
                // execution plan and it's decendants
                var serializer = new FastSerialize.Serializer(typeof(FastSerialize.JsonSerializerGeneric));
                var serializedTasks = serializer.Serialize(this.GetRoot().Tasks);
                var sw = new StreamWriter(tmpPath + "tasks.json");
                sw.Write(serializedTasks);
                sw.Flush();
                sw.Close();

                var serializedDefinitions = serializer.Serialize(this.DefinedVariables);
                sw = new StreamWriter(tmpPath + "variabledef.json");
                sw.Write(serializedDefinitions);
                sw.Flush();
                sw.Close();

                var executionPlan = serializer.Serialize(this.ExecutionPlan);
                sw = new StreamWriter(tmpPath + "executionplan.json");
                sw.Write(executionPlan);
                sw.Flush();
                sw.Close();

                var gen = new Generator();
                gen.Generate(tmpPath, "Setup.exe");
            }
           
        }
        public void Build()
        {

            foreach (VariableDefinition def in DefinedVariables.Values)
            {
                def.Build(null);
            }
            GetRoot().Build();
            return;
        }
    }
}
