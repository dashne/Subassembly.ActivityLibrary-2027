using System;
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

[Designer(typeof(RoundingDaylightDesigner))]
[RoundDayLightAutoNumbering]
[ToolboxBitmap(typeof(RoundDayLightActivity), "Resources.Icons.rounding_daylight_toolbox.ico")]
public class RoundDayLightActivity : SubassemblyActivity, IDesigntimeValidate, ICustomAccessProperty
{
	private const int DefaultTessellation = 8;

	private Dictionary<string, Argument> _arguments = new Dictionary<string, Argument>();

	private Dictionary<RoundOptionEnum, IDaylightRoundRule> _rules;

	private IDaylightRoundRule _currentRule;

	private RoundOptionEnum _creationType = RoundOptionEnum.ArcRounding;

	private InArgument<string> _linkNumber;

	private InArgument<string> startPointName;

	private InArgument<string> endPointName;

	internal IDaylightRoundRule CurrentRule => _currentRule;

	[CustomCategory(typeof(Resource), "Category_Link", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_LinkNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[LinkAutoNumbering("StartPointName", "EndPointName")]
	public InArgument<string> LinkNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.LinkNumber)) as InArgument<string>;
		}
		set
		{
			_linkNumber = PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.LinkNumber)) as InArgument<string>;
			PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.LinkNumber), value);
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
			return PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.LinkCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.LinkCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_General", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DaylightRound_DaylightLink", 3)]
	[Editor(typeof(LinkComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> DayLightLink
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.DayLightLink)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.DayLightLink), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_General", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DaylightRound_SurfaceTarget", 4)]
	[Editor(typeof(TargetComboBox<SurfaceTarget>), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	public InArgument<SurfaceTarget> Surface
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.Surface)) as InArgument<SurfaceTarget>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.Surface), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_General", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DaylightRound_Tessellation", 5)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	[TesselationDependence]
	public InArgument<int> Tessellation
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.Tessellation)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.Tessellation), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryType", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DaylightRound_RoundOption", 1)]
	[Editor(typeof(EnumComboBox<RoundOptionEnum>), typeof(PropertyValueEditor))]
	public RoundOptionEnum RoundOption
	{
		get
		{
			return (RoundOptionEnum)PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.RoundOption));
		}
		set
		{
			_creationType = RoundOption;
			PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.RoundOption), value);
		}
	}

	[Browsable(false)]
	[CustomCategory(typeof(Resource), "Category_General", 1)]
	public InArgument<int> Side
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.Side)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.Side), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_RoundDaylight_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DaylightRound_StartPointName", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[PointAutoNumbering]
	public InArgument<string> StartPointName
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.StartPointName)) as InArgument<string>;
		}
		set
		{
			startPointName = StartPointName;
			PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.StartPointName), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_RoundDaylight_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DaylightRound_StartPointCode", 2)]
	[Editor(typeof(PointCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferPointCodeName]
	public List<InArgument<string>> StartPointCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.StartPointCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.StartPointCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_RoundDaylight_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DaylightRound_EndPointName", 3)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[PointAutoNumbering]
	public InArgument<string> EndPointName
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.EndPointName)) as InArgument<string>;
		}
		set
		{
			endPointName = EndPointName;
			PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.EndPointName), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_RoundDaylight_Points", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_DaylightRound_EndPointCode", 4)]
	[Editor(typeof(PointCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferPointCodeName]
	public List<InArgument<string>> EndPointCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((RoundDayLightActivity p) => p.EndPointCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.EndPointCodes), value);
		}
	}

	public Dictionary<string, Argument> Arguments => _arguments;

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
		ObservableCollection<string> allValidPointNames = ValidGeometryNamesHelper.GetAllValidPointNames(ActivityDesigntime.Instance.CodePathTree, activityId, includeOrigin: false);
		ObservableCollection<string> linkNames = ValidGeometryNamesHelper.GetLinkNames(ActivityDesigntime.Instance.CodePathTree, activityId);
		if (!this.LinkArgumentValidate(DayLightLink, linkNames))
		{
			string message = string.Format(Resource.Message_Validate_Invalid_Value, Resource.DisplayName_DaylightRound_DaylightLink);
			list.Add(new ValidationError(message, isWarning: false, "DaylightLink"));
		}
		foreach (ValidationError item in _currentRule.DesigntimeValidation(this, allValidPointNames, linkNames))
		{
			list.Add(item);
		}
		return list;
	}

	protected void LinkNumberOnChanged(object value)
	{
		if (value != null && value != _linkNumber)
		{
			InArgument<string> linkNumber = _linkNumber;
			_linkNumber = value as InArgument<string>;
			OnGeometryNameChanged(this.PropertyName((RoundDayLightActivity p) => p.LinkNumber), linkNumber, _linkNumber);
		}
	}

	protected void RoundOptionOnChanged(object value)
	{
		if (value != null && (RoundOptionEnum)value != _creationType)
		{
			RoundOptionEnum creationType = _creationType;
			_creationType = (RoundOptionEnum)value;
			_currentRule = _rules[_creationType];
			if (_arguments.Count > 0)
			{
				_rules[creationType].SyncProperty(_arguments);
				_currentRule.RefreshArguments(_arguments);
			}
			RefreshPropertyInspector(this, null);
		}
	}

	protected void StartPointNameOnChanged(object value)
	{
		if (value != null && (InArgument<string>)value != startPointName)
		{
			InArgument<string> oldName = startPointName;
			startPointName = (InArgument<string>)value;
			OnGeometryNameChanged(this.PropertyName((RoundDayLightActivity p) => p.StartPointName), oldName, startPointName);
		}
	}

	protected void EndPointNameOnChanged(object value)
	{
		if (value != null && (InArgument<string>)value != endPointName)
		{
			InArgument<string> oldName = endPointName;
			endPointName = (InArgument<string>)value;
			OnGeometryNameChanged(this.PropertyName((RoundDayLightActivity p) => p.EndPointName), oldName, endPointName);
		}
	}

	public RoundDayLightActivity()
	{
		_rules = CreationRuleHelper.GetRuleProcessors<RoundOptionEnum, IDaylightRoundRule>();
		_currentRule = _rules[_creationType];
		Tessellation = new InArgument<int>
		{
			Expression = 8
		};
		PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.RoundOption), RoundOptionEnum.ArcRounding, RoundOptionOnChanged);
		PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.EndPointName), null, EndPointNameOnChanged);
		PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.LinkNumber), null, LinkNumberOnChanged);
		PropertyCollection_Set(this.PropertyName((RoundDayLightActivity p) => p.StartPointName), null, StartPointNameOnChanged);
	}

	protected override void LayoutExecute(CodeActivityContext context)
	{
		InternalExecute(context, Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.LayoutMode);
	}

	protected override void RoadwayExecute(CodeActivityContext context)
	{
		InternalExecute(context, Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.RoadwayMode);
	}

	private void InternalExecute(CodeActivityContext context, SubassemblyRunMode runMode)
	{
		Geometry value = context.GetValue(Geometry);
		string value2 = context.GetValue(DayLightLink);
		Link link = value.GetLink(value2);
		if (link == null || link.Points.Count != 2 || (GeometryUtility.FuzzyEqual(link.Points.First().X, link.Points.Last().X) && !GeometryUtility.FuzzyEqual(link.Points.First().Y, link.Points.Last().Y)))
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_RoundDaylight_InvalidLink);
			return;
		}
		SurfaceTarget surfaceTarget = null;
		if (runMode == Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.RoadwayMode)
		{
			surfaceTarget = context.GetValue(Surface);
		}
		else
		{
			double num = Math.Min(link.Points.First().X, link.Points.Last().X);
			double num2 = Math.Max(link.Points.First().X, link.Points.Last().X);
			double y = link.Points.Last().Y;
			surfaceTarget = new PreviewSurfaceTarget("LayoutSurface", false, num - 2000.0, Autodesk.SubassemblyComposer.WorkflowEngine.Point.Origin.Elevation + y, num2 + 2000.0, Autodesk.SubassemblyComposer.WorkflowEngine.Point.Origin.Elevation + y);
		}
		if (surfaceTarget == null)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_RoundDaylight_InvalidSurface);
			return;
		}
		Autodesk.SubassemblyComposer.WorkflowEngine.Point pointOnOffset = surfaceTarget.getPointOnOffset(link.StartPoint.Offset);
		bool flag = pointOnOffset != null && GeometryUtility.FuzzyEqual(pointOnOffset, link.StartPoint);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point pointOnOffset2 = surfaceTarget.getPointOnOffset(link.EndPoint.Offset);
		bool flag2 = pointOnOffset2 != null && GeometryUtility.FuzzyEqual(pointOnOffset2, link.EndPoint);
		if (!flag && !flag2)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_RoundDaylight_InvalidLink);
			return;
		}
		bool flag3 = GeometryUtility.FuzzyEqual(link.StartPoint, link.EndPoint);
		Link link2;
		if (flag & flag2 & flag3)
		{
			RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_RoundDaylight_DegeneratedLink);
			link2 = ProduceDegeneratedGeometry(link.StartPoint);
		}
		else if (flag & flag2)
		{
			RecordErrorInfo(context, ErrorLevel.Informational, Resource.ErrorHandling_RoundDaylight_BothOnSurface);
			Autodesk.SubassemblyComposer.WorkflowEngine.Point endPoint = link.EndPoint;
			Autodesk.SubassemblyComposer.WorkflowEngine.Point startPoint = link.StartPoint;
			link2 = ProduceGeometry(context, surfaceTarget, startPoint, endPoint);
		}
		else
		{
			Autodesk.SubassemblyComposer.WorkflowEngine.Point ptOnSurface = (flag ? link.StartPoint : link.EndPoint);
			Autodesk.SubassemblyComposer.WorkflowEngine.Point ptOutSurface = (flag ? link.EndPoint : link.StartPoint);
			link2 = ProduceGeometry(context, surfaceTarget, ptOutSurface, ptOnSurface);
		}
		if (link2 != null)
		{
			SetProductProperties(ref link2, context, value);
		}
	}

	private Link ProduceDegeneratedGeometry(Autodesk.SubassemblyComposer.WorkflowEngine.Point duplicatedPoint)
	{
		Autodesk.SubassemblyComposer.WorkflowEngine.Point startPoint = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(duplicatedPoint.X, duplicatedPoint.Y);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point endPoint = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(duplicatedPoint.X, duplicatedPoint.Y);
		return new Link(startPoint, endPoint);
	}

	private Link ProduceGeometry(CodeActivityContext context, SurfaceTarget surface, Autodesk.SubassemblyComposer.WorkflowEngine.Point ptOutSurface, Autodesk.SubassemblyComposer.WorkflowEngine.Point ptOnSurface)
	{
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
		GeometryUtility.RoundResult result = GeometryUtility.RoundResult.Failed;
		Link result2 = _currentRule.Execute(context, ptOutSurface, ptOnSurface, surface, num + 1, ref result);
		switch (result)
		{
		case GeometryUtility.RoundResult.Failed:
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_RoundDaylight_UnexpectedError);
			return result2;
		case GeometryUtility.RoundResult.InvalidParam:
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_RoundDaylight_InvalidParameter);
			return result2;
		case GeometryUtility.RoundResult.AdaptToPoint1:
			RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_RoundDaylight_AdaptToLink);
			return result2;
		case GeometryUtility.RoundResult.AdaptToPoint2:
			RecordErrorInfo(context, ErrorLevel.Warning, Resource.ErrorHandling_RoundDaylight_AdaptToSurface);
			break;
		}
		return result2;
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
		PropertyDescriptorCollection properties = CreationRuleHelper.GetProperties(this, this);
		foreach (PropertyDescriptor property in _currentRule.GetProperties(this))
		{
			_ = property.Name;
			properties.Add(property);
		}
		return properties;
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		_currentRule.CacheMetadata(metadata, _arguments);
		base.CacheMetadata(metadata);
		BindCodeArguments(metadata, LinkCodes, this.PropertyName((RoundDayLightActivity p) => p.LinkCodes));
		BindCodeArguments(metadata, StartPointCodes, this.PropertyName((RoundDayLightActivity p) => p.StartPointCodes));
		BindCodeArguments(metadata, EndPointCodes, this.PropertyName((RoundDayLightActivity p) => p.EndPointCodes));
		ValidateCodeArguments(metadata, LinkCodes);
		ValidateCodeArguments(metadata, StartPointCodes);
		ValidateCodeArguments(metadata, EndPointCodes);
	}

	public object GetPropertyValue(string propertyName)
	{
		object obj = null;
		obj = ((!Arguments.ContainsKey(propertyName)) ? CreationRuleHelper.GetPropertyValue(this, propertyName) : Arguments[propertyName]);
		if (obj == null)
		{
			obj = _currentRule.GetPropertyValue(propertyName);
			if (obj != null)
			{
				Arguments[propertyName] = (Argument)obj;
			}
		}
		return obj;
	}

	public void SetPropertyValue(string propertyName, object newValue)
	{
		if (Arguments.ContainsKey(propertyName))
		{
			Arguments[propertyName] = (Argument)newValue;
			_currentRule.SetPropertyValue(propertyName, newValue);
		}
		else
		{
			CreationRuleHelper.SetPropertyValue(this, propertyName, newValue);
		}
	}
}
