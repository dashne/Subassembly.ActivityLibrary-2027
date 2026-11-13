using System;
using System.Activities;
using System.Globalization;
using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class StringArgumentConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && value is InArgument<string> { Expression: not null } inArgument)
		{
			return inArgument.Expression.ToString();
		}
		return string.Empty;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (string.IsNullOrEmpty(value.ToString()))
		{
			return null;
		}
		return new InArgument<string>
		{
			Expression = value.ToString()
		};
	}
}
