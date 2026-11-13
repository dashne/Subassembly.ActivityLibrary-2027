using System.Activities;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.GeomMath;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class ParabolaGeneral : CreationRuleBase, ICurveRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_MidOrdinate", 11)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> MidOrdinate
	{
		get
		{
			return GetPropertyValue(this.PropertyName((ParabolaGeneral p) => p.MidOrdinate)) as InArgument<double>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((ParabolaGeneral p) => p.MidOrdinate), value);
		}
	}

	public ParabolaGeneral()
	{
		MidOrdinate = new InArgument<double>
		{
			Expression = 0.0
		};
		Init();
	}

	public Link Execute(CodeActivityContext context, Geometry geometry, Point startPoint, Point endPoint, int side, int noIncrements, IRecordError recordError)
	{
		double value = context.GetValue(MidOrdinate);
		if (value <= 0.0)
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_ParabolaGeneral_MidOrdinate);
			return null;
		}
		if (side == 1)
		{
			return GeometryFactory.AddParabolaGeneral(startPoint, endPoint, value, noIncrements);
		}
		return GeometryFactory.AddParabolaGeneral(endPoint, startPoint, value, noIncrements);
	}

	public override List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		List<ValidationError> result = new List<ValidationError>();
		Collection<string> outputPointNames = null;
		Collection<string> outputLinkNames = null;
		geometryNamesUppCase(validPointNames, validLinkNames, ref outputPointNames, ref outputLinkNames);
		validateGeometryNameInExpression(validate, outputPointNames, outputLinkNames, MidOrdinate, Resource.DisplayName_MidOrdinate, result);
		return result;
	}
}
