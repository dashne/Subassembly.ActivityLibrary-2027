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

internal class SlopeToSurface : CreationRuleBase, IPointRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeToSurface_Slope", 2)]
	[Editor(typeof(TypeInfoTextBox<Grade>), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> Slope3
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeToSurface p) => p.Slope3)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeToSurface p) => p.Slope3), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeToSurface_ReverseSlopeDirection", 3)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ReverseSlopeDirection3
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeToSurface p) => p.ReverseSlopeDirection3)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeToSurface p) => p.ReverseSlopeDirection3), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeToSurface_SurfaceTarget", 4)]
	[Editor(typeof(TargetComboBox<SurfaceTarget>), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	public InArgument<SurfaceTarget> SurfaceTarget3
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeToSurface p) => p.SurfaceTarget3)) as InArgument<SurfaceTarget>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeToSurface p) => p.SurfaceTarget3), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Misc", 100)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SlopeToSurface_DeltaXForLayout", 2)]
	[RequiredArgument]
	[CanReferParameterName]
	public InArgument<double> DeltaXForLayout3
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeToSurface p) => p.DeltaXForLayout3)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeToSurface p) => p.DeltaXForLayout3), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Misc", 100)]
	[CustomDisplayName(typeof(Resource), "Show_Errors", 3)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	public InArgument<bool> ShowErrors
	{
		get
		{
			return GetPropertyValue(this.PropertyName((SlopeToSurface p) => p.ShowErrors)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((SlopeToSurface p) => p.ShowErrors), value);
		}
	}

	public SlopeToSurface()
	{
		Slope3 = new InArgument<double>
		{
			Expression = -0.02
		};
		SurfaceTarget3 = new InArgument<SurfaceTarget>();
		ReverseSlopeDirection3 = new InArgument<bool>
		{
			Expression = false
		};
		DeltaXForLayout3 = new InArgument<double>
		{
			Expression = 5.0
		};
		ShowErrors = new InArgument<bool>
		{
			Expression = true
		};
		Init();
	}

	public Point LayoutExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		double value = context.GetValue(Slope3);
		context.GetValue(SurfaceTarget3);
		bool value2 = context.GetValue(ReverseSlopeDirection3);
		recordError.ShowErrors = context.GetValue(ShowErrors);
		double value3 = context.GetValue(DeltaXForLayout3);
		double num = 1.0;
		if ((side == -1 && !value2) || (side == 1 && value2))
		{
			num = -1.0;
		}
		return new Point(fromPoint.X + value3 * num, fromPoint.Y + Math.Abs(value3) * value);
	}

	public Point RoadwayExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int side, IRecordError recordError)
	{
		double value = context.GetValue(Slope3);
		SurfaceTarget value2 = context.GetValue(SurfaceTarget3);
		bool value3 = context.GetValue(ReverseSlopeDirection3);
		recordError.ShowErrors = context.GetValue(ShowErrors);
		bool isTowardRight = true;
		if ((side == -1 && !value3) || (side == 1 && value3))
		{
			isTowardRight = false;
		}
		Point point = null;
		if (value2 != null && value2.IsValid && fromPoint != null)
		{
			try
			{
				point = value2.getDaylightPoint(fromPoint, value, isTowardRight);
			}
			catch
			{
				recordError.RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_NoIntersectionPtWithSurfaceTarget);
				return null;
			}
		}
		if (point == null)
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_NoIntersectionPtWithSurfaceTarget);
		}
		return point;
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> result = new List<ValidationError>();
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, Slope3, Resource.DisplayName_SlopeToSurface_Slope, result);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, DeltaXForLayout3, Resource.DisplayName_SlopeToSurface_DeltaXForLayout, result);
		return result;
	}
}
