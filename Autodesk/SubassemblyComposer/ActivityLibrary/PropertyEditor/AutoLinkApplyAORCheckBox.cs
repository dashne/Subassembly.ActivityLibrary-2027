namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class AutoLinkApplyAORCheckBox : InlineCheckBox
{
	public AutoLinkApplyAORCheckBox()
	{
		PropertyBinding.EnabledProperty(this, "AutoLink", new BoolArgumentConverter());
	}
}
