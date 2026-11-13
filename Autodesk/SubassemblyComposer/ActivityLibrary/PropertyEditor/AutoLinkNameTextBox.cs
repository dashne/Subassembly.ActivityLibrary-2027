namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class AutoLinkNameTextBox : StringTextBox
{
	public AutoLinkNameTextBox()
	{
		PropertyBinding.EnabledProperty(this, "AutoLink", new BoolArgumentConverter());
	}
}
