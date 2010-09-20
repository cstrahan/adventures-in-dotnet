using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using Mono.Cecil.Cil;
using INPCAttribute;

namespace WeavingTask
{
    public class INPCWeavingTask : Task
    {
        public override bool Execute()
        {
            ProcessAssemblies();
            return true;
        }

        private static readonly string AssemblyShadowCopiesFolderName = "AssemblyShadowCopies";

        [Required]
        public string TargetDir { get; set; }

        public void ProcessAssembly(AssemblyDefinition sourceAssembly)
        {
            var propertyChangedEventHandlerTypeDefinition =
               sourceAssembly.MainModule.Import(typeof(PropertyChangedEventHandler)).Resolve();
            var boolTypeDefinition =
                sourceAssembly.MainModule.Import(typeof(Boolean)).Resolve();
            var propertyChangedEventArgsTypeDefinition =
                sourceAssembly.MainModule.Import(typeof(PropertyChangedEventArgs)).Resolve();

            foreach (TypeDefinition type in sourceAssembly.MainModule.Types)
            {
                foreach (PropertyDefinition prop in type.Properties)
                {
                    if (prop.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(NotifyAttribute).FullName))
                    {
                        var handlerRef =
                            type.Fields.First(f => f.FieldType.Name == "PropertyChangedEventHandler");
                        var processor = prop.SetMethod.Body.GetILProcessor();
                        var instructions = prop.SetMethod.Body.Instructions;

                        prop.SetMethod.Body.MaxStackSize = 4;
                        
                        prop.SetMethod.Body.Variables.Add(new VariableDefinition("propertyChanged",
                                                                                 propertyChangedEventHandlerTypeDefinition));
                        prop.SetMethod.Body.Variables.Add(new VariableDefinition("isPropertyChangedNull",
                                                                                 boolTypeDefinition));


                        Instruction ret =
                            prop.SetMethod.Body.Instructions.Last();

                        processor.InsertBefore(prop.SetMethod.Body.Instructions.First(),
                                               processor.Create(OpCodes.Nop));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Ldarg_0));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Ldfld, handlerRef));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Stloc_0));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Ldloc_0));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Ldnull));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Ceq));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Stloc_1));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Ldloc_1));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Brtrue_S, ret));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Nop));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Ldloc_0));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Ldarg_0));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Ldstr, prop.Name));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Newobj,
                                             propertyChangedEventArgsTypeDefinition.Methods.First(
                                                 m => m.IsConstructor)));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Callvirt,
                                             propertyChangedEventHandlerTypeDefinition.Methods.First(
                                                 m => m.Name == "Invoke")));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Nop));

                        processor.InsertBefore(
                            instructions.Last(),
                            processor.Create(OpCodes.Nop));
                    }
                }
            }
        }

        public void ProcessAssemblies()
        {
            List<PropertyDefinition> replaceProperties = new List<PropertyDefinition>();
            var assemblyPaths = Directory.GetFiles(TargetDir, "*.dll", SearchOption.AllDirectories);
            foreach (string assemblyPath in assemblyPaths)
            {
                if (assemblyPath.Contains(AssemblyShadowCopiesFolderName))
                    continue;

                bool isAssemblyModified = false;

                string shadowCopyPath = CreateCopy(assemblyPath);



                AssemblyDefinition sourceAssembly = AssemblyDefinition.ReadAssembly(shadowCopyPath);

                ProcessAssembly(sourceAssembly);

                sourceAssembly.Write(shadowCopyPath);
                File.Copy(shadowCopyPath, assemblyPath, true);
            }
        }

        private string CreateCopy(string assemblyPath)
        {
            string mainDir = Path.Combine(TargetDir, AssemblyShadowCopiesFolderName);
            if (Directory.Exists(mainDir) == false)
                Directory.CreateDirectory(mainDir);

            string fileTimeUTC = Path.Combine(mainDir, DateTime.Now.ToFileTimeUtc().ToString());
            Directory.CreateDirectory(fileTimeUTC);

            string finalPath = Path.Combine(fileTimeUTC, Path.GetFileName(assemblyPath));
            File.Copy(assemblyPath, finalPath);
            return finalPath;

        }
    }
}
