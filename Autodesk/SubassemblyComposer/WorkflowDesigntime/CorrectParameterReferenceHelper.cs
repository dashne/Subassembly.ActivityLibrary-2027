using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class CorrectParameterReferenceHelper : TraverseWorkflow
{
	private string OldParameterName { get; set; }

	private string NewParameterName { get; set; }

	public static void Execute(string oldValue, string newValue)
	{
		new CorrectParameterReferenceHelper(oldValue, newValue).Run();
	}

	public CorrectParameterReferenceHelper(string oldValue, string newValue)
	{
		OldParameterName = oldValue;
		NewParameterName = newValue;
	}

	public void Run()
	{
		if (!string.IsNullOrEmpty(OldParameterName))
		{
			Traverse();
		}
	}

	protected override void TraverseItem(ModelItem modelItem, List<ModelItem> modelItems)
	{
		if (modelItem.ItemType == typeof(FlowDecision))
		{
			DynamicLinkHelper.CheckForReference(modelItem.Properties["Condition"], OldParameterName);
			return;
		}
		if (modelItem.ItemType == typeof(FlowSwitch<string>))
		{
			DynamicLinkHelper.CheckForReference(modelItem.Properties["Expression"], OldParameterName);
			return;
		}
		foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(modelItem.GetCurrentValue()))
		{
			if (property.Attributes[typeof(CanReferParameterNameAttribute)] is CanReferParameterNameAttribute)
			{
				ModelProperty modelProperty = modelItem.Properties[property.Name];
				if (modelProperty != null && modelProperty.Value != null)
				{
					DynamicLinkHelper.CheckForReference(modelProperty, OldParameterName);
				}
			}
		}
	}
}
