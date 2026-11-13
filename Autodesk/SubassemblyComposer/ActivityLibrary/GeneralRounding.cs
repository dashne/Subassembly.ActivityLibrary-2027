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

internal abstract class GeneralRounding : CreationRuleBase, IDaylightRoundRule, ICreationRule, ICustomAccessProperty
{
	[RequiredArgument]
	[Editor(typeof(RoundByCombobox), typeof(PropertyValueEditor))]
	[CustomCategory(typeof(Resource), "Category_GeometryType", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DaylightRound_RoundBy", 2)]
	public InArgument<RoundByEnum> RoundBy
	{
		get
		{
			return GetPropertyValue(this.PropertyName((GeneralRounding p) => p.RoundBy)) as InArgument<RoundByEnum>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((GeneralRounding p) => p.RoundBy), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryType", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DaylightRound_RoundParameter", 3)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> RoundParameter
	{
		get
		{
			return GetPropertyValue(this.PropertyName((GeneralRounding p) => p.RoundParameter)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((GeneralRounding p) => p.RoundParameter), value);
		}
	}

	protected GeneralRounding()
	{
		RoundBy = new InArgument<RoundByEnum>
		{
			Expression = RoundByEnum.ByRadius
		};
		RoundParameter = new InArgument<double>
		{
			Expression = 3.0
		};
		Init();
	}

	public Link Execute(CodeActivityContext context, Point ptOutSurface, Point ptOnSurface, SurfaceTarget surface, int sampleCount, ref GeometryUtility.RoundResult result)
	{
		if (ptOutSurface == null || ptOnSurface == null || surface == null)
		{
			throw new ArgumentNullException();
		}
		RoundByEnum value = context.GetValue(RoundBy);
		double value2 = context.GetValue(RoundParameter);
		double num = ((ptOutSurface.Offset < ptOnSurface.Offset) ? 1.0 : (-1.0));
		List<Link> list = surface.sampleSection(ptOnSurface.Offset, ptOnSurface.Offset + num * value2);
		if (list == null || list.Count == 0)
		{
			return null;
		}
		bool roundByLenth = value == RoundByEnum.ByLength;
		List<Point> curveRet = new List<Point>();
		result = GetRoundingCurve(ptOnSurface, ptOutSurface, list[0].Points[1], roundByLenth, value2, sampleCount, ref curveRet);
		if (curveRet == null || curveRet.Count == 0)
		{
			return null;
		}
		Link link = new Link();
		link.Points.AddRange(curveRet);
		return link;
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> list = new List<ValidationError>();
		if (RoundBy == null || RoundBy.Expression == null)
		{
			string message = string.Format(Resource.Message_Validate_NotSupplied, "RoundBy");
			list.Add(new ValidationError(message, isWarning: false, "RoundBy"));
		}
		if (RoundParameter == null || RoundParameter.Expression == null)
		{
			string message2 = string.Format(Resource.Message_Validate_NotSupplied, "RoundParameter");
			list.Add(new ValidationError(message2, isWarning: false, "RoundParameter"));
		}
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, RoundParameter, Resource.DisplayName_DaylightRound_RoundParameter, list);
		return list;
	}

	protected abstract GeometryUtility.RoundResult GetRoundingCurve(Point cornerPt, Point pt1, Point pt2, bool roundByLenth, double roundParam, int sampleCount, ref List<Point> curveRet);
}
