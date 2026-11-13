using System;
using System.Collections.Generic;
using System.Globalization;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class PointGeometryTypeCollectionConverter : EnumDisplayNameConverter
{
	private List<string> _cachedResult;

	public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (!(value is Array enumArray))
		{
			return null;
		}
		if (_cachedResult != null)
		{
			return _cachedResult;
		}
		CacheDisplayNameStrings(enumArray);
		return _cachedResult;
	}

	private void CacheDisplayNameStrings(Array enumArray)
	{
		_cachedResult = new List<string>();
		foreach (object item in enumArray)
		{
			string enumDisplayName = EnumDisplayNameConverter.GetEnumDisplayName(item);
			if (!_cachedResult.Contains(enumDisplayName))
			{
				_cachedResult.Add(enumDisplayName);
			}
		}
		_cachedResult.Sort();
	}

	public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
