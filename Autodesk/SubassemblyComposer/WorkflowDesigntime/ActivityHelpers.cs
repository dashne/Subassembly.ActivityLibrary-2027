using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class ActivityHelpers
{
	public static int GetActivityId(ModelItem modelItem)
	{
		if (modelItem != null)
		{
			object property = GetProperty(modelItem, "ActivityId");
			if (property != null)
			{
				return GetActivityId(property as InArgument<int>);
			}
		}
		return 0;
	}

	public static int GetActivityId(InArgument<int> argument)
	{
		if (argument != null && argument.Expression is Literal<int> literal)
		{
			return literal.Value;
		}
		return 0;
	}

	public static string GetStringPropertyValue(ModelItem modelItem, string propertyName)
	{
		if (modelItem != null)
		{
			InArgument<string> inArgument = null;
			ModelProperty modelProperty = null;
			object property = GetProperty(modelItem, propertyName);
			if (property != null)
			{
				if (!(property is Argument))
				{
					return property.ToString();
				}
				inArgument = property as InArgument<string>;
			}
			else
			{
				modelProperty = modelItem.Properties[propertyName];
				if (modelProperty != null && modelProperty.Value != null)
				{
					inArgument = modelProperty.Value.GetCurrentValue() as InArgument<string>;
				}
			}
			if (inArgument != null)
			{
				if (inArgument.Expression is Literal<string> literal)
				{
					return literal.Value;
				}
			}
			else if (modelProperty != null && modelProperty.Value != null && modelProperty.Value.GetCurrentValue() is string result)
			{
				return result;
			}
		}
		return null;
	}

	public static bool GetBooleanPropertyValue(ModelItem modelItem, string propertyName)
	{
		if (modelItem != null)
		{
			InArgument<bool> inArgument = null;
			ModelProperty modelProperty = null;
			object property = GetProperty(modelItem, propertyName);
			if (property != null)
			{
				inArgument = property as InArgument<bool>;
			}
			else
			{
				modelProperty = modelItem.Properties[propertyName];
				if (modelProperty != null && modelProperty.Value != null)
				{
					inArgument = modelProperty.Value.GetCurrentValue() as InArgument<bool>;
				}
			}
			if (inArgument != null)
			{
				Activity<bool> expression = inArgument.Expression;
				if (expression != null)
				{
					return bool.Parse(expression.ToString());
				}
			}
			else if (modelProperty != null && modelProperty.Value != null)
			{
				return (bool)modelProperty.Value.GetCurrentValue();
			}
		}
		throw new NotSupportedException();
	}

	public static string GetStringInArgumentValue(InArgument<string> argument)
	{
		if (argument != null && argument.Expression != null)
		{
			return argument.Expression.ToString();
		}
		return string.Empty;
	}

	public static void SetPropertyValue<T>(ModelItem modelItem, string propertyName, T value)
	{
		if (modelItem != null)
		{
			ModelProperty modelProperty = modelItem.Properties[propertyName];
			if (modelProperty != null)
			{
				modelProperty.SetValue(new InArgument<T>(value));
			}
		}
	}

	public static object GetProperty(ModelItem modelItem, string propertyName)
	{
		if (modelItem == null)
		{
			return null;
		}
		if (modelItem.GetCurrentValue() is CustomActivity customActivity)
		{
			return customActivity.PropertyCollection_Get(propertyName);
		}
		ModelProperty modelProperty = modelItem.Properties[propertyName];
		if (modelProperty == null || modelProperty.Value == null)
		{
			return null;
		}
		return modelProperty.Value.GetCurrentValue();
	}

	public static void SetProperty(ModelItem modelItem, string propertyName, object propertyValue)
	{
		if (modelItem != null)
		{
			ModelProperty modelProperty = modelItem.Properties[propertyName];
			if (modelProperty != null)
			{
				DynamicLinkHelper.BeginSession();
				modelProperty.SetValue(propertyValue);
				DynamicLinkHelper.EndSession();
			}
		}
	}
}
