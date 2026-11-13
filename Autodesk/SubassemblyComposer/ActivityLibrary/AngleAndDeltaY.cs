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

internal class AngleAndDeltaY : CreationRuleBase, IPointRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleBase_ReferencePoint", 2)]
	[Editor(typeof(RefPointComboBox), typeof(PropertyValueEditor))]
	[CanReferGeometryName]
	public InArgument<string> AngleReferencePoint7
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDeltaY p) => p.AngleReferencePoint7)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDeltaY p) => p.AngleReferencePoint7), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleBase_ReverseAngleReference", 3)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ReverseAngleReference7
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDeltaY p) => p.ReverseAngleReference7)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDeltaY p) => p.ReverseAngleReference7), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleBase_Angle", 4)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> Angle7
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDeltaY p) => p.Angle7)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDeltaY p) => p.Angle7), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleAndDeltaY_DeltaY", 5)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> DeltaY7
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDeltaY p) => p.DeltaY7)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDeltaY p) => p.DeltaY7), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleAndDeltaY_ElevationTarget", 6)]
	[Editor(typeof(TargetComboBox<ElevationTarget>), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	public InArgument<ElevationTarget> ElevationTarget7
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDeltaY p) => p.ElevationTarget7)) as InArgument<ElevationTarget>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDeltaY p) => p.ElevationTarget7), value);
		}
	}

	public AngleAndDeltaY()
	{
		AngleReferencePoint7 = new InArgument<string>
		{
			Expression = GlobleString.String_None
		};
		ReverseAngleReference7 = new InArgument<bool>
		{
			Expression = false
		};
		Angle7 = new InArgument<double>
		{
			Expression = 0.0
		};
		DeltaY7 = new InArgument<double>
		{
			Expression = 0.0
		};
		ElevationTarget7 = new InArgument<ElevationTarget>();
		Init();
	}

	public Point LayoutExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		return InternalExecute(context, geom, fromPoint, side, recordError, SubassemblyRunMode.LayoutMode);
	}

	public Point RoadwayExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		return InternalExecute(context, geom, fromPoint, side, recordError, SubassemblyRunMode.RoadwayMode);
	}

	private Point InternalExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError, SubassemblyRunMode runMode)
	{
		Point point = geom.GetPoint(context.GetValue(AngleReferencePoint7));
		bool value = context.GetValue(ReverseAngleReference7);
		double value2 = context.GetValue(Angle7);
		double offsetY = context.GetValue(DeltaY7);
		double num = value2;
		value2 = GeometryFactory.AdjustAngel_Degree(value2);
		if (Math.Abs(value2 - 0.0) < 1E-05)
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_AngleCannotZero, Resource.PointGeometryType_DisplayName_AngleAndDeltaY, num));
			return null;
		}
		if (Math.Abs(value2 - 180.0) < 1E-05)
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_AngleCannotZero, Resource.PointGeometryType_DisplayName_AngleAndDeltaY, num));
			return null;
		}
		if (runMode == SubassemblyRunMode.RoadwayMode)
		{
			try
			{
				ElevationTarget value3 = context.GetValue(ElevationTarget7);
				if (value3 != null && value3.IsValid && fromPoint != null)
				{
					offsetY = value3.Value - fromPoint.Elevation;
				}
			}
			catch
			{
				recordError.RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetElevationFromElevationTarget);
			}
		}
		GeomPoint geoFromPt = new GeomPoint(fromPoint.X, fromPoint.Y);
		GeomPoint geomPoint;
		if (point == null)
		{
			geomPoint = new GeomPoint(geoFromPt.X + (double)(side * 10), geoFromPt.Y);
		}
		else
		{
			geomPoint = new GeomPoint(point.X, point.Y);
			if (Math.Abs(geoFromPt.Distance(geomPoint)) < 1E-05)
			{
				string msg = string.Format(Resource.ErrorHandling_OverlappedPoints, point.GeometryId, fromPoint.GeometryId);
				recordError.RecordErrorInfo(context, ErrorLevel.Error, msg);
				return null;
			}
		}
		return GeometryFactory.AddPointByAngleElevation(geoFromPt, geomPoint, value2 * (double)side, offsetY, value);
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> result = new List<ValidationError>();
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, Angle7, Resource.DisplayName_AngleBase_Angle, result);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, DeltaY7, Resource.DisplayName_AngleAndDeltaY_DeltaY, result);
		return result;
	}
}
