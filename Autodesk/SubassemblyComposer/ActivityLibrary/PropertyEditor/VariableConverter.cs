using System;
using System.Activities;
using System.Globalization;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class VariableConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return OutArgument2String(value);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return String2OutArgument(value as string);
	}

	public static string OutArgument2String(object argument)
	{
		if (argument != null)
		{
			if (argument is OutArgument<double>)
			{
				return GetOutArgumentText<double>(argument as OutArgument);
			}
			if (argument is OutArgument<int>)
			{
				return GetOutArgumentText<int>(argument as OutArgument);
			}
			if (argument is OutArgument<string>)
			{
				return GetOutArgumentText<string>(argument as OutArgument);
			}
			if (argument is OutArgument<bool>)
			{
				return GetOutArgumentText<bool>(argument as OutArgument);
			}
			if (argument is OutArgument<Grade>)
			{
				return GetOutArgumentText<Grade>(argument as OutArgument);
			}
			if (argument is OutArgument<Slope>)
			{
				return GetOutArgumentText<Slope>(argument as OutArgument);
			}
			if (argument is OutArgument<EnumType>)
			{
				return GetOutArgumentText<EnumType>(argument as OutArgument);
			}
		}
		return null;
	}

	private static string GetOutArgumentText<T>(OutArgument outArg)
	{
		string result = string.Empty;
		if (outArg == null)
		{
			return result;
		}
		if (outArg.Expression is VisualBasicReference<T> visualBasicReference)
		{
			result = visualBasicReference.ExpressionText;
		}
		return result;
	}

	public static OutArgument String2OutArgument(string variableName)
	{
		if (!string.IsNullOrEmpty(variableName))
		{
			Type variableType = ActivityDesigntime.Instance.GetVariableType(variableName);
			if (variableType == typeof(double))
			{
				return new OutArgument<double>(new VisualBasicReference<double>
				{
					ExpressionText = variableName
				});
			}
			if (variableType == typeof(int))
			{
				return new OutArgument<int>(new VisualBasicReference<int>
				{
					ExpressionText = variableName
				});
			}
			if (variableType == typeof(string))
			{
				return new OutArgument<string>(new VisualBasicReference<string>
				{
					ExpressionText = variableName
				});
			}
			if (variableType == typeof(bool))
			{
				return new OutArgument<bool>(new VisualBasicReference<bool>
				{
					ExpressionText = variableName
				});
			}
			if (variableType == typeof(Grade))
			{
				return new OutArgument<Grade>(new VisualBasicReference<Grade>
				{
					ExpressionText = variableName
				});
			}
			if (variableType == typeof(Slope))
			{
				return new OutArgument<Slope>(new VisualBasicReference<Slope>
				{
					ExpressionText = variableName
				});
			}
			if (variableType == typeof(EnumType))
			{
				return new OutArgument<EnumType>(new VisualBasicReference<EnumType>
				{
					ExpressionText = variableName
				});
			}
		}
		return null;
	}
}
