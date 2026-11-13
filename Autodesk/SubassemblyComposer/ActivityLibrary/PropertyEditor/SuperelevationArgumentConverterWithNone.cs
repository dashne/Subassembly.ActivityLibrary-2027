using System;
using System.Activities;
using System.Globalization;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class SuperelevationArgumentConverterWithNone : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		InArgument<CrossSegmentType> inArgument = value as InArgument<CrossSegmentType>;
		if (inArgument.Expression == null)
		{
			return CrossSegmentType.None.GetDisplayName();
		}
		return ParseCrossSegmentType(inArgument.Expression.ToString()).GetDisplayName();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		CrossSegmentType crossSegmentType = ParseCrossSegmentType(value as string);
		return new InArgument<CrossSegmentType>
		{
			Expression = crossSegmentType
		};
	}

	private CrossSegmentType ParseCrossSegmentType(string value)
	{
		foreach (CrossSegmentType item in CrossSegmentType.None.GetAvailableSegmentTypesPlusNone())
		{
			if (item.GetDisplayName() == value || item.ToString() == value)
			{
				return item;
			}
		}
		throw new ArgumentException($"Unexpected argument, {value} is not in the list of CrossSegmentType");
	}
}
