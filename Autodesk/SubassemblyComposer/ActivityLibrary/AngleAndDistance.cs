using System;
using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.GeomMath;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class AngleAndDistance : CreationRuleBase, IPointRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleBase_ReferencePoint", 2)]
	[Editor(typeof(RefPointComboBox), typeof(PropertyValueEditor))]
	[CanReferGeometryName]
	public InArgument<string> AngleReferencePoint8
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDistance p) => p.AngleReferencePoint8)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDistance p) => p.AngleReferencePoint8), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleBase_ReverseAngleReference", 3)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ReverseAngleReference8
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDistance p) => p.ReverseAngleReference8)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDistance p) => p.ReverseAngleReference8), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleBase_Angle", 4)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> Angle8
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDistance p) => p.Angle8)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDistance p) => p.Angle8), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleAndDistance_Distance", 5)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> Distance8
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDistance p) => p.Distance8)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDistance p) => p.Distance8), value);
		}
	}

	public AngleAndDistance()
	{
		AngleReferencePoint8 = new InArgument<string>
		{
			Expression = GlobleString.String_None
		};
		ReverseAngleReference8 = new InArgument<bool>
		{
			Expression = false
		};
		Angle8 = new InArgument<double>
		{
			Expression = 0.0
		};
		Distance8 = new InArgument<double>
		{
			Expression = 0.0
		};
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
		Point point = geom.GetPoint(context.GetValue(AngleReferencePoint8));
		bool value = context.GetValue(ReverseAngleReference8);
		double value2 = context.GetValue(Angle8);
		double value3 = context.GetValue(Distance8);
		value2 = GeometryFactory.AdjustAngel_Degree(value2);
		GeomPoint geoFromPt = new GeomPoint(fromPoint.X, fromPoint.Y);
		if (point == null)
		{
			return GeometryFactory.AddPointByAngleDistance(geoFromPt, GeomPoint.Empty, value2 * (double)side, value3, value);
		}
		GeomPoint geomPoint = new GeomPoint(point.X, point.Y);
		if (Math.Abs(geoFromPt.Distance(geomPoint)) < 1E-05)
		{
			string msg = string.Format(Resource.ErrorHandling_OverlappedPoints, point.GeometryId, fromPoint.GeometryId);
			recordError.RecordErrorInfo(context, ErrorLevel.Error, msg);
			return null;
		}
		return GeometryFactory.AddPointByAngleDistance(geoFromPt, geomPoint, value2 * (double)side, value3, value);
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> result = new List<ValidationError>();
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, Angle8, Resource.DisplayName_AngleBase_Angle, result);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, Distance8, Resource.DisplayName_AngleAndDistance_Distance, result);
		return result;
	}
}
