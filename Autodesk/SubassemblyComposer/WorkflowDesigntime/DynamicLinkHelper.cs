using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Autodesk.SubassemblyComposer.ActivityLibrary;
using Autodesk.SubassemblyComposer.WorkflowEngine;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal static class DynamicLinkHelper
{
	private static List<DynamicLinkItem> DynamicLinkItems = new List<DynamicLinkItem>();

	private static ItemType currentType = ItemType.None;

	private static string currentItemName;

	public static bool IsCancelled { get; set; }

	public static bool IsOptionSelected { get; set; }

	public static bool WillUpdate { get; set; }

	public static void CheckForReference(ModelProperty modelProperty, string nameToFind, Type expressionType = null)
	{
		if (!string.IsNullOrEmpty(nameToFind) && (!IsOptionSelected || WillUpdate) && modelProperty.Value != null)
		{
			Type type = modelProperty.Value.GetCurrentValue().GetType();
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			Type[] genericArguments = type.GetGenericArguments();
			if ((genericTypeDefinition == typeof(InArgument<>) || genericTypeDefinition == typeof(OutArgument<>)) && genericArguments.Length == 1)
			{
				CheckInOutModelProperty(modelProperty, nameToFind);
			}
			else if (genericTypeDefinition == typeof(List<>) && genericArguments.Length == 1 && (genericArguments[0].GetGenericTypeDefinition() == typeof(InArgument<>) || genericArguments[0].GetGenericTypeDefinition() == typeof(OutArgument<>)))
			{
				CheckInOutModelPropertyList(modelProperty, nameToFind, expressionType);
			}
			else if (genericTypeDefinition == typeof(VisualBasicValue<>))
			{
				CheckVisualBasicProperty(modelProperty, nameToFind);
			}
		}
	}

	private static void CheckInOutModelProperty(ModelProperty modelProperty, string nameToFind)
	{
		Type type = modelProperty.Value.GetCurrentValue().GetType();
		Type genericTypeDefinition = type.GetGenericTypeDefinition();
		Type type2 = type.GetGenericArguments()[0];
		dynamic val = Convert.ChangeType(modelProperty.Value.GetCurrentValue(), type);
		Type type3 = ((!(genericTypeDefinition == typeof(InArgument<>))) ? typeof(VisualBasicReference<>) : typeof(VisualBasicValue<>));
		if (modelProperty.Value.GetCurrentValue() == null)
		{
			return;
		}
		dynamic val2;
		try
		{
			val2 = Convert.ChangeType(val.Expression, type3.MakeGenericType(type2));
		}
		catch
		{
			return;
		}
		dynamic val3 = val2 == null;
		if (!(val3 ? true : false) && !((val3 | (val2.ExpressionText == null)) ? true : false))
		{
			string expression = val2.ExpressionText;
			if (compareExpression(ref expression, nameToFind))
			{
				DynamicLinkItem dynamicLinkItem = new DynamicLinkItem(modelProperty);
				dynamicLinkItem.expressionType = type3;
				dynamicLinkItem.genericType = genericTypeDefinition;
				dynamicLinkItem.argumentType = type2;
				dynamicLinkItem.expressionText = expression;
				dynamicLinkItem.propertyOption = PropertyOption.InOutModelProperty;
				DynamicLinkItems.Add(dynamicLinkItem);
			}
		}
	}

	private static void CheckInOutModelPropertyList(ModelProperty modelProperty, string nameToFind, Type expressionType)
	{
		Type type = modelProperty.Value.GetCurrentValue().GetType();
		Type type2 = type.GetGenericArguments()[0];
		Type type3 = type.GetGenericArguments()[0].GetGenericArguments()[0];
		dynamic val = Convert.ChangeType(modelProperty.Value.GetCurrentValue(), type);
		if (expressionType != null)
		{
			if (!((val != null) ? true : false))
			{
				return;
			}
			for (int i = 0; i < val.Count; i++)
			{
				object value = val[i];
				dynamic val3;
				try
				{
					dynamic val2 = Convert.ChangeType(value, type2);
					val3 = Convert.ChangeType(val2.Expression, expressionType.MakeGenericType(type3));
				}
				catch
				{
					continue;
				}
				if (val3.Value != null && string.Compare(val3.Value, nameToFind, true) == 0)
				{
					DynamicLinkItem dynamicLinkItem = new DynamicLinkItem(modelProperty);
					dynamicLinkItem.expressionType = expressionType;
					dynamicLinkItem.genericType = type2;
					dynamicLinkItem.argumentType = type3;
					dynamicLinkItem.propertyType = type;
					dynamicLinkItem.propertyOption = PropertyOption.InOutModelPropertyList;
					dynamicLinkItem.isCode = true;
					DynamicLinkItems.Add(dynamicLinkItem);
				}
			}
			return;
		}
		expressionType = ((!(type2.GetGenericTypeDefinition() == typeof(InArgument<>))) ? typeof(VisualBasicReference<>) : typeof(VisualBasicValue<>));
		if (!((val != null) ? true : false))
		{
			return;
		}
		for (int j = 0; j < val.Count; j++)
		{
			object value2 = val[j];
			dynamic val5;
			try
			{
				dynamic val4 = Convert.ChangeType(value2, type2);
				val5 = Convert.ChangeType(val4.Expression, expressionType.MakeGenericType(type3));
			}
			catch
			{
				continue;
			}
			if (val5.ExpressionText != null && string.Compare(val5.ExpressionText, nameToFind, true) == 0)
			{
				DynamicLinkItem dynamicLinkItem2 = new DynamicLinkItem(modelProperty);
				dynamicLinkItem2.expressionType = expressionType;
				dynamicLinkItem2.genericType = type2;
				dynamicLinkItem2.argumentType = type3;
				dynamicLinkItem2.propertyType = type;
				dynamicLinkItem2.propertyOption = PropertyOption.InOutModelPropertyList;
				DynamicLinkItems.Add(dynamicLinkItem2);
			}
		}
	}

	private static void CheckVisualBasicProperty(ModelProperty modelProperty, string nameToFind)
	{
		Type type = modelProperty.Value.GetCurrentValue().GetType();
		dynamic val = Convert.ChangeType(modelProperty.Value.GetCurrentValue(), type);
		dynamic val2 = val == null;
		if (!(val2 ? true : false) && !((val2 | (val.ExpressionText == null)) ? true : false))
		{
			string expression = val.ExpressionText;
			if (compareExpression(ref expression, nameToFind))
			{
				DynamicLinkItem dynamicLinkItem = new DynamicLinkItem(modelProperty);
				dynamicLinkItem.propertyType = type;
				dynamicLinkItem.expressionText = expression;
				dynamicLinkItem.propertyOption = PropertyOption.VisualBasicProperty;
				DynamicLinkItems.Add(dynamicLinkItem);
			}
		}
	}

	private static bool compareExpression(ref string expression, string textToReplace)
	{
		if (textToReplace == "Math")
		{
			return false;
		}
		if (!new Regex("(?<!([_.\"\\w])|(Math[.]))" + textToReplace + "(?![_.\"\\w])").IsMatch(expression))
		{
			return false;
		}
		return true;
	}

	private static void replaceExpression(ref string expression, string textToReplace, string textReplaceWith)
	{
		Regex regex = new Regex("(?<!([_.\"\\w])|(Math[.]))" + textToReplace + "(?![_.\"\\w])");
		expression = regex.Replace(expression, textReplaceWith);
	}

	private static void ModifyModelProperty(DynamicLinkItem item, string textToReplace, string textReplaceWith)
	{
		switch (item.propertyOption)
		{
		case PropertyOption.InOutModelProperty:
			ModifyInOutModelProperty(item, textToReplace, textReplaceWith);
			break;
		case PropertyOption.InOutModelPropertyList:
			ModifyInOutModelPropertyList(item, textToReplace, textReplaceWith);
			break;
		case PropertyOption.VisualBasicProperty:
			ModifyVisualBasicProperty(item, textToReplace, textReplaceWith);
			break;
		}
	}

	private static void ModifyInOutModelProperty(DynamicLinkItem item, string textToReplace, string textReplaceWith)
	{
		replaceExpression(ref item.expressionText, textToReplace, textReplaceWith);
		dynamic val = Activator.CreateInstance(item.expressionType.MakeGenericType(item.argumentType), item.expressionText);
		dynamic val2 = Activator.CreateInstance(item.genericType.MakeGenericType(item.argumentType), val);
		item.modelProperty.SetValue(val2);
	}

	private static void ModifyInOutModelPropertyList(DynamicLinkItem item, string textToReplace, string textReplaceWith)
	{
		dynamic val = Convert.ChangeType(item.modelProperty.Value.GetCurrentValue(), item.propertyType);
		if (val == null)
		{
			return;
		}
		if (item.isCode)
		{
			List<ModelItem> list = new List<ModelItem>();
			for (int i = 0; i < val.Count; i++)
			{
				object value = val[i];
				dynamic val3;
				try
				{
					dynamic val2 = Convert.ChangeType(value, item.genericType);
					val3 = Convert.ChangeType(val2.Expression, item.expressionType.MakeGenericType(item.argumentType));
				}
				catch
				{
					continue;
				}
				if (!((val3.Value != null && string.Compare(val3.Value, textToReplace, true) == 0) ? true : false))
				{
					continue;
				}
				if (textReplaceWith == "")
				{
					if (item.modelProperty.IsCollection && item.modelProperty.Collection.Count > i)
					{
						list.Add(item.modelProperty.Collection[i]);
					}
					continue;
				}
				object computedValue = Activator.CreateInstance(item.expressionType.MakeGenericType(item.argumentType), textReplaceWith);
				if (item.modelProperty.IsCollection && item.modelProperty.Collection.Count > i && item.modelProperty.Collection[i].Content != null)
				{
					item.modelProperty.Collection[i].Content.ComputedValue = computedValue;
				}
			}
			{
				foreach (ModelItem item2 in list)
				{
					item.modelProperty.Collection.Remove(item2);
				}
				return;
			}
		}
		for (int j = 0; j < val.Count; j++)
		{
			object value2 = val[j];
			dynamic val5;
			try
			{
				dynamic val4 = Convert.ChangeType(value2, item.genericType);
				val5 = Convert.ChangeType(val4.Expression, item.expressionType.MakeGenericType(item.argumentType));
			}
			catch
			{
				continue;
			}
			if (val5.ExpressionText != null && string.Compare(val5.ExpressionText, textToReplace, true) == 0)
			{
				dynamic val6 = Activator.CreateInstance(item.expressionType.MakeGenericType(item.argumentType), textReplaceWith);
				dynamic val7 = Activator.CreateInstance(item.genericType, val6);
				val[j] = val7;
				if (item.modelProperty.IsCollection && item.modelProperty.Collection.Count > j && item.modelProperty.Collection[j].Content != null)
				{
					item.modelProperty.Collection[j].Content.ComputedValue = (object)val6;
				}
			}
		}
		item.modelProperty.SetValue(val);
	}

	private static void ModifyVisualBasicProperty(DynamicLinkItem item, string textToReplace, string textReplaceWith)
	{
		replaceExpression(ref item.expressionText, textToReplace, textReplaceWith);
		dynamic val = Activator.CreateInstance(item.propertyType, item.expressionText);
		item.modelProperty.SetValue(val);
	}

	public static void BeginSession()
	{
		IsOptionSelected = false;
		WillUpdate = false;
		IsCancelled = false;
	}

	public static bool BeginFindCurrent(ItemType type, string name)
	{
		if (IsCancelled)
		{
			return false;
		}
		DynamicLinkItems.Clear();
		currentType = type;
		currentItemName = name;
		return true;
	}

	public static bool Proceed()
	{
		if (DynamicLinkItems.Count != 0 && !IsOptionSelected)
		{
			if (currentType != ItemType.None)
			{
				WillUpdate = ActivityLibraryAppCallbacks.InvokeTaskDlgCallback(currentType);
			}
			IsOptionSelected = true;
		}
		return !IsCancelled;
	}

	public static void EndAndUpdateForCurrent(string textToReplace, string textReplaceWith)
	{
		if (!WillUpdate)
		{
			return;
		}
		foreach (DynamicLinkItem dynamicLinkItem in DynamicLinkItems)
		{
			ModifyModelProperty(dynamicLinkItem, textToReplace, textReplaceWith);
		}
	}

	public static void EndSession()
	{
		currentType = ItemType.None;
		currentItemName = "";
	}
}
