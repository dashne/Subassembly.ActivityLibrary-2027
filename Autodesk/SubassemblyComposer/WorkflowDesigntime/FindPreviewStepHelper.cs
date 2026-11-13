using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Collections.Generic;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class FindPreviewStepHelper : FindActivityHelper
{
	private FlowNode currentNode;

	public static List<ModelItem> Execute(FlowNode current)
	{
		return new FindPreviewStepHelper(current).Traverse();
	}

	private FindPreviewStepHelper(FlowNode current)
	{
		currentNode = current;
		checker = IsLast;
	}

	private bool IsLast(ModelItem item)
	{
		if (item.Parent != null && item.Parent.ItemType == typeof(FlowStep))
		{
			FlowStep flowStep = item.Parent.GetCurrentValue() as FlowStep;
			if (flowStep.Next != null && flowStep.Next.Equals(currentNode))
			{
				return true;
			}
		}
		return false;
	}
}
