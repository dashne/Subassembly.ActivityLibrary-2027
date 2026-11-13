using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

public partial class LinksUserCtrl : UserControl, IComponentConnector
{
	public static readonly DependencyProperty StringValueProperty = DependencyProperty.Register("StringValue", typeof(string), typeof(LinksUserCtrl), new FrameworkPropertyMetadata(string.Empty, OnStringValueChanged));

	public static readonly RoutedEvent BeginSelectEvent = EventManager.RegisterRoutedEvent("BeginSelect", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LinksUserCtrl));

	public static readonly RoutedEvent EndSelectEvent = EventManager.RegisterRoutedEvent("EndSelect", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LinksUserCtrl));

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

	public LinksUserCtrl()
	{
		InitializeComponent();
		ShapeSelecting.ToolTip = Resource.Tooltip_SelectShape;
	}

	private static void OnStringValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is LinksUserCtrl linksUserCtrl)
		{
			linksUserCtrl.UpdateLinkCollection(e.NewValue as string);
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
			foreach (LinkItem item in FindResource("dataSource") as LinkCollection)
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

	private void UpdateLinkCollection(string newStringValue)
	{
		LinkCollection linkCollection = FindResource("dataSource") as LinkCollection;
		linkCollection.Clear();
		if (string.IsNullOrEmpty(newStringValue))
		{
			return;
		}
		string[] array = newStringValue.Split(',');
		foreach (string text in array)
		{
			if (!string.IsNullOrEmpty(text))
			{
				LinkItem linkItem = new LinkItem();
				linkItem.IsDuplicate = (LinkItem.IsDuplicateDelegate)Delegate.Combine(linkItem.IsDuplicate, new LinkItem.IsDuplicateDelegate(LinkNameIsExisted));
				linkItem.Name = text;
				linkItem.PropertyChanged += ItemDataChanged;
				linkCollection.Add(linkItem);
			}
		}
	}

	private void UpdateStringValueProperty()
	{
		LinkCollection obj = FindResource("dataSource") as LinkCollection;
		string text = null;
		foreach (LinkItem item in obj)
		{
			if (!string.IsNullOrEmpty(item.Name))
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += ",";
				}
				text += item.Name;
			}
		}
		StringValue = text;
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
		ObservableCollection<string> loopNames = ValidGeometryNamesHelper.GetLoopNames(ActivityDesigntime.Instance.CodePathTree, activityId);
		ObservableCollection<string> observableCollection = new ObservableCollection<string>(linkNames.Concat(loopNames));
		foreach (ModelItem geometryActivity in ActivityDesigntime.Instance.GeometryActivities)
		{
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(geometryActivity.GetCurrentValue()))
			{
				if (!(property.Attributes[typeof(GeometryAutoNumberingAttribute)] is GeometryAutoNumberingAttribute geometryAutoNumberingAttribute) || (!(geometryAutoNumberingAttribute.GetType() == typeof(LinkAutoNumberingAttribute)) && !(geometryAutoNumberingAttribute.GetType() == typeof(LoopAutoNumberingAttribute))))
				{
					continue;
				}
				bool flag = true;
				if (!string.IsNullOrEmpty(geometryAutoNumberingAttribute.CheckPropertyName))
				{
					flag = ActivityHelpers.GetBooleanPropertyValue(geometryActivity, geometryAutoNumberingAttribute.CheckPropertyName);
				}
				if (flag)
				{
					string stringPropertyValue = ActivityHelpers.GetStringPropertyValue(geometryActivity, property.Name);
					if (!string.IsNullOrEmpty(stringPropertyValue) && !list.Contains(stringPropertyValue) && observableCollection.Contains(stringPropertyValue))
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
		if (string.IsNullOrEmpty(text))
		{
			foreach (string item in observableCollection)
			{
				if (!list.Contains(item))
				{
					text = item;
					break;
				}
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			if (!string.IsNullOrEmpty(StringValue))
			{
				StringValue += ",";
			}
			StringValue += text;
		}
		else
		{
			MessageBox.Show(Resource.Message_No_Link, Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		RoutedEventArgs e2 = new RoutedEventArgs(BeginSelectEvent, this);
		RaiseEvent(e2);
	}

	private void Button_LostFocus(object sender, RoutedEventArgs e)
	{
		RoutedEventArgs e2 = new RoutedEventArgs(EndSelectEvent, this);
		RaiseEvent(e2);
	}
}
