using System.Activities;
using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class SetOutputDependenceAttribute : ArgumentDependenceAttribute
{
	public override void Execute(ActivityDesigntime designTime, ModelProperty modelProperty)
	{
		ModelItem parent = modelProperty.Parent;
		SetOutputParameterValue obj = parent.GetCurrentValue() as SetOutputParameterValue;
		OutArgument outputParammeter = obj.OutputParammeter;
		InArgument value = obj.Value;
		SetVariableValueDepend.SetAssignStatement(parent, outputParammeter, value);
	}
}
