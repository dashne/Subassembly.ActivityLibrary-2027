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

internal class IntersectionTwoPointsSlope : CreationRuleBase, IIntersectionRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_IntersectionTwoPointsSlope_Point1", 2)]
	[Editor(typeof(PointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> IntersectionTwoPointsSlope_Point1
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_Point1)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_Point1), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_IntersectionTwoPointsSlope_Slope1", 3)]
	[Editor(typeof(TypeInfoTextBox<Grade>), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> IntersectionTwoPointsSlope_Slope1
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_Slope1)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_Slope1), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_IntersectionTwoPointsSlope_Point2", 4)]
	[Editor(typeof(PointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> IntersectionTwoPointsSlope_Point2
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_Point2)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_Point2), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_IntersectionTwoPointsSlope_Slope2", 5)]
	[Editor(typeof(TypeInfoTextBox<Grade>), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> IntersectionTwoPointsSlope_Slope2
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_Slope2)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_Slope2), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ExtendSlope1", 6)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> IntersectionTwoPointsSlope_ExtendSlope1
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_ExtendSlope1)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_ExtendSlope1), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ExtendSlope2", 7)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> IntersectionTwoPointsSlope_ExtendSlope2
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_ExtendSlope2)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_ExtendSlope2), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ReverseSlope1", 8)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> IntersectionTwoPointsSlope_ReverseSlope1
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_ReverseSlope1)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_ReverseSlope1), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ReverseSlope2", 9)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> IntersectionTwoPointsSlope_ReverseSlope2
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_ReverseSlope2)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoPointsSlope p) => p.IntersectionTwoPointsSlope_ReverseSlope2), value);
		}
	}

	public IntersectionTwoPointsSlope()
	{
		IntersectionTwoPointsSlope_Point1 = new InArgument<string>
		{
			Expression = ""
		};
		IntersectionTwoPointsSlope_Slope1 = new InArgument<double>
		{
			Expression = 0.0
		};
		IntersectionTwoPointsSlope_Point2 = new InArgument<string>
		{
			Expression = ""
		};
		IntersectionTwoPointsSlope_Slope2 = new InArgument<double>
		{
			Expression = 0.0
		};
		IntersectionTwoPointsSlope_ExtendSlope1 = new InArgument<bool>
		{
			Expression = false
		};
		IntersectionTwoPointsSlope_ExtendSlope2 = new InArgument<bool>
		{
			Expression = false
		};
		IntersectionTwoPointsSlope_ReverseSlope1 = new InArgument<bool>
		{
			Expression = false
		};
		IntersectionTwoPointsSlope_ReverseSlope2 = new InArgument<bool>
		{
			Expression = false
		};
		Init();
	}

	public List<Point> Execute(CodeActivityContext context, Geometry geom, int side, IRecordError recordError)
	{
		Point point = geom.GetPoint(context.GetValue(IntersectionTwoPointsSlope_Point1));
		if (point == null)
		{
			string msg = string.Format(Resource.ErrorHandling_IntersectionTwoPointsSlope_Point1, context.GetValue(IntersectionTwoPointsSlope_Point1));
			recordError.RecordErrorInfo(context, ErrorLevel.Error, msg);
			return null;
		}
		Point point2 = geom.GetPoint(context.GetValue(IntersectionTwoPointsSlope_Point2));
		if (point2 == null)
		{
			string msg2 = string.Format(Resource.ErrorHandling_IntersectionTwoPointsSlope_Point2, context.GetValue(IntersectionTwoPointsSlope_Point2));
			recordError.RecordErrorInfo(context, ErrorLevel.Error, msg2);
			return null;
		}
		if (point == point2)
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_IntersectionTwoPointsSlope_SamePoint);
			return null;
		}
		double num = Math.Atan(context.GetValue(IntersectionTwoPointsSlope_Slope1));
		double num2 = Math.Atan(context.GetValue(IntersectionTwoPointsSlope_Slope2));
		bool value = context.GetValue(IntersectionTwoPointsSlope_ExtendSlope1);
		bool value2 = context.GetValue(IntersectionTwoPointsSlope_ExtendSlope2);
		bool value3 = context.GetValue(IntersectionTwoPointsSlope_ReverseSlope1);
		bool value4 = context.GetValue(IntersectionTwoPointsSlope_ReverseSlope2);
		if (side == -1)
		{
			num = GeometryMath.AngleMirrorByY(num);
			num2 = GeometryMath.AngleMirrorByY(num2);
		}
		if (value3)
		{
			num = GeometryMath.AngleMirrorByY(num);
		}
		if (value4)
		{
			num2 = GeometryMath.AngleMirrorByY(num2);
		}
		List<Point> list = GeometryFactory.LinkIntersection(point, num, point2, num2, extendSlope1: false, extendSlope2: false);
		if (list.Count == 0 && (value | value2))
		{
			list = GeometryFactory.LinkIntersection(point, num, point2, num2, value, value2);
		}
		return list;
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> result = new List<ValidationError>();
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, IntersectionTwoPointsSlope_Slope1, Resource.DisplayName_IntersectionTwoPointsSlope_Slope1, result);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, IntersectionTwoPointsSlope_Slope2, Resource.DisplayName_IntersectionTwoPointsSlope_Slope2, result);
		return result;
	}
}
