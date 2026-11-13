namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class Int32TextBox : InlineTextBoxEditor
{
	public Int32TextBox()
		: base(new ArgumentConverter<int>())
	{
	}
}
