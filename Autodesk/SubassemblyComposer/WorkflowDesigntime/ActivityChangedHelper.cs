using System.Activities.Presentation.Model;
using System.Activities.Statements;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class ActivityChangedHelper
{
	protected ModelItem TargetModelItem { get; set; }

	public ActivityChangedHelper(ModelItem modelItem)
	{
		TargetModelItem = modelItem;
	}

	public virtual void Run()
	{
		Traverse(TargetModelItem);
	}

	protected void Traverse(ModelItem modelItem)
	{
		if (modelItem == null)
		{
			return;
		}
		if (modelItem.ItemType == typeof(Flowchart))
		{
			foreach (ModelItem item in modelItem.Properties["Nodes"].Collection)
			{
				Traverse(item);
			}
			return;
		}
		if (modelItem.ItemType == typeof(FlowStep))
		{
			ModelProperty modelProperty = modelItem.Properties["Action"];
			if (modelProperty != null)
			{
				Traverse(modelProperty.Value);
			}
			return;
		}
		if (modelItem.ItemType == typeof(Sequence))
		{
			ModelProperty modelProperty2 = modelItem.Properties["Activities"];
			if (!modelProperty2.IsCollection)
			{
				return;
			}
			{
				foreach (ModelItem item2 in modelProperty2.Collection)
				{
					Traverse(item2);
				}
				return;
			}
		}
		TraverseItem(modelItem);
		if (!(modelItem.ItemType == typeof(CreateOffset)) || (modelItem.GetCurrentValue() as CreateOffset).InCreate)
		{
			return;
		}
		ModelProperty modelProperty3 = modelItem.Properties["Activities"];
		if (!modelProperty3.IsCollection)
		{
			return;
		}
		foreach (ModelItem item3 in modelProperty3.Collection)
		{
			Traverse(item3);
		}
	}

	protected virtual void TraverseItem(ModelItem modelItem)
	{
	}
}
