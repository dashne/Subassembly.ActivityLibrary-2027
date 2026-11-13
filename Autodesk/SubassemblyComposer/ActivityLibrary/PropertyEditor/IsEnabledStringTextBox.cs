namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class IsEnabledStringTextBox : StringTextBox
{
	public IsEnabledStringTextBox()
	{
		MyPropertyBinding.EnabledProperty(this, "IsEnabled", new BoolArgumentConverter());
	}
}
