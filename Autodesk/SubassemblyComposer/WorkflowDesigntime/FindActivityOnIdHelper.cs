using System.Activities.Presentation.Model;
using System.Collections.Generic;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class FindActivityOnIdHelper : TraverseWorkflow
{
	private int ActivityId { get; set; }

	public static bool IsPastedActivity(int id)
	{
		return new FindActivityOnIdHelper(id).Traverse().Count > 1;
	}

	public static ModelItem Execute(int id)
	{
		ModelItem selectedModelItem = ActivityDesigntime.Instance.Selection.SelectedModelItem;
		if (id == ActivityHelpers.GetActivityId(selectedModelItem))
		{
			return selectedModelItem;
		}
		List<ModelItem> list = new FindActivityOnIdHelper(id).Traverse();
		if (list.Count != 1)
		{
			return null;
		}
		return list[0];
	}

	public FindActivityOnIdHelper(int id)
	{
		ActivityId = id;
	}

	protected override void TraverseItem(ModelItem modelItem, List<ModelItem> modelItems)
	{
		if ((modelItem.ItemType.IsSubclassOf(typeof(SubassemblyActivity)) || modelItem.ItemType.IsSubclassOf(typeof(CustomCondition)) || modelItem.ItemType.IsSubclassOf(typeof(ConditionBase<string>)) || modelItem.ItemType.IsSubclassOf(typeof(CustomActivity))) && ActivityId == ActivityHelpers.GetActivityId(modelItem))
		{
			modelItems.Add(modelItem);
		}
	}
}
