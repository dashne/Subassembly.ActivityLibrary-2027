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

[Designer(typeof(DefineVariableDesigner))]
[InternalVariableDefineAutoNumbering]
[ToolboxBitmap(typeof(InternalVariableDefine), "Resources.Icons.define_variable_toolbox.ico")]
public class InternalVariableDefine : CustomActivity
{
	private string _oldVariableName = string.Empty;

	private string _variableName = string.Empty;

	private string _oldVariableType = string.Empty;

	private string _variableType = string.Empty;

	[CustomCategory(typeof(Resource), "Category_Variable", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_VariableName", 1)]
	[RequiredArgument]
	[VariableDepend]
	public string VariableName
	{
		get
		{
			return _variableName;
		}
		set
		{
			if (string.Compare(_variableName, value, StringComparison.Ordinal) != 0)
			{
				_oldVariableName = _variableName;
				_variableName = value;
			}
		}
	}

	[Browsable(false)]
	public string OldVariableName
	{
		get
		{
			return _oldVariableName;
		}
		set
		{
			_oldVariableName = value;
		}
	}

	[Browsable(false)]
	public OutArgument Variable { get; set; }

	[CustomCategory(typeof(Resource), "Category_Variable", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_VariableType", 2)]
	[Editor(typeof(VariableTypeComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[VariableDepend]
	public string VariableType
	{
		get
		{
			return _variableType;
		}
		set
		{
			_oldVariableType = _variableType;
			_variableType = value;
		}
	}

	[Browsable(false)]
	public string OldVariableType => _oldVariableType;

	[CustomCategory(typeof(Resource), "Category_Variable", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DefaultValue", 3)]
	[RequiredArgument]
	[VariableDepend]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument DefaultValue { get; set; }

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = new List<ValidationError>();
		if (string.IsNullOrEmpty(VariableName))
		{
			list.Add(new ValidationError(Resource.Message_Validate_VariableName_Empty));
		}
		else if (!NamingRules.VerifyParamName(VariableName))
		{
			list.Add(new ValidationError(string.Format(Resource.Message_Validate_Variable_InvalidName, VariableName)));
		}
		if (string.IsNullOrEmpty(VariableType))
		{
			list.Add(new ValidationError(Resource.Message_Validate_VariableType_Undefined));
		}
		if (DefaultValue != null && !string.IsNullOrEmpty(VariableType))
		{
			Type dataTypeByDisplayName = ParameterTypeRegister.GetDataTypeByDisplayName(VariableType);
			if (!TypeHelper.AreTypesCompatible(DefaultValue.ArgumentType, dataTypeByDisplayName))
			{
				list.Add(new ValidationError(Resource.Message_Validate_DefaultValue_Type_Error, isWarning: false, "DefaultValue"));
			}
		}
		if (DefaultValue != null)
		{
			ObservableCollection<string> allValidPointNames = ValidGeometryNamesHelper.GetAllValidPointNames(ActivityDesigntime.Instance.CodePathTree, activityId, includeOrigin: true);
			ObservableCollection<string> linkNames = ValidGeometryNamesHelper.GetLinkNames(ActivityDesigntime.Instance.CodePathTree, activityId);
			Collection<string> collection = new Collection<string>();
			if (DefaultValue.ArgumentType == typeof(double))
			{
				collection = this.ValidateGeometryNameInExpression(DefaultValue as InArgument<double>, allValidPointNames, linkNames);
			}
			else if (DefaultValue.ArgumentType == typeof(int))
			{
				collection = this.ValidateGeometryNameInExpression(DefaultValue as InArgument<int>, allValidPointNames, linkNames);
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
				string message = string.Format(Resource.Message_Validate_GeometryName, Resource.DisplayName_DefaultValue, text);
				list.Add(new ValidationError(message, isWarning: false, Resource.DisplayName_VariableValue));
			}
		}
		else
		{
			list.Add(new ValidationError(Resource.Message_Validate_Variable_NoDefaultValue));
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
		if (DefaultValue != null && Variable != null)
		{
			object obj = DefaultValue.Get(context);
			if (obj is ITypeInfo)
			{
				obj = (double)(obj as BaseTypeInfo);
			}
			Variable.Set(context, obj);
		}
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
		if (DefaultValue != null)
		{
			Collection<RuntimeArgument> collection = new Collection<RuntimeArgument>();
			Type argumentType = typeof(double);
			if (Variable != null)
			{
				argumentType = Variable.ArgumentType;
			}
			RuntimeArgument runtimeArgument = new RuntimeArgument("Variable", argumentType, ArgumentDirection.Out, isRequired: false);
			metadata.Bind(Variable, runtimeArgument);
			Type argumentType2 = typeof(double);
			if (DefaultValue != null)
			{
				argumentType2 = DefaultValue.ArgumentType;
			}
			RuntimeArgument runtimeArgument2 = new RuntimeArgument("Value", argumentType2, ArgumentDirection.In, isRequired: true);
			metadata.Bind(DefaultValue, runtimeArgument2);
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
			collection.Add(runtimeArgument2);
			collection.Add(runtimeArgument);
			collection.Add(runtimeArgument3);
			collection.Add(runtimeArgument4);
			metadata.SetArgumentsCollection(collection);
		}
	}

	public void ClearVariableName()
	{
		_variableName = string.Empty;
	}
}
