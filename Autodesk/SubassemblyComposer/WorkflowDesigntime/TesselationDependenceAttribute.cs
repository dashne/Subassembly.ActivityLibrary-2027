using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Model;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class TesselationDependenceAttribute : ArgumentDependenceAttribute
{
	internal const int MinTessellation = 2;

	internal const int MaxTessellation = 100;

	public override void Execute(ActivityDesigntime designTime, ModelProperty modelProperty)
	{
		if (modelProperty.Parent != null && modelProperty.Parent.Equals(designTime.Selection.SelectedModelItem) && modelProperty.Value.GetCurrentValue() is InArgument<int> inArgument && inArgument.Expression is Literal<int>)
		{
			Literal<int> literal = inArgument.Expression as Literal<int>;
			if (literal.Value < 2)
			{
				modelProperty.SetValue(new InArgument<int>(2));
			}
			else if (literal.Value > 100)
			{
				modelProperty.SetValue(new InArgument<int>(100));
			}
		}
	}
}
