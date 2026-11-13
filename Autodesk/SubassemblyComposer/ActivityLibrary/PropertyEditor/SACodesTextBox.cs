namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class SACodesTextBox : InlineTextBoxEditor
{
	public SACodesTextBox()
		: base(new SACodesArgumentConverter())
	{
	}
}
