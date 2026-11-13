using System;
using System.Activities;
using System.Activities.Expressions;
using System.Globalization;
using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class IntArgumentConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && (value as InArgument<int>).Expression is Literal<int> literal)
		{
			return literal.Value;
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			return new InArgument<int>
			{
				Expression = (int)value
			};
		}
		return null;
	}
}
