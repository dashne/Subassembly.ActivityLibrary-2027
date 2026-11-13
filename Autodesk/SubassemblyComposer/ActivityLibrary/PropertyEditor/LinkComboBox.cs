using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class LinkComboBox : GeneralComboBox
{
	public LinkComboBox()
		: base(new StringArgumentConverter(), ActivityDesigntime.Instance.Selection.ValidLinkNamesView)
	{
	}
}
