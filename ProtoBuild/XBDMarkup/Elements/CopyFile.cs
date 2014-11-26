using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuild.XBDMarkup.Elements.Interfaces;
using ProtoBuild.XBDMarkup.Errors;

namespace ProtoBuild.XBDMarkup.Elements
{
    [ChildTypes(new Type[] { typeof(Task) })]
    internal class CopyFile : XBDElement, IDatagram
    {

        internal class CopyDef
        {
            public string FileName
            {
                get; set;
            }
            internal string SourceDirectory
            {
                get;
                set;
            }
            public int DataPosition
            {
                get;
                set;
            }
            public int DataLength
            {
                get;
                set;
            }
            public string RelativePath
            {
                get;
                set;
            }
        }
        private string _source;
        private string _dest;
        private List<CopyDef> _copyFiles;
        public List<CopyDef> CopyFiles
        {
            get
            {
                return _copyFiles;
            }
            set
            {
                _copyFiles = value;
            }
        }

        public string RawSource
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }
        public string RawDest
        {
            get
            {
                return _dest;
            }
            set
            {
                _dest = value;
            }
        }

        protected string Source
        {
            get
            {
                return base.XBDDef.Builder.ReplaceVariables(_source, base.LineContext);
            }
            set
            {
                _source = value;
            }
        }
        protected string Dest
        {
            get
            {
                return base.XBDDef.Builder.ReplaceVariables(_dest, base.LineContext);
            }
            set
            {
                _dest = value;
            }
        }
        private CopyFile()
            : base(null)
        {
        }
        public CopyFile(XBDDef def) 
            : base(def)
        {
        }
        public override void Build(XBDElement parent)
        {
            var absoluteSource = base.XBDDef.Builder.ResolvePath(this.Source);

            var pDirectory = System.IO.Path.GetDirectoryName(absoluteSource);
            var pFilePattern = System.IO.Path.GetFileName(absoluteSource);
            DirectoryInfo di = null;
            if (Directory.Exists(absoluteSource))
            {
                pFilePattern = "*";
                di = new DirectoryInfo(absoluteSource);
            }
            else
            {
                if (!Directory.Exists(pDirectory))
                {
                    base.XBDDef.PushError(new DirectoryNotFoundWarning() { Message = pDirectory, SourceLine = base.LineContext });
                    return;
                }
                di = new DirectoryInfo(pDirectory);
            }

            
            var fileList = di.EnumerateFiles(pFilePattern).ToList();
            var dirList = di.EnumerateDirectories(pFilePattern).ToList();
            if (fileList.Count > 0 || dirList.Count > 0)
            {
                _copyFiles = new List<CopyDef>();
                foreach (var item in fileList)
                {
                    _copyFiles.Add(new CopyDef() { FileName = item.Name, SourceDirectory = item.DirectoryName });
                }
                foreach (var item in dirList)
                {
                    RecursiveAdd(item, item.Name);
                }
            }
            // Todo should we be worried about copying empty directory structure?
            if (_copyFiles.Count > 0)
            {
                base.XBDDef.Builder.RegisterDatagram(this);
            }


        }
        private void RecursiveAdd(DirectoryInfo di, string relPath)
        {
            var files = di.GetFiles();
            var dirs = di.GetDirectories();
            foreach (var file in files)
            {
                _copyFiles.Add(new CopyDef() { FileName = file.Name, SourceDirectory = file.DirectoryName, RelativePath = relPath });
            }
            foreach (var dir in dirs)
            {
                RecursiveAdd(dir, relPath + Path.DirectorySeparatorChar + dir.Name);
            }
        }
        protected override void Parse(XBDElement parent, System.Xml.XmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "source")
                {
                    this.Source = reader.Value;
                }
                if (reader.Name == "destination" || reader.Name == "dest")
                {
                    this.Dest = reader.Value;
                }
            }
        }

        public void SetDataPosition(int index, int position, int length)
        {
            _copyFiles[index].DataPosition = position;
            _copyFiles[index].DataLength = length;
        }

        public IEnumerable<Stream> GetDataStreams(Action<int> setIndex)
        {
            for (int x = 0; x < _copyFiles.Count; x++)
            {
                setIndex(x);
                yield return (new FileStream(_copyFiles[x].SourceDirectory + Path.DirectorySeparatorChar + _copyFiles[x].FileName, FileMode.Open));
            }
        }
        public void ReadDataStream(Stream sw)
        {
        }
    }
}
