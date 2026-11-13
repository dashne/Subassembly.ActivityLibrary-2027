using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class FindActivityHelper : TraverseWorkflow
{
	protected Predicate<ModelItem> checker;

	public static List<ModelItem> Execute(Predicate<ModelItem> pred)
	{
		return new FindActivityHelper(pred).Traverse();
	}

	protected FindActivityHelper(Predicate<ModelItem> pred)
	{
		checker = pred;
	}

	protected FindActivityHelper()
	{
	}

	protected override void TraverseItem(ModelItem modelItem, List<ModelItem> modelItems)
	{
		if (checker(modelItem))
		{
			modelItems.Add(modelItem);
		}
	}
}
