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

internal class IntersectionLinkPointSlope : CreationRuleBase, IIntersectionRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_IntersectionLinkPointSlope_FirstLink2", 2)]
	[Editor(typeof(LinkComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> IntersectionLinkPointSlope_FirstLink
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.IntersectionLinkPointSlope_FirstLink)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.IntersectionLinkPointSlope_FirstLink), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_IntersectionLinkPointSlope_Point2", 3)]
	[Editor(typeof(PointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> IntersectionLinkPointSlope_Point
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.IntersectionLinkPointSlope_Point)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.IntersectionLinkPointSlope_Point), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_IntersectionLinkPointSlope_ExtendFirstLink2", 4)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> IntersectionLinkPointSlope_ExtendFirstLink
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.IntersectionLinkPointSlope_ExtendFirstLink)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.IntersectionLinkPointSlope_ExtendFirstLink), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_IntersectionLinkPointSlope_Slope2", 5)]
	[Editor(typeof(TypeInfoTextBox<Grade>), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> Slope2
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.Slope2)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.Slope2), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_IntersectionLinkPointSlope_ExtendSlope", 6)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> IntersectionLinkPointSlope_ExtendSlope
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.IntersectionLinkPointSlope_ExtendSlope)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.IntersectionLinkPointSlope_ExtendSlope), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_IntersectionLinkPointSlope_ReverseSlope", 7)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> IntersectionLinkPointSlope_ReverseSlope
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.IntersectionLinkPointSlope_ReverseSlope)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionLinkPointSlope p) => p.IntersectionLinkPointSlope_ReverseSlope), value);
		}
	}

	public IntersectionLinkPointSlope()
	{
		IntersectionLinkPointSlope_FirstLink = new InArgument<string>
		{
			Expression = ""
		};
		IntersectionLinkPointSlope_Point = new InArgument<string>
		{
			Expression = ""
		};
		IntersectionLinkPointSlope_ExtendFirstLink = new InArgument<bool>
		{
			Expression = false
		};
		Slope2 = new InArgument<double>
		{
			Expression = 0.0
		};
		IntersectionLinkPointSlope_ExtendSlope = new InArgument<bool>
		{
			Expression = false
		};
		IntersectionLinkPointSlope_ReverseSlope = new InArgument<bool>
		{
			Expression = false
		};
		Init();
	}

	public List<Point> Execute(CodeActivityContext context, Geometry geom, int side, IRecordError recordError)
	{
		Link link = geom.GetLink(context.GetValue(IntersectionLinkPointSlope_FirstLink));
		if (link == null)
		{
			string msg = string.Format(Resource.ErrorHandling_IntersectionLinkPointSlope_FirstLink2, context.GetValue(IntersectionLinkPointSlope_FirstLink));
			recordError.RecordErrorInfo(context, ErrorLevel.Error, msg);
			return null;
		}
		Point point = geom.GetPoint(context.GetValue(IntersectionLinkPointSlope_Point));
		if (point == null)
		{
			string msg2 = string.Format(Resource.ErrorHandling_IntersectionLinkPointSlope_Point2, context.GetValue(IntersectionLinkPointSlope_Point));
			recordError.RecordErrorInfo(context, ErrorLevel.Error, msg2);
			return null;
		}
		bool value = context.GetValue(IntersectionLinkPointSlope_ExtendFirstLink);
		double angle = Math.Atan(context.GetValue(Slope2));
		bool value2 = context.GetValue(IntersectionLinkPointSlope_ExtendSlope);
		bool value3 = context.GetValue(IntersectionLinkPointSlope_ReverseSlope);
		if (side == -1)
		{
			angle = GeometryMath.AngleMirrorByY(angle);
		}
		if (value3)
		{
			angle = GeometryMath.AngleMirrorByY(angle);
		}
		List<Point> list = GeometryFactory.LinkIntersection(link, point, angle, value, extendSlope: false);
		if ((list.Count == 0) & value2)
		{
			list = GeometryFactory.LinkIntersection(link, point, angle, value, value2);
		}
		if (list.Count <= 1)
		{
			return list;
		}
		Point nearestIntersection = GeometryFactory.GetNearestIntersection(point, list);
		return new List<Point> { nearestIntersection };
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> result = new List<ValidationError>();
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, Slope2, Resource.DisplayName_IntersectionLinkPointSlope_Slope2, result);
		return result;
	}
}
