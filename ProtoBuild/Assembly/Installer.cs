using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ProtoBuild
{
    public class Installer
    {
        public static void Main(string[] args)
        {
            try
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                var installer = new Installer();

               
                currentDomain.AssemblyResolve += installer.currentDomain_AssemblyResolve;
                Console.WriteLine("Starting");
                installer.PreExec();
                installer.Exec();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured in init");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            Console.ReadLine();
        }
        public void PreExec()
        {
            var assm = this.GetType().Assembly;
            foreach (var m in assm.GetManifestResourceNames())
            {
                Console.WriteLine(m);
            }


    
          
            try
            {
                var rs = new ResourceManager("_protoResources", assm);
                var stream = rs.GetStream("Assembly_ProtoBuild");
                Console.WriteLine(stream.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Failed2");
            }

           

            

        }

        public void Exec()
        {
            try
            {
                ProtoBuild build = new ProtoBuild();
                var stream = GetDataStream("tasks.json");
                build.ImportTasks(stream);
                stream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured in exec");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private Stream GetDataStream(string name)
        {
            var assm = this.GetType().Assembly;
            var rs = new ResourceManager("_protoResources", assm); 
            return rs.GetStream(name);
        }

        private System.Reflection.Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {

                

                string[] fields = args.Name.Split(',');
                string name = fields[0];
                string culture = fields[2];
                
                

                if (name == "ProtoBuild")
                {
                    var stream = GetDataStream("Assembly_ProtoBuild");
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    stream.Close();
                    return System.Reflection.Assembly.Load(data);
                }
                else if (name == "FastSerialize")
                {
                    var stream = GetDataStream("Assembly_FastSerialize");
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    stream.Close();
                    return System.Reflection.Assembly.Load(data);
                }


                if (name.EndsWith(".resources") && !culture.EndsWith("neutral")) return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }
    }
}
