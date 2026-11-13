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

[Designer(typeof(LinkIntersectionDesigner))]
[LinkIntersectionAutoNumbering]
[ToolboxBitmap(typeof(CreatePoint), "Resources.Icons.intersection_toolbox.ico")]
public class LinkIntersection : SubassemblyActivity, ICustomAccessProperty
{
	private Dictionary<string, Argument> _arguments = new Dictionary<string, Argument>();

	private Dictionary<IntersectionType, IIntersectionRule> _rules;

	private IIntersectionRule _currentRule;

	private IntersectionType _creationType = IntersectionType.IntersectionTwoLinks;

	protected InArgument<string> _pointNumber;

	internal IIntersectionRule CurrentRule => _currentRule;

	[Browsable(false)]
	public InArgument<int> Side
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((LinkIntersection p) => p.Side)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((LinkIntersection p) => p.Side), value);
		}
	}

	public Dictionary<string, Argument> Arguments => _arguments;

	[CustomCategory(typeof(Resource), "Category_Point", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_PointNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[PointAutoNumbering]
	public virtual InArgument<string> PointNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((LinkIntersection p) => p.PointNumber)) as InArgument<string>;
		}
		set
		{
			_pointNumber = PropertyCollection_Get(this.PropertyName((LinkIntersection p) => p.PointNumber)) as InArgument<string>;
			PropertyCollection_Set(this.PropertyName((LinkIntersection p) => p.PointNumber), value);
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
			return PropertyCollection_Get(this.PropertyName((LinkIntersection p) => p.PointCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((LinkIntersection p) => p.PointCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_PointGeometryType", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_Position", 1)]
	[Editor(typeof(IntersectionTypeComboBox), typeof(PropertyValueEditor))]
	public IntersectionType CreationType
	{
		get
		{
			return (IntersectionType)PropertyCollection_Get(this.PropertyName((LinkIntersection p) => p.CreationType));
		}
		set
		{
			_creationType = CreationType;
			PropertyCollection_Set(this.PropertyName((LinkIntersection p) => p.CreationType), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties2", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_Index", 1)]
	[Browsable(false)]
	public InArgument<int> Index
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((LinkIntersection p) => p.Index)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((LinkIntersection p) => p.Index), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Misc", 100)]
	[CustomDisplayName(typeof(Resource), "Show_Errors", 2)]
	[Editor(typeof(BoolCheckBox), typeof(PropertyValueEditor))]
	public new bool ShowErrors
	{
		get
		{
			return base.ShowErrors;
		}
		set
		{
			base.ShowErrors = value;
		}
	}

	[Browsable(false)]
	public override bool IsCreatingAux => false;

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = new List<ValidationError>();
		ObservableCollection<string> allValidPointNames = ValidGeometryNamesHelper.GetAllValidPointNames(ActivityDesigntime.Instance.CodePathTree, activityId, includeOrigin: false);
		ObservableCollection<string> linkNames = ValidGeometryNamesHelper.GetLinkNames(ActivityDesigntime.Instance.CodePathTree, activityId);
		foreach (ValidationError item in _currentRule.DesigntimeValidation(this, allValidPointNames, linkNames))
		{
			list.Add(item);
		}
		return list;
	}

	protected void PointNumberOnChanged(object value)
	{
		if (value != null && value != _pointNumber)
		{
			InArgument<string> pointNumber = _pointNumber;
			_pointNumber = value as InArgument<string>;
			OnGeometryNameChanged(this.PropertyName((LinkIntersection p) => p.PointNumber), pointNumber, _pointNumber);
		}
	}

	protected void CreationTypeOnChanged(object value)
	{
		if (value != null && (IntersectionType)value != _creationType)
		{
			IntersectionType creationType = _creationType;
			_creationType = (IntersectionType)value;
			_currentRule = _rules[_creationType];
			if (_arguments.Count > 0)
			{
				_rules[creationType].SyncProperty(_arguments);
				_currentRule.RefreshArguments(_arguments);
			}
			RefreshPropertyInspector(this, null);
		}
	}

	public LinkIntersection()
	{
		_rules = CreationRuleHelper.GetRuleProcessors<IntersectionType, IIntersectionRule>();
		_currentRule = _rules[_creationType];
		Index = new InArgument<int>
		{
			Expression = 0
		};
		PropertyCollection_Set(this.PropertyName((LinkIntersection p) => p.PointNumber), null, PointNumberOnChanged);
		PropertyCollection_Set(this.PropertyName((LinkIntersection p) => p.CreationType), _creationType, CreationTypeOnChanged);
		PropertyCollection_Set(this.PropertyName((LinkIntersection p) => p.IsCreatingAux), false);
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
		if (context.GetValue(SubassemblyErrorCenter) == null)
		{
			throw new NullReferenceException("SubassemblyErrorCenter is not available");
		}
		int side = ((context.GetValue(Side) != 1) ? 1 : (-1));
		List<Autodesk.SubassemblyComposer.WorkflowEngine.Point> list = _currentRule.Execute(context, value, side, this);
		int value2 = context.GetValue(Index);
		if (list == null || list.Count == 0)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_LinkIntersect_NotIntersected);
			return;
		}
		if (value2 > list.Count - 1 || value2 < 0)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_LinkIntersect_OutOfRange);
			return;
		}
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point = list[value2];
		if (point != null)
		{
			point.ActivityId = context.GetValue(ActivityId);
			point.GeometryId = context.GetValue(PointNumber);
			point.Codes = GetCodes(context, PointCodes);
			point.IsAuxiliary = IsCreatingAux;
			if (ActivityDesigntime.Instance != null)
			{
				point.DisplayName = DisplayName;
			}
			value.Points.Add(point);
		}
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		_currentRule.CacheMetadata(metadata, _arguments);
		base.CacheMetadata(metadata);
		BindCodeArguments(metadata, PointCodes, this.PropertyName((LinkIntersection p) => p.PointCodes));
		ValidateCodeArguments(metadata, PointCodes);
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

	public override bool GeneratesPoint(string pointNumber)
	{
		return ((Literal<string>)PointNumber.Expression).Value == pointNumber;
	}
}
