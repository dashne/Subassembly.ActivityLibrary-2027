using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(CreateCurveDesigner))]
[CreateCurveAutoNumbering]
[ToolboxBitmap(typeof(CreatePoint), "Resources.Icons.curve_create_toolbox.ico")]
public class CreateCurve : SubassemblyActivity, ICustomAccessProperty
{
	public const string c_StartPoint = "StartPoint";

	public const string c_EndPoint = "EndPoint";

	public const int SIDE_RIGHT = 1;

	public const int SIDE_LEFT = -1;

	private const int NoIncrementsDefault = 8;

	private Dictionary<string, Argument> _arguments = new Dictionary<string, Argument>();

	private Dictionary<CurveType, ICurveRule> _rules;

	private ICurveRule _currentRule;

	private CurveType _creationType = CurveType.Arc3Points;

	protected InArgument<string> _linkNumber;

	private InArgument<int> _noIncrements = new InArgument<int>
	{
		Expression = 8
	};

	internal ICurveRule CurrentRule => _currentRule;

	[CustomCategory(typeof(Resource), "Category_Link", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_LinkNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[LinkAutoNumbering("StartPoint", "EndPoint")]
	public InArgument<string> LinkNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateCurve p) => p.LinkNumber)) as InArgument<string>;
		}
		set
		{
			_linkNumber = LinkNumber;
			PropertyCollection_Set(this.PropertyName((CreateCurve p) => p.LinkNumber), value);
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
			return PropertyCollection_Get(this.PropertyName((CreateCurve p) => p.LinkCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateCurve p) => p.LinkCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryType", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_NoIncrements", 2)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	[TesselationDependence]
	public InArgument<int> NoIncrements
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateCurve p) => p.NoIncrements)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateCurve p) => p.NoIncrements), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryType", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_CreationType", 1)]
	[Editor(typeof(CurveTypeComboBox), typeof(PropertyValueEditor))]
	public CurveType CreationType
	{
		get
		{
			return (CurveType)PropertyCollection_Get(this.PropertyName((CreateCurve p) => p.CreationType));
		}
		set
		{
			_creationType = CreationType;
			PropertyCollection_Set(this.PropertyName((CreateCurve p) => p.CreationType), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StartPoint", 1)]
	[Editor(typeof(NonAuxPointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> StartPoint
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateCurve p) => p.StartPoint)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateCurve p) => p.StartPoint), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_EndPoint", 10)]
	[Editor(typeof(NonAuxPointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> EndPoint
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateCurve p) => p.EndPoint)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateCurve p) => p.EndPoint), value);
		}
	}

	public Dictionary<string, Argument> Arguments => _arguments;

	[Browsable(false)]
	public InArgument<int> Side
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateCurve p) => p.Side)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateCurve p) => p.Side), value);
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
				list.Add(new ValidationError(Resource.Message_Validate_Duplicate_Name, isWarning: false, this.PropertyName((CreateCurve p) => p.LinkNumber)));
			}
		}
		ObservableCollection<string> allValidPointNames = ValidGeometryNamesHelper.GetAllValidPointNames(ActivityDesigntime.Instance.CodePathTree, activityId, includeOrigin: false);
		ObservableCollection<string> linkNames = ValidGeometryNamesHelper.GetLinkNames(ActivityDesigntime.Instance.CodePathTree, activityId);
		if (!this.PointArgumentValidate(StartPoint, allValidPointNames))
		{
			string message = string.Format(Resource.Message_Validate_Invalid_Value, Resource.DisplayName_StartPoint);
			list.Add(new ValidationError(message, isWarning: false, this.PropertyName((CreateCurve p) => p.StartPoint)));
		}
		if (!this.PointArgumentValidate(EndPoint, allValidPointNames))
		{
			string message2 = string.Format(Resource.Message_Validate_Invalid_Value, Resource.DisplayName_EndPoint);
			list.Add(new ValidationError(message2, isWarning: false, this.PropertyName((CreateCurve p) => p.EndPoint)));
		}
		foreach (ValidationError item in _currentRule.DesigntimeValidation(this, allValidPointNames, linkNames))
		{
			list.Add(item);
		}
		return list;
	}

	protected void LinkNumberOnChanged(object value)
	{
		if (value != null && (InArgument<string>)value != _linkNumber)
		{
			InArgument<string> linkNumber = _linkNumber;
			_linkNumber = value as InArgument<string>;
			OnGeometryNameChanged(this.PropertyName((CreateCurve p) => p.LinkNumber), linkNumber, _linkNumber);
		}
	}

	protected void CreationTypeOnChanged(object value)
	{
		if (value != null && (CurveType)value != _creationType)
		{
			CurveType creationType = _creationType;
			_creationType = (CurveType)value;
			_currentRule = _rules[_creationType];
			if (_arguments.Count > 0)
			{
				_rules[creationType].SyncProperty(_arguments);
				_currentRule.RefreshArguments(_arguments);
			}
			RefreshPropertyInspector(this, null);
		}
	}

	public CreateCurve()
	{
		_rules = CreationRuleHelper.GetRuleProcessors<CurveType, ICurveRule>();
		_currentRule = _rules[_creationType];
		StartPoint = new InArgument<string>
		{
			Expression = ""
		};
		EndPoint = new InArgument<string>
		{
			Expression = ""
		};
		PropertyCollection_Set(this.PropertyName((CreateCurve p) => p.LinkNumber), _linkNumber, LinkNumberOnChanged);
		PropertyCollection_Set(this.PropertyName((CreateCurve p) => p.CreationType), _creationType, CreationTypeOnChanged);
		PropertyCollection_Set(this.PropertyName((CreateCurve p) => p.NoIncrements), _noIncrements);
	}

	private void InternalExecute(CodeActivityContext context)
	{
		Geometry value = context.GetValue(Geometry);
		int num = context.GetValue(NoIncrements);
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
		int side = ((context.GetValue(Side) != 1) ? 1 : (-1));
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point = value.GetPoint(context.GetValue(StartPoint));
		if (point == null)
		{
			RecordErrorInfo(context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_CreateCurve_StartPoint, context.GetValue(StartPoint)));
			return;
		}
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point2 = value.GetPoint(context.GetValue(EndPoint));
		if (point2 == null)
		{
			RecordErrorInfo(context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_CreateCurve_EndPoint, context.GetValue(EndPoint)));
			return;
		}
		Link link = _currentRule.Execute(context, value, point, point2, side, num, this);
		if (link == null)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_CreateCurve_NoCurve);
			return;
		}
		if (Math.Abs(link.Points[0].DistanceTo(point)) < 1E-05)
		{
			link.Points[0] = point;
			link.Points[link.Points.Count - 1] = point2;
		}
		else if (Math.Abs(link.Points[0].DistanceTo(point2)) < 1E-05)
		{
			link.Points[0] = point2;
			link.Points[link.Points.Count - 1] = point;
		}
		if (IsCreatingAux)
		{
			foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point3 in link.Points)
			{
				if (point3 != link.StartPoint && point3 != link.EndPoint)
				{
					point3.IsAuxiliary = true;
				}
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
		value.Links.Add(link);
		foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point4 in link.Points)
		{
			if (point4 != link.StartPoint && point4 != link.EndPoint)
			{
				value.Points.Add(point4);
			}
		}
	}

	protected override void LayoutExecute(CodeActivityContext context)
	{
		InternalExecute(context);
	}

	protected override void RoadwayExecute(CodeActivityContext context)
	{
		InternalExecute(context);
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
		BindCodeArguments(metadata, LinkCodes, this.PropertyName((CreateCurve p) => p.LinkCodes));
		ValidateCodeArguments(metadata, LinkCodes);
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
