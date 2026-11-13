using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(CutOrFillConditionDesigner))]
[CutOrFillConditionAutoNumbering]
[ToolboxBitmap(typeof(CutOrFillCondition), "Resources.Icons.CutOrFill_toolbox.ico")]
public class CutOrFillCondition : CustomCondition
{
	public const string PointArgumentName = "Point";

	public const string SurfaceArgumentname = "Surface";

	[CustomCategory(typeof(Resource), "Category_Condition", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_CutOrFill_Point", 1)]
	[RequiredArgument]
	[CanReferGeometryName]
	[Editor(typeof(PointComboBox), typeof(PropertyValueEditor))]
	public InArgument<string> Point { get; set; }

	[CustomCategory(typeof(Resource), "Category_Condition", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_CutOrFill_Surface", 2)]
	[RequiredArgument]
	[CanReferParameterName]
	[Editor(typeof(TargetComboBox<SurfaceTarget>), typeof(PropertyValueEditor))]
	public InArgument<SurfaceTarget> Surface { get; set; }

	[RequiredArgument]
	[Browsable(false)]
	public InOutArgument<Geometry> Geometry { get; set; }

	public override string TrueLabel => Resource.CutOrFillCondition_TrueLable;

	public override string FalseLabel => Resource.CutOrFillCondition_FalseLable;

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = new List<ValidationError>();
		ObservableCollection<string> allValidPointNames = ValidGeometryNamesHelper.GetAllValidPointNames(ActivityDesigntime.Instance.CodePathTree, activityId, includeOrigin: false);
		if (!this.PointArgumentValidate(Point, allValidPointNames))
		{
			string message = string.Format(Resource.Message_Validate_Invalid_Value, "Point");
			list.Add(new ValidationError(message, isWarning: false, "Point"));
		}
		return list;
	}

	static CutOrFillCondition()
	{
		AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
		attributeTableBuilder.AddCustomAttributes(typeof(CutOrFillCondition), typeof(CutOrFillCondition).GetProperty("DisplayName", typeof(string)), new CustomCategoryAttribute(typeof(Resource), "Category_Misc", 1));
		attributeTableBuilder.AddCustomAttributes(typeof(CutOrFillCondition), typeof(CutOrFillCondition).GetProperty("DisplayName", typeof(string)), new CustomDisplayNameAttribute(typeof(Resource), "DisplayName", 1));
		MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
	}

	protected override void SubExecute(NativeActivityContext context)
	{
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point = context.GetValue(Geometry).GetPoint(context.GetValue(Point));
		if (point == null)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_CutOrFillCondition_PointNotFound);
			return;
		}
		SurfaceTarget value = context.GetValue(Surface);
		if (value == null || !value.IsValid)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_CutOrFillCondition_SurfaceNotFound);
			return;
		}
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point2 = null;
		if (value != null && value.IsValid)
		{
			point2 = value.getPointOnOffset(point.Offset);
		}
		if (point2 == null)
		{
			context.SetValue(Result, value: false);
		}
		else
		{
			context.SetValue(Result, point.Elevation < point2.Elevation);
		}
	}
}
