using System;
using System.Activities;
using System.Globalization;
using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class BoolArgumentConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			if (value is InArgument<bool>)
			{
				Activity<bool> expression = (value as InArgument<bool>).Expression;
				if (expression != null)
				{
					return bool.Parse(expression.ToString());
				}
			}
			else
			{
				bool.Parse(value.ToString());
			}
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			Activity<bool> expression = (bool)value;
			return new InArgument<bool>
			{
				Expression = expression
			};
		}
		return null;
	}
}
