using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal abstract class SubassemblyActivityAutoNumberingAttribute : CustomActivityAutoNumberingAttribute
{
	public static char DisplayNameSeparator = '&';

	public override void SetActivityProperties(ModelItem modelItem)
	{
		if (IsNewActivity(modelItem))
		{
			base.SetActivityProperties(modelItem);
			SetSubassembly(modelItem);
			SubSetActivityProperties(modelItem, SetGeometryNameProperties(modelItem));
		}
	}

	protected virtual bool IsNewActivity(ModelItem modelItem)
	{
		return true;
	}

	private string SetGeometryNameProperties(ModelItem modelItem)
	{
		string text = string.Empty;
		foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(modelItem.GetCurrentValue()))
		{
			if (!(property.Attributes[typeof(GeometryAutoNumberingAttribute)] is GeometryAutoNumberingAttribute geometryAutoNumberingAttribute))
			{
				continue;
			}
			bool flag = true;
			if (!string.IsNullOrEmpty(geometryAutoNumberingAttribute.CheckPropertyName))
			{
				flag = ActivityHelpers.GetBooleanPropertyValue(modelItem, geometryAutoNumberingAttribute.CheckPropertyName);
			}
			if (flag)
			{
				ModelProperty modelProperty = modelItem.Properties[property.Name];
				string nextGeometryName = geometryAutoNumberingAttribute.GetNextGeometryName(geometryAutoNumberingAttribute.GetType());
				modelProperty.SetValue(new InArgument<string>
				{
					Expression = nextGeometryName
				});
				if (string.IsNullOrEmpty(text))
				{
					text = nextGeometryName;
					continue;
				}
				text += DisplayNameSeparator;
				text += nextGeometryName;
			}
		}
		return text;
	}

	protected abstract void SubSetActivityProperties(ModelItem modelItem, string displayName);

	public override void CorrectActivityProperties(ModelItem modelItem, ModelItem compositeModelItem)
	{
		if (CorrectActivityId(modelItem))
		{
			CorrectNameAndDisplayName(modelItem, compositeModelItem);
		}
	}

	private void CorrectNameAndDisplayName(ModelItem modelItem, ModelItem compositeModelItem)
	{
		Collection<Tuple<string, string>> collection = new Collection<Tuple<string, string>>();
		_ = string.Empty;
		foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(modelItem.GetCurrentValue()))
		{
			if (!(property.Attributes[typeof(GeometryAutoNumberingAttribute)] is GeometryAutoNumberingAttribute geometryAutoNumberingAttribute))
			{
				continue;
			}
			bool flag = true;
			if (!string.IsNullOrEmpty(geometryAutoNumberingAttribute.CheckPropertyName))
			{
				flag = ActivityHelpers.GetBooleanPropertyValue(modelItem, geometryAutoNumberingAttribute.CheckPropertyName);
			}
			if (flag)
			{
				string stringPropertyValue = ActivityHelpers.GetStringPropertyValue(modelItem, property.Name);
				if (!string.IsNullOrEmpty(stringPropertyValue))
				{
					ModelProperty modelProperty = modelItem.Properties[property.Name];
					string nextGeometryName = geometryAutoNumberingAttribute.GetNextGeometryName(geometryAutoNumberingAttribute.GetType());
					modelProperty.SetValue(new InArgument<string>
					{
						Expression = nextGeometryName
					});
					collection.Add(Tuple.Create(stringPropertyValue, nextGeometryName));
				}
			}
		}
		foreach (Tuple<string, string> item in collection)
		{
			CorrectGeometryNameReferencesHelper.Execute(item.Item1, item.Item2, compositeModelItem);
		}
		string stringPropertyValue2 = ActivityHelpers.GetStringPropertyValue(modelItem, "DisplayName");
		if (stringPropertyValue2 == null)
		{
			return;
		}
		string text = string.Empty;
		string[] array = stringPropertyValue2.Split(DisplayNameSeparator);
		foreach (string text2 in array)
		{
			string text3 = text2;
			foreach (Tuple<string, string> item2 in collection)
			{
				if (text2.StartsWith(item2.Item1))
				{
					text3 = text2.Replace(item2.Item1, item2.Item2);
					break;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				text += DisplayNameSeparator;
				text += text3;
			}
			else
			{
				text = text3;
			}
		}
		modelItem.Properties["DisplayName"].SetValue(text);
	}
}
