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

internal class AngleAndDeltaX : CreationRuleBase, IPointRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleBase_ReferencePoint", 2)]
	[Editor(typeof(RefPointComboBox), typeof(PropertyValueEditor))]
	[CanReferGeometryName]
	public InArgument<string> AngleReferencePoint6
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDeltaX p) => p.AngleReferencePoint6)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDeltaX p) => p.AngleReferencePoint6), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleBase_ReverseAngleReference", 3)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ReverseAngleReference6
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDeltaX p) => p.ReverseAngleReference6)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDeltaX p) => p.ReverseAngleReference6), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleBase_Angle", 4)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> Angle6
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDeltaX p) => p.Angle6)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDeltaX p) => p.Angle6), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleAndDeltaX_DeltaX", 5)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> DeltaX6
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDeltaX p) => p.DeltaX6)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDeltaX p) => p.DeltaX6), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AngleAndDeltaX_OffsetTarget", 6)]
	[Editor(typeof(TargetComboBox<OffsetTarget>), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	public InArgument<OffsetTarget> OffsetTarget6
	{
		get
		{
			return GetPropertyValue(this.PropertyName((AngleAndDeltaX p) => p.OffsetTarget6)) as InArgument<OffsetTarget>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((AngleAndDeltaX p) => p.OffsetTarget6), value);
		}
	}

	public AngleAndDeltaX()
	{
		AngleReferencePoint6 = new InArgument<string>
		{
			Expression = GlobleString.String_None
		};
		ReverseAngleReference6 = new InArgument<bool>
		{
			Expression = false
		};
		Angle6 = new InArgument<double>
		{
			Expression = 0.0
		};
		DeltaX6 = new InArgument<double>
		{
			Expression = 0.0
		};
		OffsetTarget6 = new InArgument<OffsetTarget>();
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
		Point point = geom.GetPoint(context.GetValue(AngleReferencePoint6));
		bool value = context.GetValue(ReverseAngleReference6);
		double value2 = context.GetValue(Angle6);
		double offsetX = context.GetValue(DeltaX6);
		double num = value2;
		value2 = GeometryFactory.AdjustAngel_Degree(value2);
		if (Math.Abs(value2 - 90.0) < 1E-05)
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_AngleCannotZero, Resource.PointGeometryType_DisplayName_AngleAndDeltaX, num));
			return null;
		}
		if (Math.Abs(value2 - 270.0) < 1E-05)
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_AngleCannotZero, Resource.PointGeometryType_DisplayName_AngleAndDeltaX, num));
			return null;
		}
		if (runMode == SubassemblyRunMode.RoadwayMode)
		{
			try
			{
				OffsetTarget value3 = context.GetValue(OffsetTarget6);
				if (value3 != null && value3.IsValid && fromPoint != null)
				{
					offsetX = value3.Value - fromPoint.Offset;
				}
			}
			catch
			{
				recordError.RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetOffsetFromOffsetTarget);
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
		return GeometryFactory.AddPointByAngleOffset(geoFromPt, geomPoint, value2 * (double)side, offsetX, value);
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> result = new List<ValidationError>();
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, Angle6, Resource.DisplayName_AngleBase_Angle, result);
		return result;
	}
}
