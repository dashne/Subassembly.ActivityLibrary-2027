using System;
using System.Activities;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class FromPointValueConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			Activity<string> expression = (value as InArgument<string>).Expression;
			if (expression == null)
			{
				return false;
			}
			if (expression.ToString().Equals(GlobleString.String_Origin))
			{
				return false;
			}
			if (parameter.GetType().Equals(typeof(CreatePoint)) && !Regex.Match(expression.ToString(), new PointAutoNumberingAttribute().IDPattern).Success)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}
}
