using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class CorrectGeometryNameReferencesHelper : TraverseWorkflow
{
	private string OldName { get; set; }

	private string NewName { get; set; }

	public static void Execute(string oldName, string newName)
	{
		new CorrectGeometryNameReferencesHelper(oldName, newName).Run();
	}

	public static void Execute(string oldName, string newName, ModelItem modelItem)
	{
		new CorrectGeometryNameReferencesHelper(oldName, newName).Run(modelItem);
	}

	private CorrectGeometryNameReferencesHelper(string oldValue, string newValue)
	{
		OldName = oldValue;
		NewName = newValue;
	}

	public void Run()
	{
		if (!OldName.Equals(NewName) && !string.IsNullOrEmpty(OldName))
		{
			Traverse();
		}
	}

	public void Run(ModelItem item)
	{
		if (!OldName.Equals(NewName))
		{
			Traverse(item, new List<ModelItem>());
		}
	}

	private void ModifyModelProperty(ModelProperty modelProperty)
	{
		if (!(modelProperty != null) || modelProperty.Value == null || !(modelProperty.Value.GetCurrentValue() is InArgument<string> { Expression: not null } inArgument))
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = inArgument.Expression.ToString().Split(',');
		foreach (string text in array)
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(',');
				}
				if (text.Equals(OldName))
				{
					stringBuilder.Append(NewName);
				}
				else
				{
					stringBuilder.Append(text);
				}
			}
		}
		modelProperty.SetValue(new InArgument<string>
		{
			Expression = stringBuilder.ToString()
		});
	}

	protected override void TraverseItem(ModelItem modelItem, List<ModelItem> modelItems)
	{
		foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(modelItem.GetCurrentValue()))
		{
			if (property.Attributes[typeof(CanReferGeometryNameAttribute)] is CanReferGeometryNameAttribute)
			{
				ModelProperty modelProperty = modelItem.Properties[property.Name];
				ModifyModelProperty(modelProperty);
			}
		}
	}
}
