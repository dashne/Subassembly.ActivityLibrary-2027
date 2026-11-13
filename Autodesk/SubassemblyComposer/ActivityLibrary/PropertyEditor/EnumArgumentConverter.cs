using System;
using System.Activities;
using System.Globalization;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class EnumArgumentConverter<T> : EnumDisplayNameConverter where T : struct
{
	public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is InArgument<T> inArgument)
		{
			return EnumDisplayNameConverter.GetEnumDisplayName(Enum.Parse(typeof(T), inArgument.Expression.ToString()));
		}
		return null;
	}

	public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		foreach (object value2 in Enum.GetValues(typeof(T)))
		{
			string enumDisplayName = EnumDisplayNameConverter.GetEnumDisplayName(value2);
			if (value.ToString() == enumDisplayName)
			{
				return new InArgument<T>
				{
					Expression = (T)value2
				};
			}
		}
		return null;
	}
}
