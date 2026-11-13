using System.Collections.Generic;
using System.Text.RegularExpressions;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class GeometryNameComparer : IComparer<string>
{
	private static Regex DigitalPattern = new Regex(GeometryNamePatterns.DigitalPattern);

	public int Compare(string x, string y)
	{
		int result = 0;
		if (x.Equals(y))
		{
			return result;
		}
		if (y.Equals(GlobleString.String_Origin))
		{
			return -1;
		}
		if (x.Equals(GlobleString.String_Origin))
		{
			return 1;
		}
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		int result2 = 0;
		int result3 = 0;
		MatchCollection matchCollection = DigitalPattern.Matches(x);
		if (matchCollection.Count == 0)
		{
			return Comparer<string>.Default.Compare(x, y);
		}
		Match match = matchCollection[0];
		text = x.Substring(0, match.Index);
		text3 = x.Substring(match.Index + match.Length);
		int.TryParse(match.Value, out result2);
		MatchCollection matchCollection2 = DigitalPattern.Matches(y);
		if (matchCollection2.Count == 0)
		{
			return Comparer<string>.Default.Compare(x, y);
		}
		Match match2 = matchCollection2[0];
		text2 = y.Substring(0, match2.Index);
		text4 = y.Substring(match2.Index + match2.Length);
		int.TryParse(match2.Value, out result3);
		result = Comparer<string>.Default.Compare(text, text2);
		if (result != 0)
		{
			return result;
		}
		result = Comparer<string>.Default.Compare(text3, text4);
		if (result != 0)
		{
			return result;
		}
		if (result2 < result3)
		{
			result = 1;
		}
		else if (result2 > result3)
		{
			result = -1;
		}
		return result;
	}
}
