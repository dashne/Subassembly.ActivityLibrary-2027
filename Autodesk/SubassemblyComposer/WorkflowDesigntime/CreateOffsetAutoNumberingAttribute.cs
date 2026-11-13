using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class CreateOffsetAutoNumberingAttribute : SubassemblyActivityAutoNumberingAttribute
{
	protected override bool IsNewActivity(ModelItem modelItem)
	{
		if (modelItem.GetCurrentValue() is CreateOffset { OffsetNumber: not null })
		{
			return false;
		}
		return true;
	}

	protected override void SubSetActivityProperties(ModelItem modelItem, string displayName)
	{
		SetDisplayName(modelItem, displayName);
	}
}
