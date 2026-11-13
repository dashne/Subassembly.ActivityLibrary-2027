using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class LinkCodeComboBox : CodeComboBoxEditor
{
	public LinkCodeComboBox()
		: base(new DefaultCodesArgumentConverter(), CodeType.Link)
	{
	}
}
