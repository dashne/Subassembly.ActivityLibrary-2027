using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(StripSurfaceDesigner))]
[StripSurfaceAutoNumbering]
[ToolboxBitmap(typeof(StripSurfaceActivity), "Resources.Icons.surface_link_create_toolbox.ico")]
public class StripSurfaceActivity : SubassemblyActivity
{
	private InArgument<string> _linkNumber;

	private InArgument<string> startPointName;

	private InArgument<string> endPointName;

	[CustomCategory(typeof(Resource), "Category_Link", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_LinkNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[LinkAutoNumbering("StartPointName", "EndPointName")]
	public InArgument<string> LinkNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.LinkNumber)) as InArgument<string>;
		}
		set
		{
			_linkNumber = PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.LinkNumber)) as InArgument<string>;
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.LinkNumber), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Link", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_LinkCodes", 2)]
	[Editor(typeof(LinkCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferLinkCodeName]
	public List<InArgument<string>> LinkCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.LinkCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.LinkCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StripSurface_SurfaceTarget", 1)]
	[Editor(typeof(TargetComboBox<SurfaceTarget>), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	public InArgument<SurfaceTarget> Surface
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.Surface)) as InArgument<SurfaceTarget>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.Surface), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StripSurface_StartOffset", 2)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> StartOffset
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.StartOffset)) as InArgument<double>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.StartOffset), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StripSurface_EndOffset", 3)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> EndOffset
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.EndOffset)) as InArgument<double>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.EndOffset), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StripSuface_StartOffsetTarget", 4)]
	[Editor(typeof(TargetComboBox<OffsetTarget>), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	public InArgument<OffsetTarget> StartOffsetTarget
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.StartOffsetTarget)) as InArgument<OffsetTarget>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.StartOffsetTarget), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StripSuface_EndOffsetTarget", 5)]
	[Editor(typeof(TargetComboBox<OffsetTarget>), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	public InArgument<OffsetTarget> EndOffsetTarget
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.EndOffsetTarget)) as InArgument<OffsetTarget>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.EndOffsetTarget), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StripSurface_Depth", 6)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> Depth
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.Depth)) as InArgument<double>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.Depth), value);
		}
	}

	[Browsable(false)]
	public InArgument<int> Side
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.Side)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.Side), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_StripSurface_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StripSurface_StartPointName", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[PointAutoNumbering]
	public InArgument<string> StartPointName
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.StartPointName)) as InArgument<string>;
		}
		set
		{
			startPointName = StartPointName;
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.StartPointName), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_StripSurface_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StripSurface_StartPointCodes", 2)]
	[Editor(typeof(PointCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferPointCodeName]
	public List<InArgument<string>> StartPointCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.StartPointCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.StartPointCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_StripSurface_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StripSurface_EndPointName", 3)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[PointAutoNumbering]
	public InArgument<string> EndPointName
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.EndPointName)) as InArgument<string>;
		}
		set
		{
			endPointName = EndPointName;
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.EndPointName), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Misc", 100)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StripSurface_DepthForLayoutMode", 2)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> DepthForLayoutMode
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.DepthForLayoutMode)) as InArgument<double>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.DepthForLayoutMode), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_StripSurface_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StripSurface_EndPointCodes", 4)]
	[Editor(typeof(PointCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferPointCodeName]
	public List<InArgument<string>> EndPointCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((StripSurfaceActivity p) => p.EndPointCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.EndPointCodes), value);
		}
	}

	public StripSurfaceActivity()
	{
		PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.EndPointName), null, EndPointNameOnChanged);
		PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.LinkNumber), null, LinkNumberOnChanged);
		PropertyCollection_Set(this.PropertyName((StripSurfaceActivity p) => p.StartPointName), null, StartPointNameOnChanged);
		Depth = 0.0;
		DepthForLayoutMode = 0.0;
	}

	protected void LinkNumberOnChanged(object value)
	{
		if (value != null && value != _linkNumber)
		{
			InArgument<string> linkNumber = _linkNumber;
			_linkNumber = value as InArgument<string>;
			OnGeometryNameChanged(this.PropertyName((StripSurfaceActivity p) => p.LinkNumber), linkNumber, _linkNumber);
		}
	}

	protected void StartPointNameOnChanged(object value)
	{
		if (value != null && (InArgument<string>)value != startPointName)
		{
			InArgument<string> oldName = startPointName;
			startPointName = (InArgument<string>)value;
			OnGeometryNameChanged(this.PropertyName((StripSurfaceActivity p) => p.StartPointName), oldName, startPointName);
		}
	}

	protected void EndPointNameOnChanged(object value)
	{
		if (value != null && (InArgument<string>)value != endPointName)
		{
			InArgument<string> oldName = endPointName;
			endPointName = (InArgument<string>)value;
			OnGeometryNameChanged(this.PropertyName((StripSurfaceActivity p) => p.EndPointName), oldName, endPointName);
		}
	}

	protected override void LayoutExecute(CodeActivityContext context)
	{
		Geometry value = context.GetValue(Geometry);
		int num = ((Side == null || context.GetValue(Side) != 1) ? 1 : (-1));
		double num2 = context.GetValue(StartOffset) * (double)num;
		double num3 = context.GetValue(EndOffset) * (double)num;
		double value2 = context.GetValue(DepthForLayoutMode);
		if (GeometryUtility.FuzzyEqual(num2, num3))
		{
			RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_StripSurface_Degenerated);
		}
		Autodesk.SubassemblyComposer.WorkflowEngine.Point startPoint = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(num2, 0.0 - value2);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point endPoint = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(num3, 0.0 - value2);
		Link link = new Link(startPoint, endPoint);
		SetProductProperties(ref link, context, value);
	}

	protected override void RoadwayExecute(CodeActivityContext context)
	{
		Geometry value = context.GetValue(Geometry);
		SurfaceTarget value2 = context.GetValue(Surface);
		if (value2 == null || !value2.IsValid)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_StripSurface_SurfaceNotFound);
			return;
		}
		int num = ((Side == null || context.GetValue(Side) != 1) ? 1 : (-1));
		double x = context.GetValue(StartOffset) * (double)num;
		double x2 = context.GetValue(EndOffset) * (double)num;
		TryOffsetTarget(context.GetValue(StartOffsetTarget), ref x, context);
		TryOffsetTarget(context.GetValue(EndOffsetTarget), ref x2, context);
		double value3 = context.GetValue(Depth);
		Link link;
		if (GeometryUtility.FuzzyEqual(x, x2))
		{
			RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_StripSurface_Degenerated);
			link = ProduceDegeneratedGeometry(value2, x, value3);
		}
		else
		{
			link = ProduceGeometry(context, value2, x, x2, value3);
		}
		if (link != null)
		{
			SetProductProperties(ref link, context, value);
		}
	}

	private Link ProduceDegeneratedGeometry(SurfaceTarget surface, double duplicatedX, double depth)
	{
		Autodesk.SubassemblyComposer.WorkflowEngine.Point pointOnOffset = surface.getPointOnOffset(duplicatedX + Autodesk.SubassemblyComposer.WorkflowEngine.Point.Origin.Offset);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(pointOnOffset.X, pointOnOffset.Y);
		if (depth != 0.0)
		{
			pointOnOffset.Y -= depth;
			point.Y -= depth;
		}
		return new Link(pointOnOffset, point);
	}

	private Link ProduceGeometry(CodeActivityContext context, SurfaceTarget surface, double startX, double endX, double depth)
	{
		List<Link> links = surface.sampleSection(startX + Autodesk.SubassemblyComposer.WorkflowEngine.Point.Origin.Offset, endX + Autodesk.SubassemblyComposer.WorkflowEngine.Point.Origin.Offset);
		if (links == null || links.Count == 0)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_StripSurface_CantSample);
			return null;
		}
		if (depth != 0.0)
		{
			GeometryUtility.OffsetLinks(ref links, depth);
		}
		return CombineLinks(links);
	}

	private void SetProductProperties(ref Link link, CodeActivityContext context, Geometry geometry)
	{
		if (IsCreatingAux)
		{
			foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point3 in link.Points)
			{
				point3.IsAuxiliary = true;
			}
		}
		link.ActivityId = context.GetValue(ActivityId);
		link.GeometryId = context.GetValue(LinkNumber);
		link.Codes = GetCodes(context, LinkCodes);
		link.IsAuxiliary = IsCreatingAux;
		if (ActivityDesigntime.Instance != null)
		{
			link.DisplayName = DisplayName;
		}
		geometry.Links.Add(link);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point = link.Points.First();
		point.ActivityId = context.GetValue(ActivityId);
		point.GeometryId = context.GetValue(StartPointName);
		point.Codes = GetCodes(context, StartPointCodes);
		if (ActivityDesigntime.Instance != null)
		{
			point.DisplayName = DisplayName;
		}
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point2 = link.Points.Last();
		point2.ActivityId = context.GetValue(ActivityId);
		point2.GeometryId = context.GetValue(EndPointName);
		point2.Codes = GetCodes(context, EndPointCodes);
		if (ActivityDesigntime.Instance != null)
		{
			point2.DisplayName = DisplayName;
		}
		geometry.Points.AddRange(link.Points);
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
		BindCodeArguments(metadata, LinkCodes, this.PropertyName((StripSurfaceActivity p) => p.LinkCodes));
		BindCodeArguments(metadata, StartPointCodes, this.PropertyName((StripSurfaceActivity p) => p.StartPointCodes));
		BindCodeArguments(metadata, EndPointCodes, this.PropertyName((StripSurfaceActivity p) => p.EndPointCodes));
		ValidateCodeArguments(metadata, LinkCodes);
		ValidateCodeArguments(metadata, StartPointCodes);
		ValidateCodeArguments(metadata, EndPointCodes);
	}

	private void TryOffsetTarget(OffsetTarget offsetTarget, ref double x, CodeActivityContext context)
	{
		if (offsetTarget != null && offsetTarget.IsValid)
		{
			try
			{
				x = offsetTarget.Value - Autodesk.SubassemblyComposer.WorkflowEngine.Point.Origin.Offset;
			}
			catch
			{
				RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetOffsetFromOffsetTarget);
			}
		}
	}

	private Link CombineLinks(List<Link> links)
	{
		if (links.Count == 1)
		{
			return links[0];
		}
		Link link = new Link();
		for (int i = 0; i < links.Count; i++)
		{
			Link link2 = links[i];
			if (i == 0)
			{
				link.Points.Add(link2.Points[0]);
				link.Points.Add(link2.Points[1]);
			}
			else
			{
				link.Points.Add(link2.Points[1]);
			}
		}
		return link;
	}

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		return new List<ValidationError>();
	}

	public override bool GeneratesPoint(string pointNumber)
	{
		try
		{
			return ((Literal<string>)StartPointName.Expression).Value == pointNumber || ((Literal<string>)EndPointName.Expression).Value == pointNumber;
		}
		catch
		{
			return false;
		}
	}
}
