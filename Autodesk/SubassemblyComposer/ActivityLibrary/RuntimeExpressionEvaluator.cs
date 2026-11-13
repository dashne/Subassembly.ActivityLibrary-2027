using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class RuntimeExpressionEvaluator<T>
{
	public static T Evaluate(string vbExpression)
	{
		CompilerParameters compilerParameters = new CompilerParameters();
		compilerParameters.CompilerOptions = "/target:library /optimize";
		compilerParameters.GenerateInMemory = true;
		compilerParameters.IncludeDebugInformation = false;
		compilerParameters.ReferencedAssemblies.Add("mscorlib.dll");
		compilerParameters.ReferencedAssemblies.Add("System.dll");
		string value = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Subassembly.WorkflowEngine.dll");
		compilerParameters.ReferencedAssemblies.Add(value);
		string value2 = " \r\n                                using Autodesk.SubassemblyComposer.WorkflowEngine;\r\n                                public class TemporaryClass \r\n                                {\r\n                                   public static <T> Run() \r\n                                   {\r\n                                     return <vbExpression>; \r\n                                   }\r\n                                }";
		StringBuilder stringBuilder = new StringBuilder(value2);
		stringBuilder.Replace("<vbExpression>", vbExpression);
		stringBuilder.Replace("<T>", typeof(T).Name);
		stringBuilder.Replace("True", "true");
		CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("CSharp");
		value2 = stringBuilder.ToString();
		CompilerResults compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, value2);
		if (!compilerResults.Errors.HasErrors)
		{
			Type type = compilerResults.CompiledAssembly.CreateInstance("TemporaryClass").GetType();
			return (T)type.GetMethod("Run").Invoke(type, null);
		}
		return default;
	}
}
