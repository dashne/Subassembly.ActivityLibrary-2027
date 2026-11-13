using System;
using System.Activities;
using System.Globalization;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class RefPointArgumentConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && value is InArgument<string> { Expression: not null } inArgument)
		{
			return inArgument.Expression.ToString();
		}
		return GlobleString.String_None;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (string.IsNullOrEmpty(value as string) || (value as string).Equals(GlobleString.String_Origin) || (value as string).Equals(GlobleString.String_None))
		{
			return null;
		}
		return new InArgument<string>
		{
			Expression = (value as string)
		};
	}
}
