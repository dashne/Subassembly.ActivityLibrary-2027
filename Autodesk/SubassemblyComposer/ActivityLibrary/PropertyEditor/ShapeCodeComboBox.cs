using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class ShapeCodeComboBox : CodeComboBoxEditor
{
	public ShapeCodeComboBox()
		: base(new DefaultCodesArgumentConverter(), CodeType.Shape)
	{
	}
}
