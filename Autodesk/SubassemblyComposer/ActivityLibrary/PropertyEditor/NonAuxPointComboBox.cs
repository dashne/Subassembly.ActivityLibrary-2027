using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class NonAuxPointComboBox : GeneralComboBox
{
	public NonAuxPointComboBox()
		: base(new PointArgumentConverter(), ActivityDesigntime.Instance.Selection.ValidNonAuxPointNamesView)
	{
	}
}
