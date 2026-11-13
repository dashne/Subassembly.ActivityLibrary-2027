using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class IntersectionTypeComboBox : EnumComboBox<IntersectionType>
{
	public IntersectionTypeComboBox()
		: base((IValueConverter)new EnumGeneralConverter<IntersectionType>())
	{
	}
}
