using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Statements;
using System.Collections.Generic;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class TraverseWorkflow
{
	public static List<ModelItem> Execute()
	{
		return new TraverseWorkflow().Traverse();
	}

	protected List<ModelItem> Traverse()
	{
		List<ModelItem> list = new List<ModelItem>();
		if (ActivityDesigntime.Instance != null && ActivityDesigntime.Instance.Context != null)
		{
			ModelProperty modelProperty = ActivityDesigntime.Instance.Context.Services.GetService<ModelService>().Root.Properties["Implementation"];
			if (modelProperty != null)
			{
				Traverse(modelProperty.Value, list);
			}
		}
		return list;
	}

	protected virtual void TraverseItem(ModelItem modelItem, List<ModelItem> modelItems)
	{
		if (modelItem.ItemType.IsSubclassOf(typeof(SubassemblyActivity)))
		{
			modelItems.Add(modelItem);
		}
	}

	protected void Traverse(ModelItem modelItem, List<ModelItem> modelItems)
	{
		if (modelItem.ItemType == typeof(Flowchart))
		{
			TraverseFlowchart(modelItem, modelItems);
			return;
		}
		if (modelItem.ItemType == typeof(FlowStep))
		{
			ModelProperty modelProperty = modelItem.Properties["Action"];
			Traverse(modelProperty.Value, modelItems);
			return;
		}
		if (modelItem.ItemType == typeof(Sequence))
		{
			TraverseSequence(modelItem, modelItems);
			return;
		}
		TraverseItem(modelItem, modelItems);
		if (!(modelItem.ItemType == typeof(CreateOffset)) || (modelItem.GetCurrentValue() as CreateOffset).InCreate)
		{
			return;
		}
		ModelProperty modelProperty2 = modelItem.Properties["Activities"];
		if (!modelProperty2.IsCollection)
		{
			return;
		}
		foreach (ModelItem item in modelProperty2.Collection)
		{
			TraverseItem(item, modelItems);
		}
	}

	private void TraverseFlowchart(ModelItem modelItem, List<ModelItem> modelItems)
	{
		foreach (ModelItem item in modelItem.Properties["Nodes"].Collection)
		{
			Traverse(item, modelItems);
		}
	}

	private void TraverseSequence(ModelItem modelItem, List<ModelItem> modelItems)
	{
		ModelProperty modelProperty = modelItem.Properties["Activities"];
		if (!modelProperty.IsCollection)
		{
			return;
		}
		foreach (ModelItem item in modelProperty.Collection)
		{
			Traverse(item, modelItems);
		}
	}
}
