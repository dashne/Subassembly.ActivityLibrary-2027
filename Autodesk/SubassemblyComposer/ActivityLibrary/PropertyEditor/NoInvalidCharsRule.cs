using System.Globalization;
using System.Windows.Controls;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

public class NoInvalidCharsRule : ValidationRule
{
	public bool IsMultiCodes { get; set; }

	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		char[] array = (IsMultiCodes ? CodeValidator.ScanInvalidCharsForMultiCodes((string)value) : CodeValidator.ScanInvalidChars((string)value));
		if (array.Length != 0)
		{
			string text = ((array.Length == 1) ? Resource.Error_Invalid_Char : Resource.Error_Invalid_Chars);
			return new ValidationResult(isValid: false, string.Join(" ", array) + " " + text);
		}
		return ValidationResult.ValidResult;
	}
}
