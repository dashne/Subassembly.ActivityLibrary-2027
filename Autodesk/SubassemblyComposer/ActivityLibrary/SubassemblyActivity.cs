using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

public abstract class SubassemblyActivity : CustomActivity, ICustomTypeDescriptor
{
	public const int InValidActivityId = 0;

	[RequiredArgument]
	[Browsable(false)]
	public InOutArgument<Geometry> Geometry
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((SubassemblyActivity p) => p.Geometry)) as InOutArgument<Geometry>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((SubassemblyActivity p) => p.Geometry), value);
		}
	}

	[Browsable(false)]
	public virtual bool IsCreatingAux => false;

	protected void OnGeometryNameChanged(string propertyName, InArgument<string> oldName, InArgument<string> newName)
	{
		if (ActivityDesigntime.Instance != null && !ActivityDesigntime.Instance.InPasting && oldName != null && oldName != newName && ActivityId != null && !ActivityDesigntime.Instance.Context.Services.GetService<UndoEngine>().IsUndoRedoInProgress)
		{
			ModelItem modelItem = FindActivityOnIdHelper.Execute((ActivityId.Expression as Literal<int>).Value);
			string newName2 = ((newName != null) ? newName.Expression.ToString() : string.Empty);
			GeometryNameChangedHelper.Execute(ActivityDesigntime.Instance.CodePathTree, modelItem, propertyName, oldName.Expression.ToString(), newName2);
		}
	}

	public SubassemblyActivity()
	{
		PropertyCollection_Set(this.PropertyName((SubassemblyActivity p) => p.IsCreatingAux), IsCreatingAux);
	}

	protected string[] GetCodes(CodeActivityContext context, InArgument<string> codesProp)
	{
		if (codesProp == null || codesProp.Expression == null)
		{
			return new string[0];
		}
		List<string> list = new List<string>();
		string value = context.GetValue(codesProp);
		if (value != null)
		{
			string[] array = value.Split(',');
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text.Trim()))
				{
					list.Add(text.Trim());
				}
			}
		}
		return list.ToArray();
	}

	protected string[] GetCodes(CodeActivityContext context, List<InArgument<string>> codesProperty)
	{
		if (codesProperty == null || codesProperty.Count == 0)
		{
			return new string[0];
		}
		List<string> list = new List<string>();
		foreach (InArgument<string> item in codesProperty)
		{
			list.AddRange(GetCodes(context, item));
		}
		return list.ToArray();
	}

	protected void BindCodeArguments(CodeActivityMetadata metadata, List<InArgument<string>> codesProperty, string propName)
	{
		if (codesProperty == null || codesProperty.Count == 0)
		{
			return;
		}
		int num = 1;
		foreach (InArgument<string> item in codesProperty)
		{
			RuntimeArgument argument = new RuntimeArgument(propName + num, typeof(string), ArgumentDirection.In, isRequired: true);
			metadata.Bind(item, argument);
			metadata.AddArgument(argument);
			num++;
		}
	}

	protected void ValidateCodeArguments(CodeActivityMetadata metadata, List<InArgument<string>> codesProperty)
	{
		if (codesProperty == null || codesProperty.Count == 0 || ActivityDesigntime.Instance == null)
		{
			return;
		}
		ActivityDesigntime.Instance.GetInputParameterNames<string>();
		foreach (InArgument<string> item in codesProperty)
		{
			if (item != null && item.Expression != null && item.Expression is VisualBasicValue<string>)
			{
				string expressionText = (item.Expression as VisualBasicValue<string>).ExpressionText;
				Type parameterType = ActivityDesigntime.Instance.GetParameterType(expressionText);
				Type variableType = ActivityDesigntime.Instance.GetVariableType(expressionText);
				if (parameterType != typeof(InArgument<string>) && parameterType != typeof(InOutArgument<string>) && variableType != typeof(string))
				{
					metadata.AddValidationError(string.Format(Resource.Message_Validate_CodeNameNotArgument, expressionText));
				}
			}
		}
	}

	public AttributeCollection GetAttributes()
	{
		return TypeDescriptor.GetAttributes(this, noCustomTypeDesc: true);
	}

	public string GetClassName()
	{
		return TypeDescriptor.GetClassName(this, noCustomTypeDesc: true);
	}

	public string GetComponentName()
	{
		return TypeDescriptor.GetComponentName(this, noCustomTypeDesc: true);
	}

	public TypeConverter GetConverter()
	{
		return TypeDescriptor.GetConverter(this, noCustomTypeDesc: true);
	}

	public EventDescriptor GetDefaultEvent()
	{
		return TypeDescriptor.GetDefaultEvent(this, noCustomTypeDesc: true);
	}

	public PropertyDescriptor GetDefaultProperty()
	{
		return TypeDescriptor.GetDefaultProperty(this, noCustomTypeDesc: true);
	}

	public object GetEditor(Type editorBaseType)
	{
		return TypeDescriptor.GetEditor(this, editorBaseType, noCustomTypeDesc: true);
	}

	public EventDescriptorCollection GetEvents(Attribute[] attributes)
	{
		return TypeDescriptor.GetEvents(this, attributes, noCustomTypeDesc: true);
	}

	public EventDescriptorCollection GetEvents()
	{
		return TypeDescriptor.GetEvents(this, noCustomTypeDesc: true);
	}

	public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
	{
		return TypeDescriptor.GetProperties(this, attributes, noCustomTypeDesc: true);
	}

	public virtual PropertyDescriptorCollection GetProperties()
	{
		return TypeDescriptor.GetProperties(this, noCustomTypeDesc: true);
	}

	public object GetPropertyOwner(PropertyDescriptor pd)
	{
		return this;
	}
}
