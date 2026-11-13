using System;
using System.Activities;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowEngine;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class TypeInfoArgumentConverter<T> : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			InArgument<double> inArgument = value as InArgument<double>;
			if (double.TryParse(inArgument.Expression.ToString(), out var result))
			{
				string text = string.Empty;
				if (typeof(T) == typeof(Grade))
				{
					text = Grade.FormatAsString(result);
				}
				else if (typeof(T) == typeof(Slope))
				{
					text = Slope.FormatAsString(result);
				}
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			if (inArgument.Expression is VisualBasicValue<double> visualBasicValue)
			{
				return visualBasicValue.ExpressionText;
			}
		}
		return DependencyProperty.UnsetValue;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && value is string text)
		{
			if (double.TryParse(text, out var result))
			{
				return new InArgument<double>
				{
					Expression = result
				};
			}
			if (Grade.TryParse(text, out result))
			{
				return new InArgument<double>
				{
					Expression = result
				};
			}
			if (Slope.TryParse(text, out result))
			{
				return new InArgument<double>
				{
					Expression = result
				};
			}
			return new InArgument<double>(new VisualBasicValue<double>
			{
				ExpressionText = text
			});
		}
		return null;
	}
}
