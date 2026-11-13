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
using Autodesk.SubassemblyComposer.GeomMath;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(CreatePointDesigner))]
[CreatePointAutoNumbering]
[ToolboxBitmap(typeof(CreatePoint), "Resources.Icons.point_create_toolbox.ico")]
public class CreatePoint : SubassemblyActivity, ICustomAccessProperty
{
	private Dictionary<string, Argument> _arguments = new Dictionary<string, Argument>();

	private Dictionary<PointGeometryType, IPointRule> _rules;

	private IPointRule _currentRule;

	private PointGeometryType _creationType = PointGeometryType.DeltaXAndDeltaY;

	protected InArgument<string> _pointNumber;

	protected InArgument<string> _linkName;

	private Dictionary<string, PropertyDescriptorCollection> _cachedProperties = new Dictionary<string, PropertyDescriptorCollection>();

	internal IPointRule CurrentRule => _currentRule;

	[CustomCategory(typeof(Resource), "Category_Point", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_PointNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[PointAutoNumbering]
	public InArgument<string> PointNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreatePoint p) => p.PointNumber)) as InArgument<string>;
		}
		set
		{
			_pointNumber = PropertyCollection_Get(this.PropertyName((CreatePoint p) => p.PointNumber)) as InArgument<string>;
			PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.PointNumber), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Point", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_PointCodes", 2)]
	[Editor(typeof(PointCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferPointCodeName]
	public List<InArgument<string>> PointCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreatePoint p) => p.PointCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.PointCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryType", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_Position", 1)]
	[Editor(typeof(PointGeometryTypeComboBox), typeof(PropertyValueEditor))]
	public PointGeometryType Positioning
	{
		get
		{
			return (PointGeometryType)PropertyCollection_Get(this.PropertyName((CreatePoint p) => p.Positioning));
		}
		set
		{
			_creationType = Positioning;
			PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.Positioning), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FromPoint", 1)]
	[Editor(typeof(PointComboBox), typeof(PropertyValueEditor))]
	[CanReferGeometryName]
	[FromPointDepend]
	public InArgument<string> FromPoint
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreatePoint p) => p.FromPoint)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.FromPoint), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_AutoAddLink", 4)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AutoLink_Check", 1)]
	[Editor(typeof(AutoLinkCheckbox), typeof(PropertyValueEditor))]
	[AutoLinkDepend]
	public InArgument<bool> AutoLink
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreatePoint p) => p.AutoLink)) as InArgument<bool>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.AutoLink), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_AutoAddLink", 4)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AutoLink_GeometryName", 2)]
	[Editor(typeof(AutoLinkNameTextBox), typeof(PropertyValueEditor))]
	[LinkAutoNumbering("AutoLink", "FromPoint", "PointNumber", true)]
	public InArgument<string> AutoLinkGeometryName
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreatePoint p) => p.AutoLinkGeometryName)) as InArgument<string>;
		}
		set
		{
			_linkName = AutoLinkGeometryName;
			PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.AutoLinkGeometryName), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_AutoAddLink", 4)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AutoLink_Codes", 3)]
	[Editor(typeof(AutoLinkCodesTextBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferLinkCodeName]
	public List<InArgument<string>> AutoLinkCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreatePoint p) => p.AutoLinkCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.AutoLinkCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_AutoAddLink", 4)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ApplyAOR", 4)]
	[Editor(typeof(AutoLinkApplyAORCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ApplyAOR
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreatePoint p) => p.ApplyAOR)) as InArgument<bool>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.ApplyAOR), value);
		}
	}

	[Browsable(false)]
	public InArgument<int> Side
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreatePoint p) => p.Side)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.Side), value);
		}
	}

	public Dictionary<string, Argument> Arguments
	{
		get
		{
			return _arguments;
		}
		set
		{
			_arguments = value;
		}
	}

	private object GetActivity(int id, Dictionary<int, CustomActivity> dictActivity)
	{
		if (dictActivity.ContainsKey(id))
		{
			return dictActivity[id];
		}
		try
		{
			object currentValue = FindActivityOnIdHelper.Execute(id).GetCurrentValue();
			if (currentValue is CustomActivity)
			{
				dictActivity.Add(id, (CustomActivity)currentValue);
			}
			return currentValue;
		}
		catch
		{
			return null;
		}
	}

	private List<CustomActivity> GetActivityPath(IEnumerable<int> lstIds, Dictionary<int, CustomActivity> dictActivity)
	{
		try
		{
			return (from i in lstIds
				select GetActivity(i, dictActivity) into i
				where i is CustomActivity
				select i).Cast<CustomActivity>().Reverse().ToList();
		}
		catch
		{
			return null;
		}
	}

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = new List<ValidationError>();
		string value = (PointNumber.Expression as Literal<string>).Value;
		if (!GeomertyNameCheckHelper.Execute(ActivityDesigntime.Instance.CodePathTree, value, activityId))
		{
			list.Add(new ValidationError(Resource.Message_Validate_Duplicate_Name, isWarning: false, "PointNumber"));
		}
		if (!string.IsNullOrEmpty(LayoutCantData.LeftRail) && !string.IsNullOrEmpty(LayoutCantData.RightRail))
		{
			if (FromPoint != null)
			{
				bool value2 = ((Literal<bool>)AutoLink.Expression).Value;
				bool value3 = ((Literal<bool>)ApplyAOR.Expression).Value;
				if (value2 & value3)
				{
					List<CodePathTreeNode> path = new List<CodePathTreeNode>();
					string fp = ((Literal<string>)FromPoint.Expression).Value;
					string value4 = ((Literal<string>)PointNumber.Expression).Value;
					Dictionary<int, CustomActivity> dictActivity = new Dictionary<int, CustomActivity>();
					foreach (List<CodePathTreeNode> item2 in ActivityDesigntime.Instance.CodePathTree.Paths(path, fp, value4))
					{
						List<CustomActivity> activityPath = GetActivityPath(item2.Select((CodePathTreeNode i) => i.ActivityId).ToList(), dictActivity);
						CustomActivity item = activityPath.Single((CustomActivity i) => i.GeneratesPoint(fp));
						int num = activityPath.IndexOf(item);
						int num2 = activityPath.IndexOf(this);
						using IEnumerator<CustomActivity> enumerator2 = (from i in activityPath.GetRange(num + 1, num2 - num - 1)
							where i.DependsOnPoint(fp)
							select i).GetEnumerator();
						if (enumerator2.MoveNext())
						{
							CustomActivity current2 = enumerator2.Current;
							string message = string.Format(Resource.Message_Validate_ApplyAOR_Error, current2.DisplayName, ((Literal<string>)AutoLinkGeometryName.Expression).Value);
							list.Add(new ValidationError(message, isWarning: false));
						}
					}
				}
			}
		}
		ObservableCollection<string> allValidPointNames = ValidGeometryNamesHelper.GetAllValidPointNames(ActivityDesigntime.Instance.CodePathTree, activityId, includeOrigin: true);
		ObservableCollection<string> linkNames = ValidGeometryNamesHelper.GetLinkNames(ActivityDesigntime.Instance.CodePathTree, activityId);
		if (!this.PointArgumentValidate(FromPoint, allValidPointNames))
		{
			string message2 = string.Format(Resource.Message_Validate_Invalid_Value, Resource.DisplayName_FromPoint);
			list.Add(new ValidationError(message2, isWarning: false, "FromPoint"));
		}
		foreach (ValidationError item3 in _currentRule.DesigntimeValidation(this, allValidPointNames, linkNames))
		{
			list.Add(item3);
		}
		return list;
	}

	public CreatePoint()
	{
		_rules = CreationRuleHelper.GetRuleProcessors<PointGeometryType, IPointRule>();
		_currentRule = _rules[_creationType];
		AutoLink = new InArgument<bool>(constValue: false);
		ApplyAOR = new InArgument<bool>(constValue: false);
		PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.Positioning), _creationType, PositioningOnChanged);
		PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.AutoLinkGeometryName), null, AutoLinkGeometryNameOnChanged);
		PropertyCollection_Set(this.PropertyName((CreatePoint p) => p.PointNumber), null, PointNumberOnChanged);
	}

	protected void PointNumberOnChanged(object value)
	{
		if (value != null && value != _pointNumber)
		{
			InArgument<string> pointNumber = _pointNumber;
			_pointNumber = value as InArgument<string>;
			OnGeometryNameChanged(this.PropertyName((CreatePoint p) => p.PointNumber), pointNumber, _pointNumber);
		}
	}

	protected void PositioningOnChanged(object value)
	{
		if (value != null && (PointGeometryType)value != _creationType)
		{
			PointGeometryType creationType = _creationType;
			_creationType = (PointGeometryType)value;
			_currentRule = _rules[_creationType];
			if (_arguments.Count > 0)
			{
				_rules[creationType].SyncProperty(_arguments);
				_currentRule.RefreshArguments(_arguments);
			}
			if (GeometryTypeChanged != null)
			{
				GeometryTypeChanged(this, null);
			}
		}
	}

	protected void AutoLinkGeometryNameOnChanged(object value)
	{
		if (value != null && value != _linkName)
		{
			InArgument<string> linkName = _linkName;
			_linkName = value as InArgument<string>;
			OnGeometryNameChanged("AutoLinkGeometryName", linkName, _linkName);
		}
	}

	public object GetPropertyValue(string propertyName)
	{
		object obj = null;
		if (Arguments.ContainsKey(propertyName))
		{
			obj = Arguments[propertyName];
			if (obj == null)
			{
				obj = _currentRule.GetPropertyValue(propertyName);
				if (obj != null)
				{
					Arguments[propertyName] = (Argument)obj;
				}
			}
		}
		else
		{
			obj = CreationRuleHelper.GetPropertyValue(this, propertyName);
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

	public override PropertyDescriptorCollection GetProperties()
	{
		string key = _creationType.ToString();
		if (_cachedProperties.ContainsKey(key))
		{
			return _cachedProperties[key];
		}
		PropertyDescriptorCollection properties = CreationRuleHelper.GetProperties(this, this);
		foreach (PropertyDescriptor property in _currentRule.GetProperties(this))
		{
			properties.Add(property);
		}
		_cachedProperties.Add(key, properties);
		return properties;
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
		string value2 = context.GetValue(FromPoint);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point fromPoint = null;
		if (value2 != null)
		{
			fromPoint = value.GetPoint(value2);
		}
		if (fromPoint == null)
		{
			if (!string.IsNullOrEmpty(value2))
			{
				string msg = string.Format(Resource.ErrorHandling_CreatePoint_FromPointNotFound, value2);
				RecordErrorInfo(context, ErrorLevel.Error, msg);
				return;
			}
			fromPoint = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(0.0, 0.0);
		}
		int sider = ((context.GetValue(Side) != 1) ? 1 : (-1));
		Autodesk.SubassemblyComposer.WorkflowEngine.Point thisPoint = null;
		if (runMode == Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.LayoutMode)
		{
			thisPoint = _currentRule.LayoutExecute(context, value, fromPoint, sider, this);
		}
		else
		{
			thisPoint = _currentRule.RoadwayExecute(context, value, fromPoint, sider, this);
		}
		if (thisPoint != null)
		{
			thisPoint.ActivityId = context.GetValue(ActivityId);
			thisPoint.GeometryId = context.GetValue(PointNumber);
			thisPoint.Codes = GetCodes(context, PointCodes);
			thisPoint.IsAuxiliary = IsCreatingAux;
			if (ActivityDesigntime.Instance != null)
			{
				thisPoint.DisplayName = DisplayName;
			}
			value.Points.Add(thisPoint);
		}
		if (!context.GetValue(AutoLink) || fromPoint == null || thisPoint == null)
		{
			return;
		}
		if (context.GetValue(ApplyAOR))
		{
			Link link = new Link(fromPoint, thisPoint);
			link.ActivityId = context.GetValue(ActivityId);
			link.GeometryId = context.GetValue(AutoLinkGeometryName);
			link.ApplyAOR = context.GetValue(ApplyAOR);
			link.Codes = GetCodes(context, AutoLinkCodes);
			link.IsAuxiliary = fromPoint.IsAuxiliary || thisPoint.IsAuxiliary;
			if (ActivityDesigntime.Instance != null)
			{
				link.DisplayName = DisplayName;
			}
			if (RuntimeCantData.IsFromCorridor)
			{
				string leftRail = LayoutCantData.LeftRail;
				string rightRail = LayoutCantData.RightRail;
				string cantPivotType = RuntimeCantData.PivotType.ToString();
				Autodesk.SubassemblyComposer.WorkflowEngine.Point leftRail2 = null;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point rightRail2 = null;
				Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point> dictionary = new Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point>();
				foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point3 in value.Points)
				{
					if (point3.GeometryId != "")
					{
						dictionary[point3.GeometryId] = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point3.X, point3.Y);
					}
				}
				if (leftRail != null && dictionary.ContainsKey(leftRail))
				{
					leftRail2 = dictionary[leftRail];
				}
				if (rightRail != null && dictionary.ContainsKey(rightRail))
				{
					rightRail2 = dictionary[rightRail];
				}
				double leftRailDeltaElevation = RuntimeCantData.LeftRailDeltaElevation;
				double rightRailDeltaElevation = RuntimeCantData.RightRailDeltaElevation;
				bool invalidElevation = false;
				Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point> cantPoints = GeometryMath.getCantPoints(new List<Link> { link }, dictionary, cantPivotType, leftRail2, leftRailDeltaElevation, rightRail2, rightRailDeltaElevation, ref invalidElevation);
				foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point4 in value.Points)
				{
					if (cantPoints.ContainsKey(point4.GeometryId))
					{
						point4.X = cantPoints[point4.GeometryId].X;
						point4.Y = cantPoints[point4.GeometryId].Y;
					}
				}
			}
			else
			{
				string leftRail3 = LayoutCantData.LeftRail;
				string rightRail3 = LayoutCantData.RightRail;
				string cantPivotType2 = LayoutCantData.CantPivotType;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point = null;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point2 = null;
				Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point> dictionary2 = new Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point>();
				foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point5 in value.Points)
				{
					if (point5.GeometryId != "")
					{
						dictionary2[point5.GeometryId] = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point5.X, point5.Y);
					}
				}
				if (leftRail3 != null && dictionary2.ContainsKey(leftRail3))
				{
					point = dictionary2[leftRail3];
				}
				if (rightRail3 != null && dictionary2.ContainsKey(rightRail3))
				{
					point2 = dictionary2[rightRail3];
				}
				if (point != null && point2 != null)
				{
					double result = 0.0;
					double.TryParse(LayoutCantData.LeftRailDeltaElevation, out result);
					double result2 = 0.0;
					double.TryParse(LayoutCantData.RightRailDeltaElevation, out result2);
					bool invalidElevation2 = false;
					Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point> cantPoints2 = GeometryMath.getCantPoints(new List<Link> { link }, dictionary2, cantPivotType2, point, result, point2, result2, ref invalidElevation2);
					foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point6 in value.Points)
					{
						if (cantPoints2.ContainsKey(point6.GeometryId))
						{
							point6.X = cantPoints2[point6.GeometryId].X;
							point6.Y = cantPoints2[point6.GeometryId].Y;
						}
					}
				}
			}
		}
		Link link2 = new Link(value.Points.Last((Autodesk.SubassemblyComposer.WorkflowEngine.Point i) => i.ActivityId == fromPoint.ActivityId), value.Points.Last((Autodesk.SubassemblyComposer.WorkflowEngine.Point i) => i.ActivityId == thisPoint.ActivityId));
		link2.ActivityId = context.GetValue(ActivityId);
		link2.GeometryId = context.GetValue(AutoLinkGeometryName);
		link2.ApplyAOR = context.GetValue(ApplyAOR);
		link2.Codes = GetCodes(context, AutoLinkCodes);
		link2.IsAuxiliary = fromPoint.IsAuxiliary || thisPoint.IsAuxiliary;
		if (ActivityDesigntime.Instance != null)
		{
			link2.DisplayName = DisplayName;
		}
		value.Links.Add(link2);
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		_currentRule.CacheMetadata(metadata, _arguments);
		base.CacheMetadata(metadata);
		BindCodeArguments(metadata, PointCodes, this.PropertyName((CreatePoint p) => p.PointCodes));
		BindCodeArguments(metadata, AutoLinkCodes, this.PropertyName((CreatePoint p) => p.AutoLinkCodes));
		ValidateCodeArguments(metadata, PointCodes);
		ValidateCodeArguments(metadata, AutoLinkCodes);
	}

	private bool IsOrigin(InArgument<Autodesk.SubassemblyComposer.WorkflowEngine.Point> argument)
	{
		return ((argument != null) ? (argument.Expression as VisualBasicValue<Autodesk.SubassemblyComposer.WorkflowEngine.Point>) : null)?.ExpressionText.ToString().Contains("Point(0,0)") ?? false;
	}

	public override bool GeneratesPoint(string pointNumber)
	{
		return ((Literal<string>)PointNumber.Expression).Value == pointNumber;
	}

	public override bool DependsOnPoint(string pointNumber)
	{
		try
		{
			return FromPoint != null && ((Literal<string>)FromPoint.Expression).Value == pointNumber;
		}
		catch
		{
			return false;
		}
	}
}
