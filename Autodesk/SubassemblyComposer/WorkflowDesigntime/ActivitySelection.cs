using System;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class ActivitySelection
{
	private ActivityDesigntime _activityDesignTime;

	private List<string> _validPointNames { get; set; }

	private List<string> _validNonAuxPointNames { get; set; }

	private List<string> _validLinkNames { get; set; }

	private List<string> _validVariableNames { get; set; }

	public ModelItem SelectedModelItem { get; private set; }

	public bool ContainRootActivity { get; private set; }

	public ListCollectionView ValidPointNamesView { get; private set; }

	public ListCollectionView ValidNonAuxPointNamesView { get; private set; }

	public ListCollectionView ValidLinkNamesView { get; private set; }

	public ListCollectionView ValidVariableNamesView { get; private set; }

	public ActivitySelection(ActivityDesigntime activityDesignTime)
	{
		_activityDesignTime = activityDesignTime;
		Initialize();
		ActivityLibraryAppCallbacks.RefreshSelectionDataCallback = RefreshData;
	}

	public void RefreshData()
	{
		UpdateValidPointNames();
		UpdateValidNonAuxPointNames();
		UpdateValidLinkNames();
		UpdateValidVariableNames();
	}

	public void SelectionChanged(Selection selection)
	{
		Initialize();
		if (selection == null)
		{
			return;
		}
		if (selection.PrimarySelection != null)
		{
			SelectedModelItem = selection.PrimarySelection;
			if (SelectedModelItem.ItemType.IsSubclassOf(typeof(SubassemblyActivity)) || SelectedModelItem.ItemType.IsSubclassOf(typeof(CustomCondition)))
			{
				_validPointNames = new List<string>();
				UpdateValidPointNames();
				ValidPointNamesView = (ListCollectionView)CollectionViewSource.GetDefaultView(_validPointNames);
				ValidPointNamesView.Filter = PointContains;
				_validNonAuxPointNames = new List<string>();
				UpdateValidNonAuxPointNames();
				ValidNonAuxPointNamesView = (ListCollectionView)CollectionViewSource.GetDefaultView(_validNonAuxPointNames);
				ValidNonAuxPointNamesView.Filter = PointContains;
				_validLinkNames = new List<string>();
				UpdateValidLinkNames();
				ValidLinkNamesView = (ListCollectionView)CollectionViewSource.GetDefaultView(_validLinkNames);
				ValidLinkNamesView.Filter = LinkContains;
			}
			if (SelectedModelItem.ItemType.IsSubclassOf(typeof(CustomActivity)))
			{
				_validPointNames = new List<string>();
				UpdateValidPointNames();
				ValidPointNamesView = (ListCollectionView)CollectionViewSource.GetDefaultView(_validPointNames);
				ValidPointNamesView.Filter = PointContains;
				_validNonAuxPointNames = new List<string>();
				UpdateValidNonAuxPointNames();
				ValidNonAuxPointNamesView = (ListCollectionView)CollectionViewSource.GetDefaultView(_validNonAuxPointNames);
				ValidNonAuxPointNamesView.Filter = PointContains;
				_validVariableNames = new List<string>();
				UpdateValidVariableNames();
				ValidVariableNamesView = (ListCollectionView)CollectionViewSource.GetDefaultView(_validVariableNames);
				ValidVariableNamesView.Filter = VariableContains;
			}
			if (SelectedModelItem.GetCurrentValue() is CreatePoint { GeometryTypeChanged: null } createPoint)
			{
				createPoint.GeometryTypeChanged = (EventHandler<EventArgs>)Delegate.Combine(createPoint.GeometryTypeChanged, new EventHandler<EventArgs>(createPoint.RefreshPropertyInspector));
			}
		}
		foreach (ModelItem selectedObject in selection.SelectedObjects)
		{
			ModelItem root = _activityDesignTime.Context.Services.GetService<ModelService>().Root;
			if (selectedObject.Equals(root.Content.Value))
			{
				ContainRootActivity = true;
				break;
			}
		}
	}

	private void Initialize()
	{
		_validPointNames = null;
		_validLinkNames = null;
		_validVariableNames = null;
		_validNonAuxPointNames = null;
		ValidPointNamesView = null;
		ValidLinkNamesView = null;
		ValidVariableNamesView = null;
		SelectedModelItem = null;
		ContainRootActivity = false;
	}

	private bool PointContains(object objName)
	{
		string input = objName as string;
		if (SelectedModelItem.GetCurrentValue() is CreateLink && !(SelectedModelItem.GetCurrentValue() as CreateLink).IsCreatingAux && Regex.Match(input, new AuxPointAutoNumberingAttribute().IDPattern).Success)
		{
			return false;
		}
		return true;
	}

	private bool LinkContains(object objName)
	{
		return true;
	}

	private bool VariableContains(object objName)
	{
		return true;
	}

	private bool UpdateValidPointNames()
	{
		if (_validPointNames != null)
		{
			bool includeOrigin = SelectedModelItem.GetCurrentValue() is CreatePoint || SelectedModelItem.GetCurrentValue() is GetMarkPointActivity;
			int activityId = ActivityHelpers.GetActivityId(SelectedModelItem);
			return ValidGeometryNamesHelper.RefreshAllValidPointNames(_validPointNames, _activityDesignTime.CodePathTree, activityId, includeOrigin);
		}
		return false;
	}

	private bool UpdateValidNonAuxPointNames()
	{
		if (_validPointNames != null)
		{
			bool includeOrigin = SelectedModelItem.GetCurrentValue() is CreatePoint || SelectedModelItem.GetCurrentValue() is GetMarkPointActivity;
			int activityId = ActivityHelpers.GetActivityId(SelectedModelItem);
			return ValidGeometryNamesHelper.RefreshNonAuxPointNames(_validNonAuxPointNames, _activityDesignTime.CodePathTree, activityId, includeOrigin);
		}
		return false;
	}

	private bool UpdateValidLinkNames()
	{
		if (_validLinkNames != null)
		{
			int activityId = ActivityHelpers.GetActivityId(SelectedModelItem);
			return ValidGeometryNamesHelper.RefreshLinkNames(_validLinkNames, _activityDesignTime.CodePathTree, activityId);
		}
		return false;
	}

	private bool UpdateValidVariableNames()
	{
		if (_validVariableNames != null)
		{
			int activityId = ActivityHelpers.GetActivityId(SelectedModelItem);
			return GetVariableNamesHelper.Refresh(_validVariableNames, _activityDesignTime.CodePathTree, activityId);
		}
		return false;
	}
}
