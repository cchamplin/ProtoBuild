using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using System.Resources;
namespace ProtoBuild.Assembly
{
    public class Generator
    {
        public void Generate(string tmpPath, string exeFile)
        {

            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            
            if (!Directory.Exists(tmpPath))
            {
                Directory.CreateDirectory(tmpPath);
            }
            GenerateManifest(tmpPath, exeFile);

            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = tmpPath + exeFile;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("ProtoBuild.dll");
            parameters.ReferencedAssemblies.Add("FastSerialize.dll");
            parameters.GenerateInMemory = false;
            parameters.CompilerOptions = "/optimize";
            parameters.CompilerOptions += string.Format(" /win32manifest:\"{0}\"", tmpPath + exeFile + ".manifest");
            GenerateResources(tmpPath);

            parameters.EmbeddedResources.Add(tmpPath + "_protoResources.resources");

            parameters.TempFiles = new TempFileCollection(tmpPath, false);
            parameters.MainClass = "ProtoBuild.Installer";
            var provider = CodeDomProvider.CreateProvider("CSharp");

            

            string sources = GetCode();
            CompilerResults r = provider.CompileAssemblyFromSource(parameters, sources);
            var col = r.Output;
            foreach (var f in col)
            {
                Console.WriteLine(f);
            }
               
        }
        public string GetCode()
        {
            var assm = this.GetType().Assembly;
            var stream = assm.GetManifestResourceStream("ProtoBuild.Assembly.Installer.cs");
             stream.Position = 0;
             using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
             {
                 return reader.ReadToEnd();
             }

        }
        public void GenerateManifest(string tmpPath, string exeFile)
        {
            var assm = this.GetType().Assembly;
            var stream = assm.GetManifestResourceStream("ProtoBuild.Assembly.manifest.xml");
            stream.Position = 0;
            string manifestData = "";
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                manifestData = reader.ReadToEnd();
            }
            using (StreamWriter writer = new StreamWriter(tmpPath + exeFile + ".manifest"))
            {
                writer.Write(manifestData);
            }
        }
        public void GenerateResources(string tmpPath)
        {
            using (ResourceWriter res = new ResourceWriter(tmpPath + "_protoResources.resources"))
            {
                
                var fs = new FileStream("ProtoBuild.dll",FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
                res.AddResource("Assembly_ProtoBuild", fs, true);

               // byte[] b = new byte[fs.Length];
               // fs.Read(b,0,b.Length);
               // res.AddResource("Assembly_ProtoBuild", b);
                fs = new FileStream("FastSerialize.dll", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                res.AddResource("Assembly_FastSerialize", fs, true);

               // b = new byte[fs.Length];
               // fs.Read(b, 0, b.Length);
                //res.AddResource("Assembly_FastSerialize", b);
                fs = new FileStream(tmpPath + "tasks.json", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                res.AddResource("tasks.json", fs, true);


                fs = new FileStream(tmpPath + "variabledef.json", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                res.AddResource("variabledef.json", fs, true);


                fs = new FileStream(tmpPath + "executionplan.json", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                res.AddResource("executionplan.json", fs, true);


                fs = new FileStream(tmpPath + "filedata.pack", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                res.AddResource("filedata.pack", fs, true);
 

               
            }
        }
    }
}
