using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal sealed class CutOrFillConditionAutoNumberingAttribute : ConditionActivityAutoNumberingAttribute
{
	public override void SetActivityProperties(ModelItem modelItem)
	{
		base.SetActivityProperties(modelItem);
		SetDisplayName(modelItem, Resource.DisplayName_CutOrFillCondition);
		SetSubassembly(modelItem);
	}
}
