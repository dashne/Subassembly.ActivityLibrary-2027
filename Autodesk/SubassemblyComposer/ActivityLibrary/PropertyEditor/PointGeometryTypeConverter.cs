using System;
using System.Collections.Generic;
using System.Globalization;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class PointGeometryTypeConverter : EnumDisplayNameConverter
{
	private static Dictionary<PointGeometryType, string> _cachedEnum2DisplayName = new Dictionary<PointGeometryType, string>();

	private static Dictionary<string, PointGeometryType> _cachedDisplayName2Enum = new Dictionary<string, PointGeometryType>();

	public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		PointGeometryType pointGeometryType = (PointGeometryType)value;
		if (_cachedEnum2DisplayName.ContainsKey(pointGeometryType))
		{
			return _cachedEnum2DisplayName[pointGeometryType];
		}
		string enumDisplayName = EnumDisplayNameConverter.GetEnumDisplayName(pointGeometryType);
		_cachedEnum2DisplayName.Add(pointGeometryType, enumDisplayName);
		return enumDisplayName;
	}

	public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string text = value.ToString();
		if (_cachedDisplayName2Enum.ContainsKey(text))
		{
			return _cachedDisplayName2Enum[text];
		}
		foreach (PointGeometryType value2 in Enum.GetValues(typeof(PointGeometryType)))
		{
			string enumDisplayName = EnumDisplayNameConverter.GetEnumDisplayName(value2);
			if (!_cachedDisplayName2Enum.ContainsKey(enumDisplayName))
			{
				_cachedDisplayName2Enum.Add(enumDisplayName, value2);
			}
			if (text.Equals(enumDisplayName))
			{
				return value2;
			}
		}
		return null;
	}
}
