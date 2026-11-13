using System.Text.RegularExpressions;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

public static class Extensions
{
	private static readonly Regex regex = new Regex(",+");

	public static string DeleteRepeatCommas(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return string.Empty;
		}
		return regex.Replace(str, ",");
	}
}
