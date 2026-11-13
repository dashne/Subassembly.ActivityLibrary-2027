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

internal class SlopeAndDeltaX : CreationRuleBase, IPointRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeAndDeltaX_Slope", 2)]
	[Editor(typeof(TypeInfoTextBox<Grade>), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> Slope2
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeAndDeltaX p) => p.Slope2)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeAndDeltaX p) => p.Slope2), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeAndDeltaX_DeltaX", 3)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> DeltaX2
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeAndDeltaX p) => p.DeltaX2)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeAndDeltaX p) => p.DeltaX2), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeAndDeltaX_OffsetTarget", 4)]
	[Editor(typeof(TargetComboBox<OffsetTarget>), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	public InArgument<OffsetTarget> OffsetTarget2
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeAndDeltaX p) => p.OffsetTarget2)) as InArgument<OffsetTarget>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeAndDeltaX p) => p.OffsetTarget2), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeAndDeltaX_ElevationTarget", 5)]
	[Editor(typeof(TargetComboBox<ElevationTarget>), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	public InArgument<ElevationTarget> ElevationTarget2
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeAndDeltaX p) => p.ElevationTarget2)) as InArgument<ElevationTarget>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeAndDeltaX p) => p.ElevationTarget2), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeAndDeltaX_Superelevation", 6)]
	[Editor(typeof(SuperelevationComboBoxWithNone), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	public InArgument<CrossSegmentType> Superelevation2
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeAndDeltaX p) => p.Superelevation2)) as InArgument<CrossSegmentType>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeAndDeltaX p) => p.Superelevation2), value);
		}
	}

	public SlopeAndDeltaX()
	{
		Slope2 = new InArgument<double>
		{
			Expression = -0.02
		};
		DeltaX2 = new InArgument<double>
		{
			Expression = 10.0
		};
		OffsetTarget2 = new InArgument<OffsetTarget>();
		ElevationTarget2 = new InArgument<ElevationTarget>();
		Superelevation2 = new InArgument<CrossSegmentType>();
		Init();
	}

	public Point LayoutExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		double value = context.GetValue(Slope2);
		double value2 = context.GetValue(DeltaX2);
		if (double.IsInfinity(value))
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_CreatePoint_InfiniteSlope);
			return null;
		}
		double num = (double)side * value2;
		double num2 = Math.Abs(num) * value;
		return new Point(fromPoint.X + num, fromPoint.Y + num2);
	}

	public Point RoadwayExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		double num = context.GetValue(Slope2);
		double value = context.GetValue(DeltaX2);
		OffsetTarget value2 = context.GetValue(OffsetTarget2);
		ElevationTarget value3 = context.GetValue(ElevationTarget2);
		CrossSegmentType value4 = context.GetValue(Superelevation2);
		if (value4 != CrossSegmentType.None)
		{
			ICrossSlope slopeAt = Superelevation.Data.GetSlopeAt(value4);
			if (slopeAt.IsDefined)
			{
				num = slopeAt.Slope;
			}
		}
		if (double.IsInfinity(num))
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_CreatePoint_InfiniteSlope);
			return null;
		}
		double num2 = (double)side * value;
		double num3 = Math.Abs(num2) * num;
		if (value2 != null && value2.IsValid)
		{
			try
			{
				num2 = value2.Value - fromPoint.Offset;
				num3 = Math.Abs(num2) * num;
			}
			catch
			{
				recordError.RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetOffsetFromOffsetTarget);
				num2 = (double)side * value;
			}
		}
		if (value3 != null && value3.IsValid)
		{
			try
			{
				num3 = value3.Value - fromPoint.Elevation;
			}
			catch
			{
				recordError.RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetElevationFromElevationTarget);
				num3 = Math.Abs(num2) * num;
			}
		}
		return new Point(fromPoint.X + num2, fromPoint.Y + num3);
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> result = new List<ValidationError>();
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, Slope2, Resource.DisplayName_SlopeAndDeltaX_Slope, result);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, DeltaX2, Resource.DisplayName_SlopeAndDeltaX_DeltaX, result);
		return result;
	}
}
