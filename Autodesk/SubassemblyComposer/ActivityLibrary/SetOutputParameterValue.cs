using System;
using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(SetOutputParameterValueDesigner))]
[SetOutputParameterValueAutoNumbering]
[ToolboxBitmap(typeof(SetVariableValue), "Resources.Icons.set_output_param_toolbox.ico")]
public class SetOutputParameterValue : CustomActivity
{
	[CustomCategory(typeof(Resource), "Category_General", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_OutputParammeter", 1)]
	[RequiredArgument]
	[CanReferParameterName]
	[SetOutputDependence]
	[Editor(typeof(OutputParameterComboBox), typeof(PropertyValueEditor))]
	public OutArgument OutputParammeter { get; set; }

	[CustomCategory(typeof(Resource), "Category_General", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_OutputParammeter_Value", 2)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	[SetOutputDependence]
	public InArgument Value { get; set; }

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
		object obj = Value.Get(context);
		if (obj is ITypeInfo)
		{
			obj = (double)(obj as BaseTypeInfo);
		}
		if (OutputParammeter is OutArgument<Grade>)
		{
			obj = (Grade)Convert.ToDouble(obj);
		}
		else if (OutputParammeter is OutArgument<Slope>)
		{
			obj = (Slope)Convert.ToDouble(obj);
		}
		OutputParammeter.Set(context, obj);
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
		Collection<RuntimeArgument> collection = new Collection<RuntimeArgument>();
		Type argumentType = typeof(double);
		if (Value != null)
		{
			argumentType = Value.ArgumentType;
		}
		RuntimeArgument runtimeArgument = new RuntimeArgument("Value", argumentType, ArgumentDirection.In, isRequired: true);
		metadata.Bind(Value, runtimeArgument);
		Type argumentType2 = typeof(double);
		if (OutputParammeter != null)
		{
			argumentType2 = OutputParammeter.ArgumentType;
		}
		RuntimeArgument runtimeArgument2 = new RuntimeArgument("OutputParammeter", argumentType2, ArgumentDirection.Out, isRequired: true);
		metadata.Bind(OutputParammeter, runtimeArgument2);
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
		if (Value != null && OutputParammeter != null && !TypeHelper.AreTypesCompatible(Value.ArgumentType, OutputParammeter.ArgumentType))
		{
			metadata.AddValidationError(new ValidationError(Resource.Message_Validate_OutputParameter_Type_Error, isWarning: false, DisplayName));
		}
	}
}
