using System;
using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(SetVariableDesigner))]
[SetVariableValueAutoNumbering]
[ToolboxBitmap(typeof(SetVariableValue), "Resources.Icons.set_variable_toolbox.ico")]
public class SetVariableValue : CustomActivity
{
	[CustomCategory(typeof(Resource), "Category_General", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_VariableName", 1)]
	[RequiredArgument]
	[SetVariableValueDepend]
	[Editor(typeof(RootFlowChartVariablesCombobox), typeof(PropertyValueEditor))]
	public OutArgument VariableName { get; set; }

	[CustomCategory(typeof(Resource), "Category_General", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_VariableValue", 2)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	[SetVariableValueDepend]
	public InArgument VariableValue { get; set; }

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = new List<ValidationError>();
		if (VariableValue == null)
		{
			return list;
		}
		ObservableCollection<string> allValidPointNames = ValidGeometryNamesHelper.GetAllValidPointNames(ActivityDesigntime.Instance.CodePathTree, activityId, includeOrigin: true);
		ObservableCollection<string> linkNames = ValidGeometryNamesHelper.GetLinkNames(ActivityDesigntime.Instance.CodePathTree, activityId);
		Collection<string> collection = new Collection<string>();
		if (VariableValue.ArgumentType == typeof(double))
		{
			collection = this.ValidateGeometryNameInExpression(VariableValue as InArgument<double>, allValidPointNames, linkNames);
		}
		else if (VariableValue.ArgumentType == typeof(int))
		{
			collection = this.ValidateGeometryNameInExpression(VariableValue as InArgument<int>, allValidPointNames, linkNames);
		}
		string text = string.Empty;
		foreach (string item in collection)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += ",";
			}
			text += item;
		}
		if (!string.IsNullOrEmpty(text))
		{
			string message = string.Format(Resource.Message_Validate_GeometryName, Resource.DisplayName_DeltaXAndDeltaY_DeltaX, text);
			list.Add(new ValidationError(message, isWarning: false, Resource.DisplayName_VariableValue));
		}
		return list;
	}

	protected override void LayoutExecute(CodeActivityContext context)
	{
		InternalExecute(context);
	}

	protected override void RoadwayExecute(CodeActivityContext context)
	{
		InternalExecute(context);
	}

	private void InternalExecute(CodeActivityContext context)
	{
		object obj = VariableValue.Get(context);
		if (obj is ITypeInfo)
		{
			obj = (double)(obj as BaseTypeInfo);
		}
		VariableName.Set(context, obj);
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
		Collection<RuntimeArgument> collection = new Collection<RuntimeArgument>();
		Type argumentType = typeof(double);
		if (VariableValue != null)
		{
			argumentType = VariableValue.ArgumentType;
		}
		RuntimeArgument runtimeArgument = new RuntimeArgument("Value", argumentType, ArgumentDirection.In, isRequired: true);
		metadata.Bind(VariableValue, runtimeArgument);
		Type argumentType2 = typeof(double);
		if (VariableName != null)
		{
			argumentType2 = VariableName.ArgumentType;
		}
		RuntimeArgument runtimeArgument2 = new RuntimeArgument("VaribaleName", argumentType2, ArgumentDirection.Out, isRequired: true);
		metadata.Bind(VariableName, runtimeArgument2);
		Type argumentType3 = typeof(double);
		if (SubassemblyRunMode != null)
		{
			argumentType3 = SubassemblyRunMode.ArgumentType;
		}
		RuntimeArgument runtimeArgument3 = new RuntimeArgument("SubassemblyRunMode", argumentType3, ArgumentDirection.InOut, isRequired: true);
		metadata.Bind(SubassemblyRunMode, runtimeArgument3);
		Type argumentType4 = typeof(double);
		if (SubassemblyErrorCenter != null)
		{
			argumentType4 = SubassemblyErrorCenter.ArgumentType;
		}
		RuntimeArgument runtimeArgument4 = new RuntimeArgument("SubassemblyErrorCenter", argumentType4, ArgumentDirection.InOut, isRequired: true);
		metadata.Bind(SubassemblyErrorCenter, runtimeArgument4);
		collection.Add(runtimeArgument);
		collection.Add(runtimeArgument2);
		collection.Add(runtimeArgument3);
		collection.Add(runtimeArgument4);
		metadata.SetArgumentsCollection(collection);
		if (VariableValue != null && VariableName != null && !TypeHelper.AreTypesCompatible(VariableValue.ArgumentType, VariableName.ArgumentType))
		{
			metadata.AddValidationError(new ValidationError(Resource.Message_Validate_VariableType_Error, isWarning: false, "VariableValue"));
		}
	}
}
