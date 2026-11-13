using System.Activities;
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

[Designer(typeof(SetMarkPointDesigner))]
[SetMarkPointAutoNumbering]
[ToolboxBitmap(typeof(SetMarkPointActivity), "Resources.Icons.set_mark_point_toolbox.ico")]
public class SetMarkPointActivity : CustomActivity
{
	[RequiredArgument]
	[CustomCategory(typeof(Resource), "Category_General", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SetMarkPoint_PointName", 1)]
	[Editor(typeof(PointComboBox), typeof(PropertyValueEditor))]
	public InArgument<string> PointNumber { get; set; }

	[CustomCategory(typeof(Resource), "Category_General", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SetMarkPoint_MarkName", 2)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<string> MarkName { get; set; }

	[RequiredArgument]
	[Browsable(false)]
	public InOutArgument<Geometry> Geometry { get; set; }

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = base.SubValidateInDesigntime(activityId);
		ObservableCollection<string> allValidPointNames = ValidGeometryNamesHelper.GetAllValidPointNames(ActivityDesigntime.Instance.CodePathTree, activityId, includeOrigin: false);
		if (!this.PointArgumentValidate(PointNumber, allValidPointNames))
		{
			string message = string.Format(Resource.Message_Validate_Invalid_Value, Resource.DisplayName_SetMarkPoint_PointName);
			list.Add(new ValidationError(message, isWarning: false, "PointNumber"));
		}
		return list;
	}

	protected override void LayoutExecute(CodeActivityContext context)
	{
		InternalExecute(context);
	}

	protected override void RoadwayExecute(CodeActivityContext context)
	{
		InternalExecute(context);
	}

	private void InternalExecute(CodeActivityContext context)
	{
		Geometry value = context.GetValue(Geometry);
		string value2 = context.GetValue(PointNumber);
		string value3 = context.GetValue(MarkName);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point = value.GetPoint(value2);
		if (string.IsNullOrEmpty(value3))
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_SetMarkPoint_NoName);
		}
		else if (point == null)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_SetMarkPoint_PointNotExist);
		}
		else if (!value.MarkPointManager.SetMarkPoint(value3, point))
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_SetMarkPoint_Failed);
		}
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
	}
}
