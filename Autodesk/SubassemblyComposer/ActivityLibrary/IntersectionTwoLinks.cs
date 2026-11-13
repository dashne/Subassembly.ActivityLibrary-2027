using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.GeomMath;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class IntersectionTwoLinks : CreationRuleBase, IIntersectionRule, ICreationRule, ICustomAccessProperty
{
	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_Link1", 2)]
	[Editor(typeof(LinkComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> FirstLink1
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoLinks p) => p.FirstLink1)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoLinks p) => p.FirstLink1), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_Link2", 3)]
	[Editor(typeof(LinkComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> SecondLink1
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoLinks p) => p.SecondLink1)) as InArgument<string>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoLinks p) => p.SecondLink1), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ExtendLink1", 4)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ExtendFirstLink1
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoLinks p) => p.ExtendFirstLink1)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoLinks p) => p.ExtendFirstLink1), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ExtendLink2", 5)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ExtendSecondLink1
	{
		get
		{
			return GetPropertyValue(this.PropertyName((IntersectionTwoLinks p) => p.ExtendSecondLink1)) as InArgument<bool>;
		}
		set
		{
			SetPropertyValue(this.PropertyName((IntersectionTwoLinks p) => p.ExtendSecondLink1), value);
		}
	}

	public IntersectionTwoLinks()
	{
		FirstLink1 = new InArgument<string>
		{
			Expression = ""
		};
		SecondLink1 = new InArgument<string>
		{
			Expression = ""
		};
		ExtendFirstLink1 = new InArgument<bool>
		{
			Expression = false
		};
		ExtendSecondLink1 = new InArgument<bool>
		{
			Expression = false
		};
		Init();
	}

	public List<Point> Execute(CodeActivityContext context, Geometry geom, int side, IRecordError recordError)
	{
		Link link = geom.GetLink(context.GetValue(FirstLink1));
		if (link == null)
		{
			string msg = string.Format(Resource.ErrorHandling_IntersectionTwoLinks_FirstLink1, context.GetValue(FirstLink1));
			recordError.RecordErrorInfo(context, ErrorLevel.Error, msg);
			return null;
		}
		Link link2 = geom.GetLink(context.GetValue(SecondLink1));
		if (link2 == null)
		{
			string msg2 = string.Format(Resource.ErrorHandling_IntersectionTwoLinks_SecondLink1, context.GetValue(SecondLink1));
			recordError.RecordErrorInfo(context, ErrorLevel.Error, msg2);
			return null;
		}
		bool value = context.GetValue(ExtendFirstLink1);
		bool value2 = context.GetValue(ExtendSecondLink1);
		List<Point> list = GeometryFactory.LinkIntersection(link, link2, value, value2, side);
		if (list.Count <= 1)
		{
			return list;
		}
		Point nearestIntersection = GeometryFactory.GetNearestIntersection(link.StartPoint, list);
		return new List<Point> { nearestIntersection };
	}
}
