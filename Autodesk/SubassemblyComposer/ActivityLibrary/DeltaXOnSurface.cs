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

internal class DeltaXOnSurface : CreationRuleBase, IPointRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DeltaXOnSurface_DeltaX", 2)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> DeltaX4
	{
		get
		{
			return GetPropertyValue(this.PropertyName((DeltaXOnSurface p) => p.DeltaX4)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((DeltaXOnSurface p) => p.DeltaX4), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DeltaXOnSurface_SurfaceTarget", 3)]
	[Editor(typeof(TargetComboBox<SurfaceTarget>), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	public InArgument<SurfaceTarget> SurfaceTarget4
	{
		get
		{
			return GetPropertyValue(this.PropertyName((DeltaXOnSurface p) => p.SurfaceTarget4)) as InArgument<SurfaceTarget>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((DeltaXOnSurface p) => p.SurfaceTarget4), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DeltaXOnSurface_OffsetTarget", 4)]
	[Editor(typeof(TargetComboBox<OffsetTarget>), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	public InArgument<OffsetTarget> OffsetTarget4
	{
		get
		{
			return GetPropertyValue(this.PropertyName((DeltaXOnSurface p) => p.OffsetTarget4)) as InArgument<OffsetTarget>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((DeltaXOnSurface p) => p.OffsetTarget4), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Misc", 100)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DeltaXOnSurface_DeltaYForLayout", 2)]
	[RequiredArgument]
	[CanReferParameterName]
	public InArgument<double> DeltaYForLayout4
	{
		get
		{
			return GetPropertyValue(this.PropertyName((DeltaXOnSurface p) => p.DeltaYForLayout4)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((DeltaXOnSurface p) => p.DeltaYForLayout4), value);
		}
	}

	public DeltaXOnSurface()
	{
		DeltaX4 = new InArgument<double>
		{
			Expression = 0.0
		};
		SurfaceTarget4 = new InArgument<SurfaceTarget>();
		OffsetTarget4 = new InArgument<OffsetTarget>();
		DeltaYForLayout4 = new InArgument<double>
		{
			Expression = 0.0
		};
		Init();
	}

	public Point LayoutExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		double num = context.GetValue(DeltaX4) * (double)side;
		double value = context.GetValue(DeltaYForLayout4);
		return new Point(fromPoint.X + num, fromPoint.Y + value);
	}

	public Point RoadwayExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		double num = context.GetValue(DeltaX4) * (double)side;
		SurfaceTarget value = context.GetValue(SurfaceTarget4);
		OffsetTarget value2 = context.GetValue(OffsetTarget4);
		Point point = null;
		if (value2 != null && value2.IsValid && fromPoint != null)
		{
			try
			{
				num = value2.Value - fromPoint.Offset;
			}
			catch
			{
				recordError.RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetOffsetFromOffsetTarget);
			}
		}
		if (value != null && value.IsValid && fromPoint != null)
		{
			try
			{
				point = value.getPointOnOffset(fromPoint.Offset + num);
			}
			catch
			{
				recordError.RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_CannotGetElevationFromSurfaceTarget);
				return null;
			}
			if (point == null && fromPoint != null)
			{
				recordError.RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_CannotGetElevationFromSurfaceTarget);
				return null;
			}
			return point;
		}
		recordError.RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_NoSpecifiedSurfaceTarget);
		return null;
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> result = new List<ValidationError>();
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, DeltaX4, Resource.DisplayName_DeltaXOnSurface_DeltaX, result);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, DeltaYForLayout4, Resource.DisplayName_DeltaXOnSurface_DeltaYForLayout, result);
		return result;
	}
}
