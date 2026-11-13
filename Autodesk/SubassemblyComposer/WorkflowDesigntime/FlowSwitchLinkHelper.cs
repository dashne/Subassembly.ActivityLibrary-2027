using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class FlowSwitchLinkHelper
{
	public static bool Execute(ModelItem modelItem)
	{
		return new FlowSwitchLinkHelper().OnAddModelItemKeyValuePair(modelItem);
	}

	private bool OnAddModelItemKeyValuePair(ModelItem modelItem)
	{
		if (modelItem.ItemType.FullName.Contains("System.Activities.Presentation.ModelItemKeyValuePair`2"))
		{
			ModelProperty modelProperty = modelItem.Properties["Key"];
			if (modelProperty.Value != null && modelItem.Parent != null && modelItem.Parent.Parent != null && modelItem.Parent.Parent.Parent != null)
			{
				FlowSwitch<string> flowSwitch = modelItem.Parent.Parent.Parent.GetCurrentValue() as FlowSwitch<string>;
				Collection<string> collection = AllCases(modelItem.Parent.Parent.Parent);
				string item = modelProperty.Value.GetCurrentValue() as string;
				if (!collection.Contains(item))
				{
					foreach (KeyValuePair<string, FlowNode> @case in flowSwitch.Cases)
					{
						collection.Remove(@case.Key);
					}
					if (collection.Count > 0)
					{
						modelProperty.SetValue(collection[0]);
					}
					return true;
				}
			}
		}
		return false;
	}

	private Collection<string> AllCases(ModelItem flowSwicth)
	{
		Collection<string> collection = new Collection<string>();
		ConditionBase<string> conditionBase = FindSwitchConditionHelper.Execute(flowSwicth);
		if (conditionBase != null && conditionBase is SwitchCondition)
		{
			foreach (KeyValuePair<string, InArgument<string>> caseLabelArgument in (conditionBase as SwitchCondition).CaseLabelArguments)
			{
				string text = caseLabelArgument.Value.Expression.ToString();
				if (!string.IsNullOrEmpty(text))
				{
					collection.Add(text);
				}
			}
		}
		return collection;
	}
}
