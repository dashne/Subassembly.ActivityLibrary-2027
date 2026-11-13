using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class PointCodeComboBox : CodeComboBoxEditor
{
	public PointCodeComboBox()
		: base(new DefaultCodesArgumentConverter(), CodeType.Point)
	{
	}
}
