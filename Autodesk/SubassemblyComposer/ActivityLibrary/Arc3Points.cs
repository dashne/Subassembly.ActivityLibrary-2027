using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.GeomMath;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class Arc3Points : CreationRuleBase, ICurveRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ArcPoint", 3)]
	[Editor(typeof(PointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> ArcPoint2
	{
		get
		{
			return GetPropertyValue(this.PropertyName((Arc3Points p) => p.ArcPoint2)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((Arc3Points p) => p.ArcPoint2), value);
		}
	}

	public Arc3Points()
	{
		ArcPoint2 = new InArgument<string>
		{
			Expression = ""
		};
		Init();
	}

	public Link Execute(CodeActivityContext context, Geometry geometry, Point startPoint, Point endPoint, int side, int noIncrements, IRecordError recordError)
	{
		Point point = geometry.GetPoint(context.GetValue(ArcPoint2));
		if (point == null)
		{
			recordError.RecordErrorInfo(context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_Arc3Points_ArcPoint, context.GetValue(ArcPoint2)));
			return null;
		}
		if (side == 1)
		{
			return GeometryFactory.AddArc3Points(startPoint, endPoint, point, noIncrements);
		}
		return GeometryFactory.AddArc3Points(endPoint, startPoint, point, noIncrements);
	}
}
