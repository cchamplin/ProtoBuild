using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using ProtoBuild.XBDMarkup.Errors;
namespace ProtoBuild.XBDMarkup.Elements
{
    [ChildTypes(new Type[] { typeof(Task) })]
    internal class RemoveFile : XBDElement
    {
        private string _path;

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                this._path = value;
            }
        }

        public string ResolvedPath { 
            get
            {
                return this.XBDDef.Builder.ReplaceVariables(_path, base.LineContext);
            } 
            set
            {
                this._path = value;
            } 
        }
        protected List<String> deletionPaths;
        public List<String> DeletionPaths
        {
            get
            {
                return deletionPaths;
            }
            set
            {
                deletionPaths = value;
            }
        }
        private RemoveFile()
            : base(null)
        {
        }
        public RemoveFile(XBDDef def)
            : base(def)
        {

        }

        protected override void Parse(XBDElement parent, XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "path")
                {
                    this.ResolvedPath = reader.Value;
                }
            }
        }
        public override void Build(XBDElement parent)
        {
            var absolutePath = base.XBDDef.Builder.ResolvePath(this.ResolvedPath);

            var pDirectory = System.IO.Path.GetDirectoryName(absolutePath);
            var pFilePattern = System.IO.Path.GetFileName(absolutePath);
            if (!Directory.Exists(pDirectory))
            {
                base.XBDDef.PushError(new DirectoryNotFoundWarning() { Message = pDirectory, SourceLine = base.LineContext });
                return;
            }
            DirectoryInfo di = new DirectoryInfo(pDirectory);
            var fileList = di.EnumerateFiles(pFilePattern).ToList();
            var dirList = di.EnumerateDirectories(pFilePattern).ToList();
            // Talking about directory?
            if (fileList.Count > 0 || dirList.Count > 0)
            {
                deletionPaths = new List<String>();
                foreach (var item in fileList)
                {
                    deletionPaths.Add(item.FullName);
                }
                foreach (var item in dirList)
                {
                    deletionPaths.Add(item.FullName);
                }
            }
            return;
        }
    }
}
