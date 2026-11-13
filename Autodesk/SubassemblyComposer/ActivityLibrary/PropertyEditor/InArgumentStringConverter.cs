using System;
using System.Activities;
using System.Globalization;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class InArgumentStringConverter<T> : ArgumentConverter<string>
{
	public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			if ((value as InArgument<T>).Expression is VisualBasicValue<T> visualBasicValue)
			{
				return visualBasicValue.ExpressionText.ToString();
			}
			return base.Convert(value, targetType, parameter, culture);
		}
		return null;
	}

	public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			string text = value.ToString();
			if (ActivityDesigntime.Instance.GetInputParameterNames<T>().Contains(text))
			{
				return new InArgument<T>(new VisualBasicValue<T>(text));
			}
			return base.ConvertBack(value, targetType, parameter, culture);
		}
		return null;
	}
}
