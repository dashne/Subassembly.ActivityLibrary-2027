namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class IsEnabledMixPointComboBox : PointComboBox
{
	public IsEnabledMixPointComboBox()
	{
		MyPropertyBinding.EnabledProperty(this, "IsEnabled", new BoolArgumentConverter());
	}
}
