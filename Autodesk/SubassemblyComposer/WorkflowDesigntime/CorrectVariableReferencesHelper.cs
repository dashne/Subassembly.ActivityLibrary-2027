using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class CorrectVariableReferencesHelper : TraverseWorkflow
{
	private static bool doFind;

	private static List<ModelItem> modelItems;

	private string OldVariableName { get; set; }

	private string NewVariableName { get; set; }

	private string VariableTypeName { get; set; }

	public static void Execute(string newName, string oldName, string typeName)
	{
		new CorrectVariableReferencesHelper(newName, oldName, typeName).Run();
	}

	public static void ExecuteFind(string newName, string oldName, string typeName)
	{
		CorrectVariableReferencesHelper correctVariableReferencesHelper = new CorrectVariableReferencesHelper(newName, oldName, typeName);
		doFind = true;
		modelItems = correctVariableReferencesHelper.Find();
		doFind = false;
	}

	public static void UpdateSetVarValue(string NewVariableName, string OldVariableName, string VariableTypeName)
	{
		if (modelItems == null || !DynamicLinkHelper.WillUpdate)
		{
			return;
		}
		foreach (ModelItem modelItem in modelItems)
		{
			ModelProperty modelProperty = modelItem.Properties["VariableName"];
			if (modelProperty != null)
			{
				OutArgument value = ParameterTypeRegister.CreateDefaultParameter("dummy", VariableTypeName, ParameterDirection.Output, NewVariableName).ArgumentValue as OutArgument;
				modelProperty.SetValue(value);
			}
			modelProperty = modelItem.Properties["VariableValue"];
			ModifyModelPropertyHelper.ModifyModelProperty(modelProperty, OldVariableName, NewVariableName);
		}
	}

	private CorrectVariableReferencesHelper(string newName, string oldName, string typeName)
	{
		OldVariableName = oldName;
		NewVariableName = newName;
		VariableTypeName = typeName;
	}

	private void Run()
	{
		foreach (ModelItem item in Traverse())
		{
			ModelProperty modelProperty = item.Properties["VariableName"];
			if (modelProperty != null)
			{
				OutArgument value = ParameterTypeRegister.CreateDefaultParameter("dummy", VariableTypeName, ParameterDirection.Output, NewVariableName).ArgumentValue as OutArgument;
				modelProperty.SetValue(value);
			}
			modelProperty = item.Properties["VariableValue"];
			ModifyModelPropertyHelper.ModifyModelProperty(modelProperty, OldVariableName, NewVariableName);
		}
	}

	private List<ModelItem> Find()
	{
		return Traverse();
	}

	public static bool Proceed()
	{
		if (modelItems != null && modelItems.Count > 0 && !DynamicLinkHelper.IsOptionSelected)
		{
			DynamicLinkHelper.WillUpdate = ActivityLibraryAppCallbacks.InvokeTaskDlgCallback(ItemType.Variable);
		}
		return !DynamicLinkHelper.IsCancelled;
	}

	protected override void TraverseItem(ModelItem modelItem, List<ModelItem> modelItems)
	{
		if (modelItem.ItemType == typeof(SetVariableValue) && modelItem.GetCurrentValue() is SetVariableValue setVariableValue)
		{
			string value = VariableConverter.OutArgument2String(setVariableValue.VariableName);
			if (OldVariableName.Equals(value))
			{
				modelItems.Add(modelItem);
				return;
			}
		}
		if (modelItem.ItemType == typeof(FlowDecision))
		{
			ModelProperty modelProperty = modelItem.Properties["Condition"];
			if (doFind)
			{
				DynamicLinkHelper.CheckForReference(modelProperty, OldVariableName);
			}
			else
			{
				ModifyModelPropertyHelper.ModifyModelProperty(modelProperty, OldVariableName, NewVariableName);
			}
			return;
		}
		if (modelItem.ItemType == typeof(FlowSwitch<string>))
		{
			ModelProperty modelProperty2 = modelItem.Properties["Expression"];
			if (doFind)
			{
				DynamicLinkHelper.CheckForReference(modelProperty2, OldVariableName);
			}
			else
			{
				ModifyModelPropertyHelper.ModifyModelProperty(modelProperty2, OldVariableName, NewVariableName);
			}
			return;
		}
		foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(modelItem.GetCurrentValue()))
		{
			if (!(property.Attributes[typeof(CanReferVariableNameAttribute)] is CanReferVariableNameAttribute))
			{
				continue;
			}
			ModelProperty modelProperty3 = modelItem.Properties[property.Name];
			if (modelProperty3 != null && modelProperty3.Value != null)
			{
				if (doFind)
				{
					DynamicLinkHelper.CheckForReference(modelProperty3, OldVariableName);
				}
				else
				{
					ModifyModelPropertyHelper.ModifyModelProperty(modelProperty3, OldVariableName, NewVariableName);
				}
			}
		}
	}
}
