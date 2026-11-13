using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class InterpolatePoint : CreationRuleBase, IPointRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_InterpolatePoint_DestPoint", 2)]
	[Editor(typeof(PointComboBox), typeof(PropertyValueEditor))]
	[CanReferGeometryName]
	public InArgument<string> DestPoint
	{
		get
		{
			return GetPropertyValue(this.PropertyName((InterpolatePoint p) => p.DestPoint)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((InterpolatePoint p) => p.DestPoint), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_InterpolatePoint_Parameter", 3)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> InterpolateParam
	{
		get
		{
			return GetPropertyValue(this.PropertyName((InterpolatePoint p) => p.InterpolateParam)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((InterpolatePoint p) => p.InterpolateParam), value);
		}
	}

	public InterpolatePoint()
	{
		DestPoint = new InArgument<string>();
		InterpolateParam = new InArgument<double>(0.5);
		Init();
	}

	public Point LayoutExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		return InternalExecute(context, geom, fromPoint, side, recordError);
	}

	public Point RoadwayExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		return InternalExecute(context, geom, fromPoint, side, recordError);
	}

	private Point InternalExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		string value = context.GetValue(DestPoint);
		Point point = geom.GetPoint(value);
		if (point == null)
		{
			if (!string.IsNullOrEmpty(value))
			{
				string msg = string.Format(Resource.ErrorHandling_CreatePoint_TargetPointNotFound, value);
				recordError.RecordErrorInfo(context, ErrorLevel.Error, msg);
				return null;
			}
			point = new Point(0.0, 0.0);
		}
		double value2 = context.GetValue(InterpolateParam);
		return GeometryUtility.InterpolatePoint(fromPoint, point, value2);
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> list = new List<ValidationError>();
		if (!validate.PointArgumentValidate(DestPoint, validPointNames))
		{
			string message = string.Format(Resource.Message_Validate_Invalid_Value, Resource.DisplayName_FromPoint);
			list.Add(new ValidationError(message, isWarning: false, "FromPoint"));
		}
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, InterpolateParam, Resource.DisplayName_InterpolatePoint_Parameter, list);
		return list;
	}
}
