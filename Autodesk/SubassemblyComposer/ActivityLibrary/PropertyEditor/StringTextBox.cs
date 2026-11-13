namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class StringTextBox : InlineTextBoxEditor
{
	public StringTextBox()
		: base(new ArgumentConverter<string>())
	{
	}
}
