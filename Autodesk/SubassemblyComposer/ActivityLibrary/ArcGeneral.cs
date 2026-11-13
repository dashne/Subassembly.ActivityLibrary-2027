using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.GeomMath;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class ArcGeneral : CreationRuleBase, ICurveRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_CenterPoint", 3)]
	[Editor(typeof(PointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> CenterPoint
	{
		get
		{
			return GetPropertyValue(this.PropertyName((ArcGeneral p) => p.CenterPoint)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((ArcGeneral p) => p.CenterPoint), value);
		}
	}

	public ArcGeneral()
	{
		CenterPoint = new InArgument<string>
		{
			Expression = ""
		};
		Init();
	}

	public Link Execute(CodeActivityContext context, Geometry geometry, Point startPoint, Point endPoint, int side, int noIncrements, IRecordError recordError)
	{
		Point point = geometry.GetPoint(context.GetValue(CenterPoint));
		if (point == null)
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_ArcGeneral_CenterPoint, context.GetValue(CenterPoint)));
			return null;
		}
		if (side == 1)
		{
			return GeometryFactory.AddArcGeneral(point, startPoint, endPoint, noIncrements);
		}
		return GeometryFactory.AddArcGeneral(point, endPoint, startPoint, noIncrements);
	}
}
