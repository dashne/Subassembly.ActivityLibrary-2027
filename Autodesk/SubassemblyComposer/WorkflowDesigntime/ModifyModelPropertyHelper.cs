using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal static class ModifyModelPropertyHelper
{
	public static void ModifyModelProperty(ModelProperty modelProperty, string textToReplace, string textReplaceWith, Type expressionType = null)
	{
		if (string.IsNullOrEmpty(textToReplace))
		{
			return;
		}
		Type type = modelProperty.Value.GetCurrentValue().GetType();
		Type genericTypeDefinition = type.GetGenericTypeDefinition();
		Type[] genericArguments = type.GetGenericArguments();
		if ((genericTypeDefinition == typeof(InArgument<>) || genericTypeDefinition == typeof(OutArgument<>)) && genericArguments.Length == 1)
		{
			ModifyInOutModelProperty(modelProperty, textToReplace, textReplaceWith);
			return;
		}
		if (genericTypeDefinition == typeof(List<>) && genericArguments.Length == 1 && (genericArguments[0].GetGenericTypeDefinition() == typeof(InArgument<>) || genericArguments[0].GetGenericTypeDefinition() == typeof(OutArgument<>)))
		{
			ModifyInOutModelPropertyList(modelProperty, textToReplace, textReplaceWith, expressionType);
			return;
		}
		if (genericTypeDefinition == typeof(VisualBasicValue<>))
		{
			ModifyVisualBasicProperty(modelProperty, textToReplace, textReplaceWith);
			return;
		}
		throw new NotImplementedException();
	}

	private static void ModifyInOutModelProperty(ModelProperty modelProperty, string textToReplace, string textReplaceWith)
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
		dynamic val3 = val2 == null || val2.ExpressionText == null;
		if (!(val3 ? true : false) && !((val3 | (textReplaceWith == null)) ? true : false))
		{
			string expression = val2.ExpressionText;
			if (replaceExpression(ref expression, textToReplace, textReplaceWith))
			{
				dynamic val4 = Activator.CreateInstance(type3.MakeGenericType(type2), expression);
				dynamic val5 = Activator.CreateInstance(genericTypeDefinition.MakeGenericType(type2), val4);
				modelProperty.SetValue(val5);
			}
		}
	}

	private static void ModifyInOutModelPropertyList(ModelProperty modelProperty, string textToReplace, string textReplaceWith, Type expressionType)
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
			List<ModelItem> list = new List<ModelItem>();
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
				if (!((val3.Value != null && string.Compare(val3.Value, textToReplace, true) == 0) ? true : false))
				{
					continue;
				}
				if (textReplaceWith == "")
				{
					if (modelProperty.IsCollection && modelProperty.Collection.Count > i)
					{
						list.Add(modelProperty.Collection[i]);
					}
					continue;
				}
				object computedValue = Activator.CreateInstance(expressionType.MakeGenericType(type3), textReplaceWith);
				if (modelProperty.IsCollection && modelProperty.Collection.Count > i && modelProperty.Collection[i].Content != null)
				{
					modelProperty.Collection[i].Content.ComputedValue = computedValue;
				}
			}
			{
				foreach (ModelItem item in list)
				{
					modelProperty.Collection.Remove(item);
				}
				return;
			}
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
			if (val5.ExpressionText != null && string.Compare(val5.ExpressionText, textToReplace, true) == 0)
			{
				dynamic val6 = Activator.CreateInstance(expressionType.MakeGenericType(type3), textReplaceWith);
				dynamic val7 = Activator.CreateInstance(type2, val6);
				val[j] = val7;
				if (modelProperty.IsCollection && modelProperty.Collection.Count > j && modelProperty.Collection[j].Content != null)
				{
					modelProperty.Collection[j].Content.ComputedValue = (object)val6;
				}
			}
		}
		modelProperty.SetValue(val);
	}

	private static void ModifyVisualBasicProperty(ModelProperty modelProperty, string textToReplace, string textReplaceWith)
	{
		Type type = modelProperty.Value.GetCurrentValue().GetType();
		dynamic val = Convert.ChangeType(modelProperty.Value.GetCurrentValue(), type);
		dynamic val2 = val == null || val.ExpressionText == null;
		if (!(val2 ? true : false) && !((val2 | (textReplaceWith == null)) ? true : false))
		{
			string expression = val.ExpressionText;
			if (replaceExpression(ref expression, textToReplace, textReplaceWith))
			{
				dynamic val3 = Activator.CreateInstance(type, expression);
				modelProperty.SetValue(val3);
			}
		}
	}

	private static bool replaceExpression(ref string expression, string textToReplace, string textReplaceWith)
	{
		if (textToReplace == "Math")
		{
			return false;
		}
		Regex regex = new Regex("(?<!([_.\"\\w])|(Math[.]))" + textToReplace + "(?![_.\"\\w])");
		if (!regex.IsMatch(expression))
		{
			return false;
		}
		expression = regex.Replace(expression, textReplaceWith);
		return true;
	}
}
