using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

public partial class LoopLinksUserCtrl : UserControl, IComponentConnector
{
	public static readonly DependencyProperty StringValueProperty = DependencyProperty.Register("StringValue", typeof(string), typeof(LoopLinksUserCtrl), new FrameworkPropertyMetadata(string.Empty, OnStringValueChanged));

	public static readonly RoutedEvent BeginSelectLoopLinkEvent = EventManager.RegisterRoutedEvent("BeginSelectLoopLink", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LoopLinksUserCtrl));

	public static readonly RoutedEvent EndSelectLoopLinkEvent = EventManager.RegisterRoutedEvent("EndSelectLoopLink", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LoopLinksUserCtrl));

	public string StringValue
	{
		get
		{
			return (string)GetValue(StringValueProperty);
		}
		set
		{
			SetValue(StringValueProperty, value);
		}
	}

	public LoopLinksUserCtrl()
	{
		InitializeComponent();
		LinkSelecting.ToolTip = Resource.Tooltip_SelectLink;
	}

	private static void OnStringValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is LoopLinksUserCtrl loopLinksUserCtrl)
		{
			loopLinksUserCtrl.UpdateLoopLinkCollection(e.NewValue as string);
		}
	}

	public void ItemDataChanged(object sender, PropertyChangedEventArgs e)
	{
		UpdateStringValueProperty();
	}

	public bool LinkNameIsExisted(string linkName)
	{
		if (!string.IsNullOrEmpty(linkName))
		{
			foreach (LoopLinkItem item in FindResource("dataSource") as LoopLinkCollection)
			{
				if (item.Name.Equals(linkName))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SetStringValue(List<string> names)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string name in names)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append(name);
		}
		StringValue = stringBuilder.ToString();
	}

	private void UpdateLoopLinkCollection(string newStringValue)
	{
		LoopLinkCollection loopLinkCollection = FindResource("dataSource") as LoopLinkCollection;
		loopLinkCollection.Clear();
		if (string.IsNullOrEmpty(newStringValue))
		{
			return;
		}
		string[] array = newStringValue.Split(',');
		foreach (string text in array)
		{
			if (!string.IsNullOrEmpty(text))
			{
				LoopLinkItem loopLinkItem = new LoopLinkItem();
				loopLinkItem.IsDuplicate = (LoopLinkItem.IsDuplicateDelegate)Delegate.Combine(loopLinkItem.IsDuplicate, new LoopLinkItem.IsDuplicateDelegate(LinkNameIsExisted));
				loopLinkItem.Name = text;
				loopLinkItem.PropertyChanged += ItemDataChanged;
				loopLinkCollection.Add(loopLinkItem);
			}
		}
	}

	private void UpdateStringValueProperty()
	{
		LoopLinkCollection obj = FindResource("dataSource") as LoopLinkCollection;
		string text = null;
		ObservableCollection<string> observableCollection = ValidGeometryNamesHelper.GetLinkNames(activityId: ActivityHelpers.GetActivityId(ActivityDesigntime.Instance.Selection.SelectedModelItem), codePathTree: ActivityDesigntime.Instance.CodePathTree);
		bool flag = true;
		foreach (LoopLinkItem item in obj)
		{
			if (!string.IsNullOrEmpty(item.Name))
			{
				if (!observableCollection.Contains(item.Name))
				{
					flag = false;
					MessageBox.Show(string.Format(Resource.Message_CreateLoop_LinkNameNotValid, item.Name), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
					break;
				}
				text = (string.IsNullOrEmpty(text) ? (text + item.Name) : (text + "," + item.Name));
			}
		}
		if (flag)
		{
			StringValue = text;
		}
		else
		{
			StringValue = StringValue;
		}
	}

	private void AddLink_Click(object sender, RoutedEventArgs e)
	{
		string text = string.Empty;
		string stringValue = StringValue;
		List<string> list = new List<string>();
		if (!string.IsNullOrEmpty(stringValue))
		{
			list = new List<string>(stringValue.Split(','));
		}
		int activityId = ActivityHelpers.GetActivityId(ActivityDesigntime.Instance.Selection.SelectedModelItem);
		ObservableCollection<string> linkNames = ValidGeometryNamesHelper.GetLinkNames(ActivityDesigntime.Instance.CodePathTree, activityId);
		foreach (ModelItem geometryActivity in ActivityDesigntime.Instance.GeometryActivities)
		{
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(geometryActivity.GetCurrentValue()))
			{
				if (!(property.Attributes[typeof(GeometryAutoNumberingAttribute)] is GeometryAutoNumberingAttribute geometryAutoNumberingAttribute) || !(geometryAutoNumberingAttribute.GetType() == typeof(LinkAutoNumberingAttribute)))
				{
					continue;
				}
				bool flag = true;
				if (!string.IsNullOrEmpty(geometryAutoNumberingAttribute.CheckPropertyName))
				{
					flag = ActivityHelpers.GetBooleanPropertyValue(geometryActivity, geometryAutoNumberingAttribute.CheckPropertyName);
				}
				if (flag && (geometryActivity.ItemType == typeof(CreateLink) || geometryActivity.ItemType == typeof(CreatePoint)))
				{
					string stringPropertyValue = ActivityHelpers.GetStringPropertyValue(geometryActivity, property.Name);
					if (!string.IsNullOrEmpty(stringPropertyValue) && !list.Contains(stringPropertyValue) && linkNames.Contains(stringPropertyValue))
					{
						text = stringPropertyValue;
						break;
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				break;
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			if (!string.IsNullOrEmpty(StringValue))
			{
				StringValue = StringValue + "," + text;
			}
			else
			{
				StringValue += text;
			}
		}
		else
		{
			MessageBox.Show(Resource.Message_No_Link, Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		RoutedEventArgs e2 = new RoutedEventArgs(BeginSelectLoopLinkEvent, this);
		RaiseEvent(e2);
	}
}
