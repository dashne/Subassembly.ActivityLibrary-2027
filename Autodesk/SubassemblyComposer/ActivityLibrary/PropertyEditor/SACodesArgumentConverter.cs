using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class SACodesArgumentConverter : ArgumentConverter<string>
{
	private const char LiteralStartSymbol = '"';

	private const char LiteralEndSymbol = '"';

	private const char SeperateSymbol = ',';

	public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			if (!(value is List<InArgument<string>> { Count: not 0 } list))
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (InArgument<string> item in list)
			{
				if (item != null && item.Expression != null)
				{
					if (item.Expression is VisualBasicValue<string>)
					{
						stringBuilder.Append((item.Expression as VisualBasicValue<string>).ExpressionText);
					}
					else
					{
						stringBuilder.Append('"');
						stringBuilder.Append(item.Expression.ToString());
						stringBuilder.Append('"');
					}
					stringBuilder.Append(',');
				}
			}
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}
		return null;
	}

	public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			List<InArgument<string>> list = new List<InArgument<string>>();
			string[] array = (value as string).Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (!string.IsNullOrEmpty(text))
				{
					if (text.StartsWith('"'.ToString()) && text.EndsWith('"'.ToString()))
					{
						InArgument<string> item = new InArgument<string>(text.Substring(1, text.Length - 2));
						list.Add(item);
					}
					else
					{
						InArgument<string> item2 = new InArgument<string>(new VisualBasicValue<string>(text));
						list.Add(item2);
					}
				}
			}
			return list;
		}
		return null;
	}
}
