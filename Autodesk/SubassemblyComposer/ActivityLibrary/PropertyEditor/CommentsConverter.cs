using System;
using System.Globalization;
using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class CommentsConverter : IValueConverter
{
	private string oldDisplayName;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string result = string.Empty;
		string text = value as string;
		if (!string.IsNullOrEmpty(text))
		{
			result = ExtractComment(text);
			oldDisplayName = text;
		}
		return result;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string newComment = value as string;
		return InjectComment(oldDisplayName, newComment);
	}

	public static string ExtractComment(string displayName)
	{
		string result = string.Empty;
		int num = displayName.IndexOf('(');
		int num2 = displayName.LastIndexOf(')');
		if (num >= 0 && num2 >= 0 && num2 > num)
		{
			result = displayName.Substring(num + 1, num2 - num - 1);
		}
		return result;
	}

	public static string InjectComment(string displayName, string newComment)
	{
		string result = displayName;
		int num = displayName.IndexOf('(');
		int num2 = displayName.LastIndexOf(')');
		if (!string.IsNullOrEmpty(newComment))
		{
			if (num >= 0 && num2 >= 0 && num2 > num)
			{
				string oldValue = displayName.Substring(num + 1, num2 - num - 1);
				result = displayName.Replace(oldValue, newComment);
			}
			else
			{
				result = displayName.Trim() + " (" + newComment + ")";
			}
		}
		else if (num >= 0)
		{
			result = displayName.Substring(0, num);
		}
		return result;
	}
}
