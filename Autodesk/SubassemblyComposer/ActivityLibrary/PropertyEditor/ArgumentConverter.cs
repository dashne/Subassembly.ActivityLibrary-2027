using System;
using System.Activities;
using System.Globalization;
using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class ArgumentConverter<T> : IValueConverter
{
	public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			Activity<T> expression = (value as InArgument<T>).Expression;
			if (expression != null)
			{
				return expression.ToString();
			}
		}
		return null;
	}

	public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(double))
		{
			if (double.TryParse(value as string, out var result))
			{
				return new InArgument<double>
				{
					Expression = result
				};
			}
			return null;
		}
		if (typeFromHandle == typeof(int))
		{
			if (int.TryParse(value as string, out var result2))
			{
				return new InArgument<int>
				{
					Expression = result2
				};
			}
			return null;
		}
		if (typeFromHandle == typeof(bool))
		{
			if (bool.TryParse(value as string, out var result3))
			{
				return new InArgument<bool>
				{
					Expression = result3
				};
			}
			return null;
		}
		return new InArgument<string>
		{
			Expression = value.ToString()
		};
	}
}
