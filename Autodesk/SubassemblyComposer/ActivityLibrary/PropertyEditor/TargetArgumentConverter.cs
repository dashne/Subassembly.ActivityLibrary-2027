using System;
using System.Activities;
using System.Globalization;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class TargetArgumentConverter<T> : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return GlobleString.String_None;
		}
		if (!((value as InArgument<T>).Expression is VisualBasicValue<T> visualBasicValue))
		{
			return GlobleString.String_None;
		}
		string text = visualBasicValue.ExpressionText.ToString();
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return GlobleString.String_None;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || (value as string).Equals(GlobleString.String_None))
		{
			return null;
		}
		return new InArgument<T>(new VisualBasicValue<T>
		{
			ExpressionText = (value as string)
		});
	}
}
