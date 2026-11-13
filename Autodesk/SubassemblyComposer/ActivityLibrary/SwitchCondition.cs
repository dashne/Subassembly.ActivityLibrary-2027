using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Statements;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(SwitchConditionDesigner))]
[SwitchConditionAutoNumbering]
[ToolboxBitmap(typeof(SwitchCondition), "Resources.Icons.switch_toolbox.ico")]
public class SwitchCondition : ConditionBase<string>, ICustomTypeDescriptor, ICustomAccessProperty
{
	public const string c_CaseNumber = "CaseNumber";

	public const string c_Cases = "Cases";

	private Dictionary<string, InArgument<string>> _caseLabelArguments = new Dictionary<string, InArgument<string>>();

	private Dictionary<string, InArgument<bool>> _caseConditionArguments = new Dictionary<string, InArgument<bool>>();

	private int _nextKey;

	public InArgument<int> CaseNumber { get; set; }

	public Dictionary<string, InArgument<string>> CaseLabelArguments => _caseLabelArguments;

	public Dictionary<string, InArgument<bool>> CaseConditionArguments => _caseConditionArguments;

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = new List<ValidationError>();
		ModelItem modelItem = FindActivityOnIdHelper.Execute(activityId);
		if (modelItem != null && modelItem.Parent != null)
		{
			ModelProperty modelProperty = modelItem.Parent.Properties["Next"];
			if (modelProperty != null && modelProperty.Value != null && modelProperty.Value.ItemType == typeof(FlowSwitch<string>) && modelProperty.Value.GetCurrentValue() is FlowSwitch<string> flowSwitch)
			{
				foreach (KeyValuePair<string, FlowNode> @case in flowSwitch.Cases)
				{
					bool flag = false;
					foreach (KeyValuePair<string, InArgument<string>> caseLabelArgument in CaseLabelArguments)
					{
						if (@case.Key.Equals(caseLabelArgument.Value.Expression.ToString()))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						string message = string.Format(Resource.Message_Validate_Case_Not_Defined, @case.Key);
						list.Add(new ValidationError(message, isWarning: false, "Cases"));
					}
				}
			}
		}
		Collection<string> collection = new Collection<string>();
		foreach (KeyValuePair<string, InArgument<string>> caseLabelArgument2 in CaseLabelArguments)
		{
			string text = caseLabelArgument2.Value.Expression.ToString();
			if (!collection.Contains(text))
			{
				collection.Add(text);
				continue;
			}
			string message2 = string.Format(Resource.Message_Validate_CaseLabel_Duplicated, text);
			list.Add(new ValidationError(message2, isWarning: false, "Cases"));
			break;
		}
		return list;
	}

	public PropertyDescriptorCollection GetProperties()
	{
		PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
		foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeof(ConditionBase<string>)))
		{
			if (!property.Name.Equals("DisplayName") && !property.Name.Equals("ActivityId"))
			{
				propertyDescriptorCollection.Add(property);
			}
		}
		ActivityPropertyDescriptor value = new ActivityPropertyDescriptor(Resource.DisplayName, GetType(), typeof(string), BrowsableAttribute.Yes, ReadOnlyAttribute.No, new CustomCategoryAttribute(typeof(Resource), "Category_Misc", 1));
		ActivityPropertyDescriptor value2 = new ActivityPropertyDescriptor("ActivityId", GetType(), typeof(InArgument<int>), BrowsableAttribute.No, ReadOnlyAttribute.No);
		propertyDescriptorCollection.Add(value);
		propertyDescriptorCollection.Add(value2);
		List<int> list = new List<int>();
		foreach (string key2 in CaseLabelArguments.Keys)
		{
			Match match = Regex.Match(key2, "\\d+");
			list.Add(int.Parse(match.Value));
		}
		list.Sort();
		for (int i = 0; i < list.Count; i++)
		{
			string key = KeyNumber2String(list[i]);
			ActivityPropertyDescriptor value3 = new ActivityPropertyDescriptor(Key2LabelPropertyName(key), GetType(), typeof(InArgument<string>), BrowsableAttribute.Yes, ReadOnlyAttribute.No, new CustomCategoryAttribute(typeof(Resource), "Category_Cases", 2), new EditorAttribute(typeof(InlineSwitchCaseCtrl), typeof(PropertyValueEditor)), new CustomDisplayNameAttribute((i + 1).ToString(), i + 1));
			propertyDescriptorCollection.Add(value3);
			ActivityPropertyDescriptor value4 = new ActivityPropertyDescriptor(Key2ConditionPropertyName(key), GetType(), typeof(InArgument<bool>), BrowsableAttribute.No, ReadOnlyAttribute.No);
			propertyDescriptorCollection.Add(value4);
		}
		int index = list.Count + 1;
		ActivityPropertyDescriptor value5 = new ActivityPropertyDescriptor("CaseNumber", GetType(), typeof(InArgument<int>), BrowsableAttribute.Yes, ReadOnlyAttribute.No, new CustomCategoryAttribute(typeof(Resource), "Category_Cases", 2), new EditorAttribute(typeof(CustomInlineSwitchCaseNumberCtrl), typeof(PropertyValueEditor)), new CustomDisplayNameAttribute(typeof(Resource), "DisplayName_AddNewCase", index));
		propertyDescriptorCollection.Add(value5);
		return propertyDescriptorCollection;
	}

	protected override void CacheMetadata(NativeActivityMetadata metadata)
	{
		Collection<RuntimeArgument> collection = new Collection<RuntimeArgument>();
		foreach (KeyValuePair<string, InArgument<string>> caseLabelArgument in CaseLabelArguments)
		{
			RuntimeArgument runtimeArgument = new RuntimeArgument(Key2LabelPropertyName(caseLabelArgument.Key), typeof(string), ArgumentDirection.In, isRequired: true);
			metadata.Bind(caseLabelArgument.Value, runtimeArgument);
			collection.Add(runtimeArgument);
		}
		foreach (KeyValuePair<string, InArgument<bool>> caseConditionArgument in CaseConditionArguments)
		{
			RuntimeArgument runtimeArgument2 = new RuntimeArgument(Key2ConditionPropertyName(caseConditionArgument.Key), typeof(bool), ArgumentDirection.In, isRequired: true);
			metadata.Bind(caseConditionArgument.Value, runtimeArgument2);
			collection.Add(runtimeArgument2);
		}
		metadata.SetArgumentsCollection(collection);
		base.CacheMetadata(metadata);
		if (ActivityDesigntime.Instance == null || _nextKey != 0)
		{
			return;
		}
		foreach (KeyValuePair<string, InArgument<string>> caseLabelArgument2 in CaseLabelArguments)
		{
			int num = int.Parse(Regex.Match(caseLabelArgument2.Key, "\\d+").Value);
			if (_nextKey < num)
			{
				_nextKey = num;
			}
		}
	}

	protected override void SubExecute(NativeActivityContext context)
	{
		string value = "Default";
		foreach (KeyValuePair<string, InArgument<bool>> caseConditionArgument in CaseConditionArguments)
		{
			if (caseConditionArgument.Value.Get<bool>(context))
			{
				value = CaseLabelArguments[caseConditionArgument.Key].Get<string>(context);
				break;
			}
		}
		context.SetValue(Result, value);
	}

	public object GetPropertyValue(string propertyName)
	{
		object result = null;
		switch (propertyName)
		{
		case "DisplayName":
			result = DisplayName;
			break;
		case "ActivityId":
			result = ActivityId;
			break;
		case "CaseNumber":
			if (CaseNumber == null)
			{
				CaseNumber = new InArgument<int>
				{
					Expression = CaseLabelArguments.Count
				};
			}
			result = CaseNumber;
			break;
		default:
		{
			string key = PropertyName2Key(propertyName);
			if (CaseLabelArguments.ContainsKey(key) && IsLabelProperty(propertyName))
			{
				result = CaseLabelArguments[key];
			}
			else if (CaseConditionArguments.ContainsKey(key) && IsConditionProperty(propertyName))
			{
				result = CaseConditionArguments[key];
			}
			break;
		}
		}
		return result;
	}

	public void SetPropertyValue(string propertyName, object newValue)
	{
		switch (propertyName)
		{
		case "DisplayName":
			DisplayName = newValue as string;
			return;
		case "ActivityId":
			ActivityId = newValue as InArgument<int>;
			return;
		case "CaseNumber":
			CaseNumber = newValue as InArgument<int>;
			return;
		}
		string key = PropertyName2Key(propertyName);
		if (CaseLabelArguments.ContainsKey(key) && IsLabelProperty(propertyName))
		{
			_ = CaseLabelArguments[key];
			InArgument<string> inArgument = newValue as InArgument<string>;
			if (newValue != null && inArgument != null && !string.IsNullOrEmpty(inArgument.Expression.ToString()))
			{
				CaseLabelArguments[key] = inArgument;
			}
		}
		else if (CaseConditionArguments.ContainsKey(key) && IsConditionProperty(propertyName))
		{
			CaseConditionArguments[key] = newValue as InArgument<bool>;
		}
	}

	public void RefreshCaseItemArguments()
	{
		List<int> list = new List<int>();
		foreach (string key2 in CaseLabelArguments.Keys)
		{
			Match match = Regex.Match(key2, "\\d+");
			list.Add(int.Parse(match.Value));
		}
		list.Sort();
		for (int i = 0; i < list.Count; i++)
		{
			string key = KeyNumber2String(list[i]);
			if (CaseLabelArguments[key].Expression.ToString().IndexOf(Resource.String_Default_CaseLabel_Prefix, StringComparison.Ordinal) == 0)
			{
				Activity<string> expression = Resource.String_Default_CaseLabel_Prefix + (i + 1);
				CaseLabelArguments[key].Expression = expression;
			}
		}
	}

	public string GetNextCaseItemKey()
	{
		_nextKey++;
		return KeyNumber2String(_nextKey);
	}

	private string KeyNumber2String(int number)
	{
		return "Case" + number;
	}

	private bool IsLabelProperty(string propertyName)
	{
		return propertyName.Contains("_Label");
	}

	private bool IsConditionProperty(string propertyName)
	{
		return propertyName.Contains("_Condition");
	}

	private string Key2LabelPropertyName(string key)
	{
		return key + "_Label";
	}

	public string Key2ConditionPropertyName(string key)
	{
		return key + "_Condition";
	}

	private string PropertyName2Key(string propertyName)
	{
		return propertyName.Split('_')[0];
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

	public object GetPropertyOwner(PropertyDescriptor pd)
	{
		return this;
	}
}
