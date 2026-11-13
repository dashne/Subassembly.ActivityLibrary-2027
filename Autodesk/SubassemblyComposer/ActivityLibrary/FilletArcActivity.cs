using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(FilletArcDesigner))]
[FilletArcAutoNumbering]
[ToolboxBitmap(typeof(FilletArcActivity), "Resources.Icons.fillet_arc_toolbox.ico")]
public class FilletArcActivity : SubassemblyActivity, IDesigntimeValidate, ICustomAccessProperty
{
	private const int DefaultTessellation = 8;

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
			return PropertyCollection_Get(this.PropertyName((FilletArcActivity p) => p.LinkNumber)) as InArgument<string>;
		}
		set
		{
			_linkNumber = LinkNumber;
			PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.LinkNumber), value);
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
			return PropertyCollection_Get(this.PropertyName((FilletArcActivity p) => p.LinkCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.LinkCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_FilletArc_GeometryProperties", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FilletArc_FirstLink", 1)]
	[Editor(typeof(LinkComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> FirstLink
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((FilletArcActivity p) => p.FirstLink)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.FirstLink), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_FilletArc_GeometryProperties", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FilletArc_SecondLink", 2)]
	[Editor(typeof(LinkComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> SecondLink
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((FilletArcActivity p) => p.SecondLink)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.SecondLink), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_FilletArc_GeometryProperties", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FilletArc_Tessellation", 3)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	[TesselationDependence]
	public InArgument<int> Tessellation
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((FilletArcActivity p) => p.Tessellation)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.Tessellation), value);
		}
	}

	[RequiredArgument]
	[Editor(typeof(RoundByCombobox), typeof(PropertyValueEditor))]
	[CustomCategory(typeof(Resource), "Category_FilletArc_GeometryProperties", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FilletArc_RoundBy", 4)]
	public InArgument<RoundByEnum> RoundBy
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((FilletArcActivity p) => p.RoundBy)) as InArgument<RoundByEnum>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.RoundBy), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_FilletArc_GeometryProperties", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FilletArc_RoundParameter", 5)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> RoundParameter
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((FilletArcActivity p) => p.RoundParameter)) as InArgument<double>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.RoundParameter), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_FilletArc_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FilletArc_StartPointName", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[PointAutoNumbering]
	public InArgument<string> StartPointName
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((FilletArcActivity p) => p.StartPointName)) as InArgument<string>;
		}
		set
		{
			startPointName = StartPointName;
			PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.StartPointName), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_FilletArc_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FilletArc_StartPointCode", 2)]
	[Editor(typeof(PointCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferPointCodeName]
	public List<InArgument<string>> StartPointCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((FilletArcActivity p) => p.StartPointCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.StartPointCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_FilletArc_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FilletArc_EndPointName", 3)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[PointAutoNumbering]
	public InArgument<string> EndPointName
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((FilletArcActivity p) => p.EndPointName)) as InArgument<string>;
		}
		set
		{
			endPointName = EndPointName;
			PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.EndPointName), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_FilletArc_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FilletArc_EndPointCode", 4)]
	[Editor(typeof(PointCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferPointCodeName]
	public List<InArgument<string>> EndPointCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((FilletArcActivity p) => p.EndPointCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.EndPointCodes), value);
		}
	}

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = new List<ValidationError>();
		if (LinkNumber != null && LinkNumber.Expression != null)
		{
			string value = (LinkNumber.Expression as Literal<string>).Value;
			if (!GeomertyNameCheckHelper.Execute(ActivityDesigntime.Instance.CodePathTree, value, activityId))
			{
				list.Add(new ValidationError(Resource.Message_Validate_Duplicate_Name, isWarning: false, "GeometryName"));
			}
		}
		ObservableCollection<string> linkNames = ValidGeometryNamesHelper.GetLinkNames(ActivityDesigntime.Instance.CodePathTree, activityId);
		if (!this.LinkArgumentValidate(FirstLink, linkNames))
		{
			string message = string.Format(Resource.Message_Validate_Invalid_Value, Resource.DisplayName_FilletArc_FirstLink);
			list.Add(new ValidationError(message, isWarning: false, "FirstLink"));
		}
		if (!this.LinkArgumentValidate(SecondLink, linkNames))
		{
			string message2 = string.Format(Resource.Message_Validate_Invalid_Value, Resource.DisplayName_FilletArc_SecondLink);
			list.Add(new ValidationError(message2, isWarning: false, "SecondLink"));
		}
		return list;
	}

	protected void LinkNumberOnChanged(object value)
	{
		if (value != null && (InArgument<string>)value != _linkNumber)
		{
			InArgument<string> linkNumber = _linkNumber;
			_linkNumber = value as InArgument<string>;
			OnGeometryNameChanged(this.PropertyName((FilletArcActivity p) => p.LinkNumber), linkNumber, _linkNumber);
		}
	}

	protected void StartPointNameOnChanged(object value)
	{
		if (value != null && (InArgument<string>)value != startPointName)
		{
			InArgument<string> oldName = startPointName;
			startPointName = (InArgument<string>)value;
			OnGeometryNameChanged(this.PropertyName((FilletArcActivity p) => p.StartPointName), oldName, startPointName);
		}
	}

	protected void EndPointNameOnChanged(object value)
	{
		if (value != null && (InArgument<string>)value != endPointName)
		{
			InArgument<string> oldName = endPointName;
			endPointName = (InArgument<string>)value;
			OnGeometryNameChanged(this.PropertyName((FilletArcActivity p) => p.EndPointName), oldName, endPointName);
		}
	}

	public FilletArcActivity()
	{
		Tessellation = new InArgument<int>
		{
			Expression = 8
		};
		RoundBy = new InArgument<RoundByEnum>
		{
			Expression = RoundByEnum.ByRadius
		};
		RoundParameter = new InArgument<double>
		{
			Expression = 3.0
		};
		PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.LinkNumber), _linkNumber, LinkNumberOnChanged);
		PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.EndPointName), null, EndPointNameOnChanged);
		PropertyCollection_Set(this.PropertyName((FilletArcActivity p) => p.StartPointName), null, StartPointNameOnChanged);
	}

	private bool IsAngleFormed(Link link1, Link link2, out Autodesk.SubassemblyComposer.WorkflowEngine.Point vertex, out Autodesk.SubassemblyComposer.WorkflowEngine.Point pt1, out Autodesk.SubassemblyComposer.WorkflowEngine.Point pt2)
	{
		if (link1.Points.Count != 2 || link2.Points.Count != 2)
		{
			vertex = null;
			pt1 = null;
			pt2 = null;
			return false;
		}
		if (link1.StartPoint.GeometryId == link2.StartPoint.GeometryId)
		{
			vertex = link1.StartPoint;
			pt1 = link1.EndPoint;
			pt2 = link2.EndPoint;
			return true;
		}
		if (link1.StartPoint.GeometryId == link2.EndPoint.GeometryId)
		{
			vertex = link1.StartPoint;
			pt1 = link1.EndPoint;
			pt2 = link2.StartPoint;
			return true;
		}
		if (link1.EndPoint.GeometryId == link2.StartPoint.GeometryId)
		{
			vertex = link1.EndPoint;
			pt1 = link1.StartPoint;
			pt2 = link2.EndPoint;
			return true;
		}
		if (link1.EndPoint.GeometryId == link2.EndPoint.GeometryId)
		{
			vertex = link1.EndPoint;
			pt1 = link1.StartPoint;
			pt2 = link2.StartPoint;
			return true;
		}
		vertex = null;
		pt1 = null;
		pt2 = null;
		return false;
	}

	protected override void LayoutExecute(CodeActivityContext context)
	{
		InternalExecute(context);
	}

	protected override void RoadwayExecute(CodeActivityContext context)
	{
		InternalExecute(context);
	}

	private void InternalExecute(CodeActivityContext context)
	{
		Geometry value = context.GetValue(Geometry);
		string value2 = context.GetValue(FirstLink);
		Link link = value.GetLink(value2);
		string value3 = context.GetValue(SecondLink);
		Link link2 = value.GetLink(value3);
		if (link == null || link2 == null)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_FilletArc_LinkNotExist);
			return;
		}
		if (!IsAngleFormed(link, link2, out var vertex, out var pt, out var pt2))
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_FilletArc_CantFillet);
			return;
		}
		bool roundByLenth = context.GetValue(RoundBy) == RoundByEnum.ByLength;
		double value4 = context.GetValue(RoundParameter);
		int num = context.GetValue(Tessellation);
		if (num > 100)
		{
			num = 100;
			RecordErrorInfo(context, ErrorLevel.Warning, string.Format(Resource.ErrorHandling_TesselationTooLarge, 100));
		}
		else if (num < 2)
		{
			num = 2;
			RecordErrorInfo(context, ErrorLevel.Warning, string.Format(Resource.ErrorHandling_TesselationTooSmall, 2));
		}
		List<Autodesk.SubassemblyComposer.WorkflowEngine.Point> curveRet = new List<Autodesk.SubassemblyComposer.WorkflowEngine.Point>();
		switch (GeometryUtility.GetRoundingArc(vertex, pt, pt2, roundByLenth, value4, num + 1, ref curveRet))
		{
		case GeometryUtility.RoundResult.Failed:
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_FilletArc_FilletFailed);
			return;
		case GeometryUtility.RoundResult.InvalidParam:
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_RoundDaylight_InvalidParameter);
			return;
		case GeometryUtility.RoundResult.AdaptToPoint1:
			RecordErrorInfo(context, ErrorLevel.Warning, string.Format(Resource.ErrorHandling_FilletArc_AdaptToLink, value2));
			break;
		case GeometryUtility.RoundResult.AdaptToPoint2:
			RecordErrorInfo(context, ErrorLevel.Warning, string.Format(Resource.ErrorHandling_FilletArc_AdaptToLink, value3));
			break;
		}
		Link link3 = new Link();
		link3.Points.AddRange(curveRet);
		SetProductProperties(ref link3, context, value);
	}

	private void SetProductProperties(ref Link link, CodeActivityContext context, Geometry geometry)
	{
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
		point.IsAuxiliary = IsCreatingAux;
		if (ActivityDesigntime.Instance != null)
		{
			point.DisplayName = DisplayName;
		}
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point2 = link.Points.Last();
		point2.ActivityId = context.GetValue(ActivityId);
		point2.GeometryId = context.GetValue(EndPointName);
		point2.Codes = GetCodes(context, EndPointCodes);
		point2.IsAuxiliary = IsCreatingAux;
		if (ActivityDesigntime.Instance != null)
		{
			point2.DisplayName = DisplayName;
		}
		geometry.Points.AddRange(link.Points);
	}

	public override PropertyDescriptorCollection GetProperties()
	{
		return CreationRuleHelper.GetProperties(this, this);
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
		BindCodeArguments(metadata, LinkCodes, this.PropertyName((FilletArcActivity p) => p.LinkCodes));
		BindCodeArguments(metadata, StartPointCodes, this.PropertyName((FilletArcActivity p) => p.StartPointCodes));
		BindCodeArguments(metadata, EndPointCodes, this.PropertyName((FilletArcActivity p) => p.EndPointCodes));
		ValidateCodeArguments(metadata, LinkCodes);
		ValidateCodeArguments(metadata, StartPointCodes);
		ValidateCodeArguments(metadata, EndPointCodes);
	}

	public object GetPropertyValue(string propertyName)
	{
		return CreationRuleHelper.GetPropertyValue(this, propertyName);
	}

	public void SetPropertyValue(string propertyName, object newValue)
	{
		CreationRuleHelper.SetPropertyValue(this, propertyName, newValue);
	}
}
