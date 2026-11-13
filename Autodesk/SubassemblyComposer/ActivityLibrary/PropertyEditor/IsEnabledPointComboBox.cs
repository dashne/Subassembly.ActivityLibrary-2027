namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class IsEnabledPointComboBox : NonAuxPointComboBox
{
	public IsEnabledPointComboBox()
	{
		MyPropertyBinding.EnabledProperty(this, "IsEnabled", new BoolArgumentConverter());
	}
}
