using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class CurveTypeComboBox : EnumComboBox<CurveType>
{
	public CurveTypeComboBox()
		: base((IValueConverter)new EnumGeneralConverter<CurveType>())
	{
	}
}
