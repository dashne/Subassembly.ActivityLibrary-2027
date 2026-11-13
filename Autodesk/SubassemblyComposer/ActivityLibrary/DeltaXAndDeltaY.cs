using System.Activities;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class DeltaXAndDeltaY : CreationRuleBase, IPointRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DeltaXAndDeltaY_DeltaX", 2)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> DeltaX1
	{
		get
		{
			return GetPropertyValue(this.PropertyName((DeltaXAndDeltaY p) => p.DeltaX1)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((DeltaXAndDeltaY p) => p.DeltaX1), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DeltaXAndDeltaY_DeltaY", 3)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> DeltaY1
	{
		get
		{
			return GetPropertyValue(this.PropertyName((DeltaXAndDeltaY p) => p.DeltaY1)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((DeltaXAndDeltaY p) => p.DeltaY1), value);
		}
	}

	public DeltaXAndDeltaY()
	{
		DeltaX1 = new InArgument<double>
		{
			Expression = 0.0
		};
		DeltaY1 = new InArgument<double>
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
		double num = (double)side * context.GetValue(DeltaX1);
		double value = context.GetValue(DeltaY1);
		return new Point(fromPoint.X + num, fromPoint.Y + value);
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> result = new List<ValidationError>();
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, DeltaX1, Resource.DisplayName_DeltaXAndDeltaY_DeltaX, result);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, DeltaY1, Resource.DisplayName_DeltaXAndDeltaY_DeltaY, result);
		return result;
	}
}
