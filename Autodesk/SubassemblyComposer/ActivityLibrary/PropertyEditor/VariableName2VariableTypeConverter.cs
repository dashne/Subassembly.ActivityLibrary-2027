using System;
using System.Activities.Presentation.Model;
using System.Globalization;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class VariableName2VariableTypeConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null && value is ModelItem modelItem)
		{
			string variableName = VariableConverter.OutArgument2String(modelItem.GetCurrentValue());
			return ActivityDesigntime.Instance.GetVariableType(variableName);
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}
}
