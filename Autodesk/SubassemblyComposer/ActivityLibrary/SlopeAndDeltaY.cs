using System;
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

internal class SlopeAndDeltaY : CreationRuleBase, IPointRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeAndDeltaY_Slope", 2)]
	[Editor(typeof(TypeInfoTextBox<Grade>), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> Slope5
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.Slope5)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.Slope5), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeAndDeltaY_DeltaY", 3)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> DeltaY5
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.DeltaY5)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.DeltaY5), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeAndDeltaY_ReverseSlopeDirection", 4)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ReverseSlopeDirection5
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.ReverseSlopeDirection5)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.ReverseSlopeDirection5), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeAndDeltaY_OffsetTarget", 5)]
	[CanReferParameterName]
	[Editor(typeof(TargetComboBox<OffsetTarget>), typeof(PropertyValueEditor))]
	public InArgument<OffsetTarget> OffsetTarget5
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.OffsetTarget5)) as InArgument<OffsetTarget>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.OffsetTarget5), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeAndDeltaY_ElevationTarget", 6)]
	[CanReferParameterName]
	[Editor(typeof(TargetComboBox<ElevationTarget>), typeof(PropertyValueEditor))]
	public InArgument<ElevationTarget> ElevationTarget5
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.ElevationTarget5)) as InArgument<ElevationTarget>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.ElevationTarget5), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeAndDeltaY_Superelevation", 7)]
	[CanReferParameterName]
	[Editor(typeof(SuperelevationComboBoxWithNone), typeof(PropertyValueEditor))]
	public InArgument<CrossSegmentType> Superelevation5
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.Superelevation5)) as InArgument<CrossSegmentType>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeAndDeltaY p) => p.Superelevation5), value);
		}
	}

	public SlopeAndDeltaY()
	{
		Slope5 = new InArgument<double>
		{
			Expression = 0.5
		};
		DeltaY5 = new InArgument<double>
		{
			Expression = 10.0
		};
		OffsetTarget5 = new InArgument<OffsetTarget>();
		ElevationTarget5 = new InArgument<ElevationTarget>();
		Superelevation5 = new InArgument<CrossSegmentType>();
		ReverseSlopeDirection5 = new InArgument<bool>
		{
			Expression = false
		};
		Init();
	}

	private bool CalDistance(double slope, double height, bool isRightSide, out double distance)
	{
		distance = 0.0;
		if (slope > 0.0)
		{
			if (height <= 0.0)
			{
				return false;
			}
			distance = Math.Abs(height / slope);
		}
		else if (slope < 0.0)
		{
			if (height >= 0.0)
			{
				return false;
			}
			distance = Math.Abs(height / slope);
		}
		else
		{
			if (height < 0.0 || height > 0.0)
			{
				return false;
			}
			distance = 0.0;
		}
		if (!isRightSide)
		{
			distance = 0.0 - distance;
		}
		return true;
	}

	public Point LayoutExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		double value = context.GetValue(Slope5);
		double value2 = context.GetValue(DeltaY5);
		bool value3 = context.GetValue(ReverseSlopeDirection5);
		bool isRightSide = true;
		if ((side == -1 && !value3) || (side == 1 && value3))
		{
			isRightSide = false;
		}
		double num = value2;
		double distance = 0.0;
		if (!CalDistance(value, num, isRightSide, out distance))
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_CannotComputePointBySlopeAndElevation);
			return null;
		}
		return new Point(fromPoint.X + distance, fromPoint.Y + num);
	}

	public Point RoadwayExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		double slope = context.GetValue(Slope5);
		double value = context.GetValue(DeltaY5);
		OffsetTarget value2 = context.GetValue(OffsetTarget5);
		ElevationTarget value3 = context.GetValue(ElevationTarget5);
		CrossSegmentType value4 = context.GetValue(Superelevation5);
		bool value5 = context.GetValue(ReverseSlopeDirection5);
		bool isRightSide = true;
		if ((side == -1 && !value5) || (side == 1 && value5))
		{
			isRightSide = false;
		}
		if (value4 != CrossSegmentType.None)
		{
			ICrossSlope slopeAt = Superelevation.Data.GetSlopeAt(value4);
			if (slopeAt.IsDefined)
			{
				slope = slopeAt.Slope;
			}
		}
		double num = value;
		double distance = 0.0;
		if (value3 != null && value3.IsValid)
		{
			try
			{
				num = value3.Value - fromPoint.Elevation;
			}
			catch
			{
				recordError.RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetElevationFromElevationTarget);
				num = value;
			}
		}
		bool flag = false;
		if (value2 != null && value2.IsValid)
		{
			try
			{
				distance = value2.Value - fromPoint.Offset;
				flag = true;
			}
			catch
			{
				recordError.RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetOffsetFromOffsetTarget);
			}
		}
		if (!flag && !CalDistance(slope, num, isRightSide, out distance))
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_CannotComputePointBySlopeAndElevation);
			return null;
		}
		return new Point(fromPoint.X + distance, fromPoint.Y + num);
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> result = new List<ValidationError>();
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, Slope5, Resource.DisplayName_SlopeAndDeltaY_Slope, result);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, DeltaY5, Resource.DisplayName_SlopeAndDeltaY_DeltaY, result);
		return result;
	}
}
