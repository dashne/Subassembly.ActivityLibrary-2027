using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class RoundDayLightAutoNumberingAttribute : SubassemblyActivityAutoNumberingAttribute
{
	protected override bool IsNewActivity(ModelItem modelItem)
	{
		if (modelItem.GetCurrentValue() is RoundDayLightActivity { LinkNumber: not null })
		{
			return false;
		}
		return true;
	}

	protected override void SubSetActivityProperties(ModelItem modelItem, string displayName)
	{
		SetDisplayName(modelItem, displayName);
		SetSideProperty(modelItem);
	}
}
