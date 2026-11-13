using System.Activities.Presentation.PropertyEditing;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class SuperelevationComboBoxWithNone : PropertyValueEditor
{
	public SuperelevationComboBoxWithNone()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ComboBox));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay,
			Converter = new SuperelevationArgumentConverterWithNone()
		};
		frameworkElementFactory2.SetValue(ItemsControl.ItemsSourceProperty, GetAvailableCrossSegmentTypesWithNone());
		frameworkElementFactory2.SetValue(ComboBox.TextProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}

	private Collection<string> GetAvailableCrossSegmentTypesWithNone()
	{
		Collection<string> collection = new Collection<string>();
		foreach (CrossSegmentType item in CrossSegmentType.None.GetAvailableSegmentTypesPlusNone())
		{
			collection.Add(item.GetDisplayName());
		}
		return collection;
	}
}
