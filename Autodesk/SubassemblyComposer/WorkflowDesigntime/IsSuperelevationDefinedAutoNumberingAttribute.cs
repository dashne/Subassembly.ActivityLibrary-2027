using System.Activities;
using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class IsSuperelevationDefinedAutoNumberingAttribute : ConditionActivityAutoNumberingAttribute
{
	public override void SetActivityProperties(ModelItem modelItem)
	{
		base.SetActivityProperties(modelItem);
		SetDisplayName(modelItem, Resource.DisplayName_IsSuperelevationDefined);
		SetCrossSlopeToCheck(modelItem, CrossSegmentType.All);
	}

	private void SetCrossSlopeToCheck(ModelItem modelItem, CrossSegmentType defaultCrossSegment)
	{
		ModelProperty modelProperty = modelItem.Properties["CrossSlopeToCheck"];
		if (modelProperty.Value == null)
		{
			modelProperty.SetValue(new InArgument<CrossSegmentType>
			{
				Expression = defaultCrossSegment
			});
		}
	}
}
