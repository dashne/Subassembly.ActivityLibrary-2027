using System;
using System.Activities;
using System.Globalization;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class OutputParameterArgumentConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			if (value is OutArgument<double>)
			{
				return ((value as OutArgument).Expression as VisualBasicReference<double>).ExpressionText.ToString();
			}
			if (value is OutArgument<int>)
			{
				return ((value as OutArgument).Expression as VisualBasicReference<int>).ExpressionText.ToString();
			}
			if (value is OutArgument<string>)
			{
				return ((value as OutArgument).Expression as VisualBasicReference<string>).ExpressionText.ToString();
			}
			if (value is OutArgument<bool>)
			{
				return ((value as OutArgument).Expression as VisualBasicReference<bool>).ExpressionText.ToString();
			}
			if (value is OutArgument<Grade>)
			{
				return ((value as OutArgument).Expression as VisualBasicReference<Grade>).ExpressionText.ToString();
			}
			if (value is OutArgument<Slope>)
			{
				return ((value as OutArgument).Expression as VisualBasicReference<Slope>).ExpressionText.ToString();
			}
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = value as string;
		if (!string.IsNullOrEmpty(text))
		{
			Type parameterType = ActivityDesigntime.Instance.GetParameterType(value as string);
			if (parameterType == typeof(OutArgument<double>) || parameterType == typeof(InOutArgument<double>))
			{
				return new OutArgument<double>(new VisualBasicReference<double>
				{
					ExpressionText = text
				});
			}
			if (parameterType == typeof(OutArgument<int>) || parameterType == typeof(InOutArgument<int>))
			{
				return new OutArgument<int>(new VisualBasicReference<int>
				{
					ExpressionText = text
				});
			}
			if (parameterType == typeof(OutArgument<string>) || parameterType == typeof(InOutArgument<string>))
			{
				return new OutArgument<string>(new VisualBasicReference<string>
				{
					ExpressionText = text
				});
			}
			if (parameterType == typeof(OutArgument<bool>) || parameterType == typeof(InOutArgument<bool>))
			{
				return new OutArgument<bool>(new VisualBasicReference<bool>
				{
					ExpressionText = text
				});
			}
			if (parameterType == typeof(OutArgument<Grade>) || parameterType == typeof(InOutArgument<Grade>))
			{
				return new OutArgument<Grade>(new VisualBasicReference<Grade>
				{
					ExpressionText = text
				});
			}
			if (parameterType == typeof(OutArgument<Slope>) || parameterType == typeof(InOutArgument<Slope>))
			{
				return new OutArgument<Slope>(new VisualBasicReference<Slope>
				{
					ExpressionText = text
				});
			}
		}
		return null;
	}
}
