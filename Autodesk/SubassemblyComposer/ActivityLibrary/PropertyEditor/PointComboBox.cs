using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class PointComboBox : GeneralComboBox
{
	public PointComboBox()
		: base(new PointArgumentConverter(), ActivityDesigntime.Instance.Selection.ValidPointNamesView)
	{
	}
}
