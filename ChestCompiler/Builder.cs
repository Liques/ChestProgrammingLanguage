using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.CodeDom.Compiler;

namespace ChestCompiler
{
    public static class Builder
    {
        public static CompilerResults Compile(string code, string sourceName = null)
        {
            //var codeTranspiled = Build.Transpile(textBlock);

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            if (sourceName != null)
            {
                File.WriteAllText(sourceName, code);                
            }

            return provider.CompileAssemblyFromSource(new CompilerParameters(), new string[] { code });

        }

            
    }


}
