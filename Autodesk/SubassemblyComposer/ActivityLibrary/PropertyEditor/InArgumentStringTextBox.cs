namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class InArgumentStringTextBox<T> : InlineTextBoxEditor
{
	public InArgumentStringTextBox()
		: base(new InArgumentStringConverter<T>())
	{
	}
}
