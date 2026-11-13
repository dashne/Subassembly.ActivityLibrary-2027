using System.Activities;
using System.Activities.Expressions;
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

[Designer(typeof(GetMarkPointDesigner))]
[GetMarkPointAutoNumbering]
[ToolboxBitmap(typeof(GetMarkPointActivity), "Resources.Icons.get_mark_point_toolbox.ico")]
public class GetMarkPointActivity : SubassemblyActivity
{
	protected InArgument<string> _pointNumber;

	[RequiredArgument]
	[CanReferParameterName]
	[CustomCategory(typeof(Resource), "Category_General", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_GetMarkPoint_MarkName", 2)]
	public InArgument<string> MarkName
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((GetMarkPointActivity p) => p.MarkName)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((GetMarkPointActivity p) => p.MarkName), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Point", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_PointNumber", 3)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[PointAutoNumbering]
	public InArgument<string> PointNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((GetMarkPointActivity p) => p.PointNumber)) as InArgument<string>;
		}
		set
		{
			_pointNumber = PropertyCollection_Get(this.PropertyName((GetMarkPointActivity p) => p.PointNumber)) as InArgument<string>;
			PropertyCollection_Set(this.PropertyName((GetMarkPointActivity p) => p.PointNumber), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Point", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_PointCodes", 2)]
	[Editor(typeof(PointCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferPointCodeName]
	public List<InArgument<string>> PointCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((GetMarkPointActivity p) => p.PointCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((GetMarkPointActivity p) => p.PointCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Preview", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FromPoint", 1)]
	[Editor(typeof(PointComboBox), typeof(PropertyValueEditor))]
	[CanReferGeometryName]
	[CanReferVariableName]
	public InArgument<string> PrevFromPoint
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((GetMarkPointActivity p) => p.PrevFromPoint)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((GetMarkPointActivity p) => p.PrevFromPoint), value);
		}
	}

	[RequiredArgument]
	[CustomCategory(typeof(Resource), "Category_Preview", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DeltaXAndDeltaY_DeltaX", 2)]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> PrevOffset
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((GetMarkPointActivity p) => p.PrevOffset)) as InArgument<double>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((GetMarkPointActivity p) => p.PrevOffset), value);
		}
	}

	[RequiredArgument]
	[CustomCategory(typeof(Resource), "Category_Preview", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DeltaXAndDeltaY_DeltaY", 3)]
	[CanReferParameterName]
	public InArgument<double> PrevDeltaElevation
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((GetMarkPointActivity p) => p.PrevDeltaElevation)) as InArgument<double>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((GetMarkPointActivity p) => p.PrevDeltaElevation), value);
		}
	}

	[Browsable(false)]
	public InArgument<int> Side
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((GetMarkPointActivity p) => p.Side)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((GetMarkPointActivity p) => p.Side), value);
		}
	}

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = base.SubValidateInDesigntime(activityId);
		string value = (PointNumber.Expression as Literal<string>).Value;
		if (!GeomertyNameCheckHelper.Execute(ActivityDesigntime.Instance.CodePathTree, value, activityId))
		{
			list.Add(new ValidationError(Resource.Message_Validate_Duplicate_Name, isWarning: false, "PointNumber"));
		}
		ObservableCollection<string> allValidPointNames = ValidGeometryNamesHelper.GetAllValidPointNames(ActivityDesigntime.Instance.CodePathTree, activityId, includeOrigin: false);
		if (!this.PointArgumentValidate(PrevFromPoint, allValidPointNames))
		{
			string message = string.Format(Resource.Message_Validate_Invalid_Value, Resource.DisplayName_SetMarkPoint_PointName);
			list.Add(new ValidationError(message, isWarning: false, "PreviewFromPoint"));
		}
		return list;
	}

	public GetMarkPointActivity()
	{
		PropertyCollection_Set(this.PropertyName((GetMarkPointActivity p) => p.PointNumber), null, PointNumberOnChanged);
		PrevOffset = 0.0;
		PrevDeltaElevation = 0.0;
	}

	protected void PointNumberOnChanged(object value)
	{
		if (value != null && value != _pointNumber)
		{
			InArgument<string> pointNumber = _pointNumber;
			_pointNumber = value as InArgument<string>;
			OnGeometryNameChanged(this.PropertyName((GetMarkPointActivity p) => p.PointNumber), pointNumber, _pointNumber);
		}
	}

	protected override void LayoutExecute(CodeActivityContext context)
	{
		Geometry value = context.GetValue(Geometry);
		context.GetValue(MarkName);
		context.GetValue(SubassemblyRunMode);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point previewPoint = GetPreviewPoint(context);
		previewPoint.ActivityId = context.GetValue(ActivityId);
		previewPoint.GeometryId = context.GetValue(PointNumber);
		previewPoint.Codes = GetCodes(context, PointCodes);
		previewPoint.IsAuxiliary = IsCreatingAux;
		if (ActivityDesigntime.Instance != null)
		{
			previewPoint.DisplayName = DisplayName;
		}
		value.Points.Add(previewPoint);
	}

	protected override void RoadwayExecute(CodeActivityContext context)
	{
		Geometry value = context.GetValue(Geometry);
		string value2 = context.GetValue(MarkName);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point = value.MarkPointManager.GetMarkPoint(value2);
		if (point == null)
		{
			RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_GetMarkPoint_NotExist);
			point = GetPreviewPoint(context);
		}
		point.ActivityId = context.GetValue(ActivityId);
		point.GeometryId = context.GetValue(PointNumber);
		point.Codes = GetCodes(context, PointCodes);
		point.IsAuxiliary = IsCreatingAux;
		if (ActivityDesigntime.Instance != null)
		{
			point.DisplayName = DisplayName;
		}
		value.Points.Add(point);
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
		BindCodeArguments(metadata, PointCodes, this.PropertyName((GetMarkPointActivity p) => p.PointCodes));
		ValidateCodeArguments(metadata, PointCodes);
	}

	protected Autodesk.SubassemblyComposer.WorkflowEngine.Point GetPreviewPoint(CodeActivityContext context)
	{
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point = context.GetValue(Geometry).GetPoint(context.GetValue(PrevFromPoint));
		if (point == null)
		{
			point = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(0.0, 0.0);
		}
		double num = 0.0;
		if (PrevOffset != null && PrevOffset.Expression != null)
		{
			int num2 = ((context.GetValue(Side) != 1) ? 1 : (-1));
			num = context.GetValue(PrevOffset) * (double)num2;
		}
		double num3 = 0.0;
		if (PrevDeltaElevation != null && PrevDeltaElevation.Expression != null)
		{
			num3 = context.GetValue(PrevDeltaElevation);
		}
		return new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point.X + num, point.Y + num3);
	}
}
