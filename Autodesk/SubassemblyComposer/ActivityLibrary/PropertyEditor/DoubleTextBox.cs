namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class DoubleTextBox : InlineTextBoxEditor
{
	public DoubleTextBox()
		: base(new ArgumentConverter<double>())
	{
	}
}
