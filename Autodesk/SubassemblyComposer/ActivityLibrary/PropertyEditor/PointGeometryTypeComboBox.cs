using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class PointGeometryTypeComboBox : EnumComboBox<PointGeometryType>
{
	public PointGeometryTypeComboBox()
		: base((IValueConverter)new PointGeometryTypeConverter())
	{
	}
}
