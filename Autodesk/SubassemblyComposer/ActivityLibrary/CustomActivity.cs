using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

public abstract class CustomActivity : CodeActivity, IDesigntimeValidate, IRecordError
{
	private Dictionary<string, object> _propertyCollection = new Dictionary<string, object>();

	private Dictionary<string, Action<object>> _propertyChangeActions = new Dictionary<string, Action<object>>();

	public EventHandler<EventArgs> GeometryTypeChanged;

	[CustomCategory(typeof(Resource), "Category_Misc", 100)]
	[CustomDisplayName(typeof(Resource), "Comment", 1)]
	[Editor(typeof(CommentTextBox), typeof(PropertyValueEditor))]
	public new string DisplayName
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CustomActivity p) => p.DisplayName)).ToString();
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CustomActivity p) => p.DisplayName), value);
		}
	}

	[Browsable(false)]
	[RequiredArgument]
	public InArgument<int> ActivityId
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CustomActivity p) => p.ActivityId)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CustomActivity p) => p.ActivityId), value);
		}
	}

	[RequiredArgument]
	[Browsable(false)]
	public InOutArgument<SubassemblyErrorCenter> SubassemblyErrorCenter
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CustomActivity p) => p.SubassemblyErrorCenter)) as InOutArgument<SubassemblyErrorCenter>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CustomActivity p) => p.SubassemblyErrorCenter), value);
		}
	}

	[RequiredArgument]
	[Browsable(false)]
	public InOutArgument<SubassemblyRunMode> SubassemblyRunMode
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CustomActivity p) => p.SubassemblyRunMode)) as InOutArgument<SubassemblyRunMode>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CustomActivity p) => p.SubassemblyRunMode), value);
		}
	}

	[Browsable(false)]
	public bool ShowErrors
	{
		get
		{
			object obj = PropertyCollection_Get(this.PropertyName((CustomActivity p) => p.ShowErrors));
			if (obj != null)
			{
				return (bool)obj;
			}
			return false;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CustomActivity p) => p.ShowErrors), value);
		}
	}

	private void DisplayNameOnChanged(object value)
	{
		if (value != null && !base.DisplayName.Equals(value))
		{
			base.DisplayName = value.ToString();
			RefreshPropertyInspector(this, null);
		}
	}

	protected CustomActivity()
	{
		ShowErrors = true;
		PropertyCollection_Set(this.PropertyName((CustomActivity p) => p.DisplayName), base.DisplayName, DisplayNameOnChanged);
	}

	public void RecordErrorInfo(ActivityContext context, ErrorLevel level, string msg)
	{
		SubassemblyErrorCenter subassemblyErrorCenter = context.GetValue(SubassemblyErrorCenter) ?? throw new NullReferenceException("SubassemblyErrorCenter is not available");
		string mErrorHost = DisplayName;
		subassemblyErrorCenter.mErrorInfoList.Add(new SubassemblyErrorInfo(level, mErrorHost, msg, ShowErrors));
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
		if (ActivityDesigntime.Instance == null || ActivityDesigntime.Instance.IsRefreshing)
		{
			return;
		}
		int value = (ActivityId.Expression as Literal<int>).Value;
		foreach (ValidationError item in this.ValidateInDesigntime(value))
		{
			metadata.AddValidationError(item);
		}
	}

	public virtual List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		return new List<ValidationError>();
	}

	private void CheckAPIErrors(CodeActivityContext context)
	{
		if (ApiErrorCollection.IsEmpty)
		{
			return;
		}
		foreach (string allError in ApiErrorCollection.GetAllErrors())
		{
			RecordErrorInfo(context, ErrorLevel.Error, allError);
		}
		ApiErrorCollection.Clear();
	}

	protected override void Execute(CodeActivityContext context)
	{
		try
		{
			CheckAPIErrors(context);
			if (context.GetValue(SubassemblyRunMode) == Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.LayoutMode)
			{
				LayoutExecute(context);
			}
			else
			{
				RoadwayExecute(context);
			}
		}
		catch
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.ErrorHandling_CustomActivity_UnknownError);
		}
	}

	protected abstract void LayoutExecute(CodeActivityContext context);

	protected abstract void RoadwayExecute(CodeActivityContext context);

	public void PropertyCollection_Set(string propertyName, object propertyValue, Action<object> onPropertyChanged = null)
	{
		if (isNameProperty(propertyName) && ActivityDesigntime.Instance != null && ActivityId != null && (propertyValue as InArgument<string>)?.Expression != null && !ActivityDesigntime.Instance.IsRefreshing)
		{
			ModelItem modelItem = FindActivityOnIdHelper.Execute((ActivityId.Expression as Literal<int>).Value);
			if (modelItem != null)
			{
				GeometryNameChangedHelper geometryNameChangedHelper = new GeometryNameChangedHelper(ActivityDesigntime.Instance.CodePathTree, modelItem, propertyName);
				string newName = (propertyValue as InArgument<string>).Expression.ToString();
				if (!geometryNameChangedHelper.VerifyGeometryName(ref newName))
				{
					return;
				}
				(propertyValue as InArgument<string>).Expression = newName;
			}
		}
		if (!_propertyCollection.ContainsKey(propertyName))
		{
			_propertyCollection.Add(propertyName, propertyValue);
		}
		else
		{
			_propertyCollection[propertyName] = propertyValue;
		}
		if (onPropertyChanged != null && !_propertyChangeActions.ContainsKey(propertyName))
		{
			_propertyChangeActions.Add(propertyName, onPropertyChanged);
		}
		if (_propertyChangeActions.ContainsKey(propertyName))
		{
			_propertyChangeActions[propertyName](propertyValue);
		}
	}

	private bool isNameProperty(string propertyName)
	{
		return propertyName switch
		{
			"AutoLinkGeometryName" => true, 
			"EndPointName" => true, 
			"LinkNumber" => true, 
			"LoopNumber" => true, 
			"OffsetNumber" => true, 
			"PointNumber" => true, 
			"ShapeNumber" => true, 
			"StartPointName" => true, 
			_ => false, 
		};
	}

	public object PropertyCollection_Get(string propertyName)
	{
		if (_propertyCollection.ContainsKey(propertyName))
		{
			return _propertyCollection[propertyName];
		}
		return null;
	}

	public void RefreshPropertyInspector(object sender, EventArgs e)
	{
		if (ActivityDesigntime.Instance != null)
		{
			ActivityDesigntime.Instance.RefreshPropertyGrid();
		}
	}

	public virtual bool GeneratesPoint(string pointNumber)
	{
		return false;
	}

	public virtual bool DependsOnPoint(string pointNumber)
	{
		return false;
	}
}
