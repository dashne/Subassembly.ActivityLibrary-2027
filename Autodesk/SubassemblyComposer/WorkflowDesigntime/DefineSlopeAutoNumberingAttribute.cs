using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class DefineSlopeAutoNumberingAttribute : CustomActivityAutoNumberingAttribute
{
	public override void SetActivityProperties(ModelItem modelItem)
	{
		base.SetActivityProperties(modelItem);
		SetDisplayName(modelItem, Resource.DisplayName_DefineSlope);
	}
}
