using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class RoundByCombobox : EnumComboBox<RoundByEnum>
{
	public RoundByCombobox()
		: base((IValueConverter)new EnumArgumentConverter<RoundByEnum>())
	{
	}
}
