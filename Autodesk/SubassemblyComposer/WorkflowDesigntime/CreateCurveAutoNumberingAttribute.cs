using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class CreateCurveAutoNumberingAttribute : CreateLinkAutoNumberingAttribute
{
	protected override bool IsNewActivity(ModelItem modelItem)
	{
		if (modelItem.GetCurrentValue() is CreateCurve { LinkNumber: not null })
		{
			return false;
		}
		return true;
	}

	protected override void SubSetActivityProperties(ModelItem modelItem, string displayName)
	{
		base.SubSetActivityProperties(modelItem, displayName);
		SetSideProperty(modelItem);
	}
}
