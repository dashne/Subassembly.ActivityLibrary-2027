using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Model;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class LoopRepetitionAttribute : ArgumentDependenceAttribute
{
	internal const int MinLoopRepetition = 0;

	internal const int MaxLoopRepetition = 100;

	public override void Execute(ActivityDesigntime designTime, ModelProperty modelProperty)
	{
		if (modelProperty.Parent != null && modelProperty.Value != null && modelProperty.Parent.Equals(designTime.Selection.SelectedModelItem) && modelProperty.Value.GetCurrentValue() is InArgument<int> inArgument && inArgument.Expression is Literal<int>)
		{
			Literal<int> obj = inArgument.Expression as Literal<int>;
			if (obj.Value < 0)
			{
				modelProperty.SetValue(new InArgument<int>(0));
			}
			if (obj.Value > 100)
			{
				modelProperty.SetValue(new InArgument<int>(100));
			}
		}
	}
}
