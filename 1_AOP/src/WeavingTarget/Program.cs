using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using WeavingTask;

namespace WeavingTarget
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = new INPCWeavingTask();

            var assembly = typeof(Program).Assembly;
            var asmDef = AssemblyDefinition.ReadAssembly(assembly.Location);
            task.ProcessAssembly(asmDef);
            asmDef.Write(Path.Combine(Path.GetDirectoryName(assembly.Location), "Foo.dll"));

            Console.WriteLine("Press any key to exit . . .");
            Console.ReadKey(true);
        }
    }
}
