namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class AutoLinkCodesTextBox : LinkCodeComboBox
{
	public AutoLinkCodesTextBox()
	{
		PropertyBinding.EnabledProperty(this, "AutoLink", new BoolArgumentConverter());
	}
}
