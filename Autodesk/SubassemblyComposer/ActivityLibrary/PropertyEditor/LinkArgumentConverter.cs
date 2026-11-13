using System;
using System.Activities;
using System.Globalization;
using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class LinkArgumentConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && value is InArgument<string> { Expression: not null } inArgument)
		{
			return inArgument.Expression.ToString();
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (string.IsNullOrEmpty(value as string))
		{
			return null;
		}
		return new InArgument<string>
		{
			Expression = (value as string)
		};
	}
}
