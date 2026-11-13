using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class RefPointComboBox : PropertyValueEditor
{
	public RefPointComboBox()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ComboBox));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay,
			Converter = new RefPointArgumentConverter()
		};
		List<object> list = new List<object> { GlobleString.String_None };
		ListCollectionView validPointNamesView = ActivityDesigntime.Instance.Selection.ValidPointNamesView;
		if (validPointNamesView != null)
		{
			for (int i = 0; i < validPointNamesView.Count; i++)
			{
				object itemAt = validPointNamesView.GetItemAt(i);
				if (string.Compare(itemAt.ToString(), GlobleString.String_Origin) != 0)
				{
					list.Add(itemAt);
				}
			}
		}
		ListCollectionView value2 = (ListCollectionView)CollectionViewSource.GetDefaultView(list);
		frameworkElementFactory2.SetValue(ItemsControl.ItemsSourceProperty, value2);
		frameworkElementFactory2.SetValue(Selector.SelectedItemProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
