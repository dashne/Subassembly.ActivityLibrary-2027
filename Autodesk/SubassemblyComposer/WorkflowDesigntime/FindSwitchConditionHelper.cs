using System.Activities.Presentation.Model;
using System.Collections.Generic;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class FindSwitchConditionHelper : TraverseWorkflow
{
	private ModelItem FlowSwitch { get; set; }

	public static ConditionBase<string> Execute(ModelItem flowSwicth)
	{
		List<ModelItem> list = new FindSwitchConditionHelper(flowSwicth).Traverse();
		if (list.Count == 1)
		{
			return list[0].GetCurrentValue() as ConditionBase<string>;
		}
		return null;
	}

	private FindSwitchConditionHelper(ModelItem flowSwicth)
	{
		FlowSwitch = flowSwicth;
	}

	protected override void TraverseItem(ModelItem modelItem, List<ModelItem> modelItems)
	{
		if (modelItem.ItemType.IsSubclassOf(typeof(ConditionBase<string>)) && modelItem.Parent.Properties["Next"].Value == FlowSwitch)
		{
			modelItems.Add(modelItem);
		}
	}
}
