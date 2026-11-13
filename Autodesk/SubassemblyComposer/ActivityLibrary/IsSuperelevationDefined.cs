using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(IsSuperelevationDefinedDesigner))]
[IsSuperelevationDefinedAutoNumbering]
public class IsSuperelevationDefined : CustomCondition
{
	public const string CrossSlopeToCheckArgumentName = "CrossSlopeToCheck";

	[CustomCategory(typeof(Resource), "Category_Condition", 1)]
	[Editor(typeof(SuperelevationComboBoxWithAll), typeof(PropertyValueEditor))]
	public InArgument<CrossSegmentType> CrossSlopeToCheck { get; set; }

	protected override void SubExecute(NativeActivityContext context)
	{
		CrossSegmentType value = context.GetValue(CrossSlopeToCheck);
		bool flag = false;
		context.SetValue(value: (value != CrossSegmentType.All) ? Superelevation.Data.GetSlopeAt(value).IsDefined : IsAllCrossSlopeDefined(), argument: Result);
	}

	private static bool IsAllCrossSlopeDefined()
	{
		foreach (CrossSegmentType availableSegmentType in CrossSegmentType.All.GetAvailableSegmentTypes())
		{
			if (!Superelevation.Data.GetSlopeAt(availableSegmentType).IsDefined)
			{
				return false;
			}
		}
		return true;
	}
}
