using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class LinkIntersectionAutoNumberingAttribute : CreatePointAutoNumberingAttribute
{
	protected override bool IsNewActivity(ModelItem modelItem)
	{
		if (modelItem.GetCurrentValue() is LinkIntersection { PointNumber: not null })
		{
			return false;
		}
		return true;
	}
}
