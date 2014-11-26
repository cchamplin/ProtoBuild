using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuild.XBDMarkup;

namespace BuildTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ProtoBuild.Builder build = new ProtoBuild.Builder();
            build.AddStaticVariable("install-directory", "J:\\Cosmos User Kit2");
            //def.AddStaticVariable("data-directory", "J:\\Cosmos\\cosmos-105304");
            //def.Parse("J:\\Cosmos\\vs2010.xml");
            build.AddStaticVariable("data-directory", "J:\\Cosmos\\test-data");
            build.Parse("J:\\Cosmos\\tester.xml");
           // def.ImportTasks((new StreamReader("tasks.json")).BaseStream);


            /*build.Build();
            var errors = build.GetErrors();
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
            build.Compile();*/
            Console.ReadLine();
        }
    }
}
