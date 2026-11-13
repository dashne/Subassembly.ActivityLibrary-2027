namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class AutoLinkCheckbox : InlineCheckBox
{
	public AutoLinkCheckbox()
	{
		PropertyBinding.EnabledProperty(this, "FromPoint", new FromPointValueConverter());
	}
}
