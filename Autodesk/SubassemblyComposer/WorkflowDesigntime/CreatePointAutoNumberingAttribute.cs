using System;
using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class CreatePointAutoNumberingAttribute : SubassemblyActivityAutoNumberingAttribute
{
	protected override bool IsNewActivity(ModelItem modelItem)
	{
		if (modelItem.GetCurrentValue() is CreatePoint { PointNumber: not null })
		{
			return false;
		}
		return true;
	}

	protected override void SubSetActivityProperties(ModelItem modelItem, string displayName)
	{
		SetDisplayName(modelItem, displayName);
		SetSideProperty(modelItem);
		if (modelItem.GetCurrentValue() is CreatePoint { GeometryTypeChanged: null } createPoint)
		{
			createPoint.GeometryTypeChanged = (EventHandler<EventArgs>)Delegate.Combine(createPoint.GeometryTypeChanged, new EventHandler<EventArgs>(createPoint.RefreshPropertyInspector));
		}
	}
}
