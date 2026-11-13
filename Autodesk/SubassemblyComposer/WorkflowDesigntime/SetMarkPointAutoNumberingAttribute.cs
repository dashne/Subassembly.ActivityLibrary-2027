using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class SetMarkPointAutoNumberingAttribute : CustomActivityAutoNumberingAttribute
{
	public override void SetActivityProperties(ModelItem modelItem)
	{
		base.SetActivityProperties(modelItem);
		SetSubassembly(modelItem);
		SetDisplayName(modelItem, Resource.DisplayName_SetMarkPoint);
	}
}
