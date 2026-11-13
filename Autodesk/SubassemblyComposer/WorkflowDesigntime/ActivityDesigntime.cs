using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class ActivityDesigntime
{
	private static ActivityDesigntime _currentInstance;

	public bool IsRefreshing;

	private bool _inPasting;

	private ActivitySelection _selection;

	private List<ModelItem> _activities;

	private bool _refreshGeometryActivities = true;

	private CodePathTreeNode _codePathTree;

	private ObservableCollection<string> _surfaceTargetNames;

	private ObservableCollection<string> _offsetTargetNames;

	private ObservableCollection<string> _elevationTargetNames;

	private ObservableCollection<string> _outputParameternames;

	public Collection<ModelProperty> CachedAutoLinkModelProperty = new Collection<ModelProperty>();

	public static ActivityDesigntime Instance => _currentInstance;

	public EditingContext Context { get; private set; }

	public bool InPasting
	{
		get
		{
			return _inPasting;
		}
		set
		{
			_inPasting = value;
			_ = _inPasting;
		}
	}

	public ActivitySelection Selection
	{
		get
		{
			if (_selection == null)
			{
				_selection = new ActivitySelection(this);
			}
			return _selection;
		}
	}

	public List<ModelItem> GeometryActivities
	{
		get
		{
			if (_refreshGeometryActivities)
			{
				_activities = TraverseWorkflow.Execute();
				_activities.Reverse();
				_refreshGeometryActivities = false;
			}
			return _activities;
		}
	}

	public CodePathTreeNode CodePathTree
	{
		get
		{
			if (CodePathTreeDirty || (_codePathTree == null && ActivitiesInLoop.Count == 0))
			{
				CodePathParser codePathParser = new CodePathParser();
				_codePathTree = codePathParser.Traverse();
				ActivitiesInLoop.Clear();
				ActivitiesInLoop.AddRange(codePathParser.ActivitiesInLoop);
				CodePathTreeDirty = false;
			}
			return _codePathTree;
		}
	}

	public List<int> ActivitiesInLoop { get; private set; }

	public bool CodePathTreeDirty { internal get; set; }

	public event EventHandler<EventArgs> RefreshPropertyGridEvent;

	public static ActivityDesigntime CreateInstance(EditingContext _context)
	{
		if (_context == null)
		{
			throw new ApplicationException(Resource.Meaasge_SystemException);
		}
		_currentInstance = new ActivityDesigntime(_context);
		return _currentInstance;
	}

	public static void ClearInstance()
	{
		_currentInstance = null;
	}

	public static void RestoreInstance(ActivityDesigntime _oldInstance)
	{
		_currentInstance = _oldInstance;
	}

	private ActivityDesigntime(EditingContext context)
	{
		Context = context;
		ActivitiesInLoop = new List<int>();
		CodePathTreeDirty = true;
	}

	public void RefreshPropertyGrid()
	{
		RefreshPropertyGridEvent(this, new EventArgs());
	}

	public void ParameterNameChanged(ParameterDirection direction, Type paramType, string oldValue, string newValue)
	{
		RefreshPropertyGrid();
		if (paramType == typeof(OffsetTargetParameter))
		{
			UpdateObservableCollection(_offsetTargetNames, oldValue, newValue);
		}
		else if (paramType == typeof(ElevationTargetParameter))
		{
			UpdateObservableCollection(_elevationTargetNames, oldValue, newValue);
		}
		else if (paramType == typeof(SurfaceTargetParameter))
		{
			UpdateObservableCollection(_surfaceTargetNames, oldValue, newValue);
		}
		else if (direction == ParameterDirection.InAndOut || direction == ParameterDirection.Output)
		{
			UpdateObservableCollection(_outputParameternames, oldValue, newValue);
		}
	}

	private void UpdateObservableCollection(ObservableCollection<string> collection, string oldValue, string newValue)
	{
		if (collection == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(oldValue))
		{
			if (!string.IsNullOrEmpty(newValue))
			{
				InsertAndSort(collection, newValue);
			}
		}
		else if (string.IsNullOrEmpty(newValue))
		{
			if (!string.IsNullOrEmpty(oldValue))
			{
				collection.Remove(oldValue);
			}
		}
		else
		{
			collection.Remove(oldValue);
			InsertAndSort(collection, newValue);
		}
	}

	private void InsertAndSort(ObservableCollection<string> collection, string item)
	{
		int i = 0;
		if (!item.Equals(GlobleString.String_None))
		{
			for (; i < collection.Count && (collection[i].Equals(GlobleString.String_None) || Comparer<string>.Default.Compare(item, collection[i]) >= 0); i++)
			{
			}
		}
		collection.Insert(i, item);
	}

	public ObservableCollection<string> GetTargetParamterNames<T>()
	{
		ObservableCollection<string> observableCollection = new ObservableCollection<string>();
		observableCollection.Add(GlobleString.String_None);
		ActivityBuilder currentActivityBuilder = GetCurrentActivityBuilder();
		if (currentActivityBuilder != null)
		{
			foreach (DynamicActivityProperty property in currentActivityBuilder.Properties)
			{
				if (property.Type == typeof(InArgument<T>))
				{
					InsertAndSort(observableCollection, property.Name);
				}
			}
		}
		if (typeof(T) == typeof(SurfaceTarget))
		{
			_surfaceTargetNames = observableCollection;
		}
		else if (typeof(T) == typeof(OffsetTarget))
		{
			_offsetTargetNames = observableCollection;
		}
		else if (typeof(T) == typeof(ElevationTarget))
		{
			_elevationTargetNames = observableCollection;
		}
		return observableCollection;
	}

	public Collection<string> GetInputParameterNames<T>()
	{
		Collection<string> collection = new Collection<string>();
		ActivityBuilder currentActivityBuilder = GetCurrentActivityBuilder();
		if (currentActivityBuilder != null)
		{
			foreach (DynamicActivityProperty property in currentActivityBuilder.Properties)
			{
				if (property.Type == typeof(InArgument<T>) || property.Type == typeof(InOutArgument<T>))
				{
					collection.Add(property.Name);
				}
			}
		}
		return collection;
	}

	public ObservableCollection<string> GetOutputParamterNames()
	{
		ObservableCollection<string> observableCollection = new ObservableCollection<string>();
		ActivityBuilder currentActivityBuilder = GetCurrentActivityBuilder();
		if (currentActivityBuilder != null)
		{
			foreach (DynamicActivityProperty property in currentActivityBuilder.Properties)
			{
				if ((property.Type.IsSubclassOf(typeof(OutArgument)) || property.Type.IsSubclassOf(typeof(InOutArgument))) && !property.Name.Equals("Geometry") && !property.Name.Equals("SubassemblyErrorCenter") && !property.Name.Equals("SubassemblyRunMode"))
				{
					InsertAndSort(observableCollection, property.Name);
				}
			}
		}
		_outputParameternames = observableCollection;
		return observableCollection;
	}

	public Type GetParameterType(string parameterName)
	{
		ActivityBuilder currentActivityBuilder = GetCurrentActivityBuilder();
		if (currentActivityBuilder != null)
		{
			foreach (DynamicActivityProperty property in currentActivityBuilder.Properties)
			{
				if (property.Name.Equals(parameterName, StringComparison.CurrentCultureIgnoreCase))
				{
					return property.Type;
				}
			}
		}
		return null;
	}

	public ObservableCollection<string> GetAllUserDefinedVariableNames()
	{
		ObservableCollection<string> observableCollection = new ObservableCollection<string>();
		ModelItem rootFlowchartModelItem = GetRootFlowchartModelItem();
		if (rootFlowchartModelItem != null && rootFlowchartModelItem.GetCurrentValue() is Flowchart flowchart)
		{
			foreach (Variable variable in flowchart.Variables)
			{
				if (variable.Type != typeof(EnumType))
				{
					observableCollection.Add(variable.Name);
				}
			}
		}
		return observableCollection;
	}

	public ObservableCollection<string> GetAllUserDefinedVariableNames(Type type)
	{
		ObservableCollection<string> observableCollection = new ObservableCollection<string>();
		ModelItem rootFlowchartModelItem = GetRootFlowchartModelItem();
		if (rootFlowchartModelItem != null && rootFlowchartModelItem.GetCurrentValue() is Flowchart flowchart)
		{
			foreach (Variable variable in flowchart.Variables)
			{
				if (variable.Type == type)
				{
					observableCollection.Add(variable.Name);
				}
			}
		}
		return observableCollection;
	}

	public bool IsVaribiableExisting(string name)
	{
		return FindVariableModelItem(name) != null;
	}

	public bool IsDuplicatedWithParameter(string name)
	{
		ActivityBuilder currentActivityBuilder = GetCurrentActivityBuilder();
		if (currentActivityBuilder != null)
		{
			foreach (DynamicActivityProperty property in currentActivityBuilder.Properties)
			{
				if (property.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
				{
					return true;
				}
			}
		}
		return false;
	}

	public Type GetVariableType(string variableName)
	{
		ModelItem rootFlowchartModelItem = GetRootFlowchartModelItem();
		if (rootFlowchartModelItem != null && rootFlowchartModelItem.GetCurrentValue() is Flowchart flowchart)
		{
			foreach (Variable variable in flowchart.Variables)
			{
				if (variable.Name.Equals(variableName, StringComparison.CurrentCultureIgnoreCase))
				{
					return variable.Type;
				}
			}
		}
		return null;
	}

	public void DefineOrModifyVariable(string name, string dataType)
	{
		DeleteVariable(name);
		if (!EnumContainer.Instance.EnumTypeData.DefinedVariables.Contains(name))
		{
			EnumContainer.Instance.EnumTypeData.DefinedVariables.Add(name);
		}
		Type dataTypeByDisplayName = ParameterTypeRegister.GetDataTypeByDisplayName(dataType);
		Variable variable = Variable.Create(name, dataTypeByDisplayName, VariableModifiers.None);
		variable.Default = null;
		GetRootFlowchartModelItem().Properties["Variables"].Collection.Add(variable);
	}

	public void DeleteVariable(string name)
	{
		if (EnumContainer.Instance.EnumTypeData.DefinedVariables.Contains(name))
		{
			EnumContainer.Instance.EnumTypeData.DefinedVariables.Remove(name);
		}
		ModelItem modelItem = FindVariableModelItem(name);
		if (modelItem != null)
		{
			GetRootFlowchartModelItem().Properties["Variables"].Collection.Remove(modelItem);
		}
	}

	private ModelItem FindVariableModelItem(string name)
	{
		ModelItem rootFlowchartModelItem = GetRootFlowchartModelItem();
		ModelItem result = null;
		if (rootFlowchartModelItem != null)
		{
			foreach (ModelItem item in rootFlowchartModelItem.Properties["Variables"].Collection)
			{
				if (item.GetCurrentValue() is Variable variable && variable.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
				{
					result = item;
					break;
				}
			}
		}
		return result;
	}

	public void ActivityAdded(ModelItem modelItem)
	{
		_refreshGeometryActivities = true;
		CodePathTreeDirty = true;
		ResetAutoNumberEngine();
		if (!FlowSwitchLinkHelper.Execute(modelItem))
		{
			ActivityAddedHelper.Execute(modelItem, InPasting);
		}
	}

	public void ActivityRemoved(ModelItem modelItem)
	{
		_refreshGeometryActivities = true;
		CodePathTreeDirty = true;
		ResetAutoNumberEngine();
		ActivityRemovedHelper.Execute(this, modelItem);
	}

	public void PropertyChanged(ModelProperty modelProperty)
	{
		_refreshGeometryActivities = true;
		CodePathTreeDirty = true;
		ResetAutoNumberEngine();
		if (!InPasting)
		{
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(modelProperty.Parent.GetCurrentValue())[modelProperty.Name];
			if (propertyDescriptor != null && propertyDescriptor.Attributes[typeof(ArgumentDependenceAttribute)] is ArgumentDependenceAttribute argumentDependenceAttribute)
			{
				argumentDependenceAttribute.Execute(this, modelProperty);
			}
		}
	}

	public void OnCaseLabelChanged(int activityId, string propertyName, string newValue, string oldValue)
	{
		ModelItem modelItem = FindActivityOnIdHelper.Execute(activityId);
		if (modelItem != null && modelItem.Parent != null)
		{
			ModelProperty modelProperty = modelItem.Parent.Properties["Next"];
			if (modelProperty != null && modelProperty.Value != null)
			{
				_ = modelProperty.Value.Properties["Cases"].Value;
			}
		}
	}

	private ActivityBuilder GetCurrentActivityBuilder()
	{
		ModelItem root = Context.Services.GetService<ModelService>().Root;
		if (root != null)
		{
			return root.GetCurrentValue() as ActivityBuilder;
		}
		return null;
	}

	private ModelItem GetRootFlowchartModelItem()
	{
		ModelProperty modelProperty = Context.Services.GetService<ModelService>().Root.Properties["Implementation"];
		if (modelProperty != null)
		{
			return modelProperty.Value;
		}
		return null;
	}

	private void ResetAutoNumberEngine()
	{
		if (!InPasting)
		{
			AutoNumberEngine.Reset();
		}
	}
}
