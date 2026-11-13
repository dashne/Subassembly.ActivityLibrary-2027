using System.Activities;
using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.WorkflowEngine;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class SetVariableValueDepend : ArgumentDependenceAttribute
{
	public override void Execute(ActivityDesigntime designTime, ModelProperty modelProperty)
	{
		ModelItem parent = modelProperty.Parent;
		SetVariableValue obj = parent.GetCurrentValue() as SetVariableValue;
		OutArgument variableName = obj.VariableName;
		InArgument variableValue = obj.VariableValue;
		SetAssignStatement(parent, variableName, variableValue);
	}

	internal static void SetAssignStatement(ModelItem modelItem, OutArgument outArg, InArgument inArg)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (outArg != null)
		{
			if (outArg.ArgumentType == typeof(int))
			{
				text = GetOutArgumentText<int>(outArg);
			}
			else if (outArg.ArgumentType == typeof(double))
			{
				text = GetOutArgumentText<double>(outArg);
			}
			else if (outArg.ArgumentType == typeof(string))
			{
				text = GetOutArgumentText<string>(outArg);
			}
			else if (outArg.ArgumentType == typeof(Slope))
			{
				text = GetOutArgumentText<Slope>(outArg);
			}
			else if (outArg.ArgumentType == typeof(Grade))
			{
				text = GetOutArgumentText<Grade>(outArg);
			}
		}
		if (inArg != null)
		{
			if (inArg.ArgumentType == typeof(int))
			{
				text2 = GetInArgumentText<int>(inArg);
			}
			else if (inArg.ArgumentType == typeof(double))
			{
				text2 = GetInArgumentText<double>(inArg);
			}
			else if (inArg.ArgumentType == typeof(string))
			{
				text2 = GetInArgumentText<string>(inArg);
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			string displayName = text + " = " + text2;
			ModelProperty modelProperty = modelItem.Properties["DisplayName"];
			string newComment = CommentsConverter.ExtractComment(modelProperty.Value.GetCurrentValue() as string);
			displayName = CommentsConverter.InjectComment(displayName, newComment);
			modelProperty.SetValue(displayName);
		}
	}

	private static string GetOutArgumentText<T>(OutArgument outArg)
	{
		string result = string.Empty;
		if (outArg.Expression is VisualBasicReference<T> visualBasicReference)
		{
			result = visualBasicReference.ExpressionText;
		}
		return result;
	}

	private static string GetInArgumentText<T>(InArgument inArg)
	{
		string result = string.Empty;
		if (inArg.Expression is VisualBasicValue<T> visualBasicValue)
		{
			result = visualBasicValue.ExpressionText;
		}
		return result;
	}
}
