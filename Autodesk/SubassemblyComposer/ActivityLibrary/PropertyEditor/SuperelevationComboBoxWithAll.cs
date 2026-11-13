using System.Activities.Presentation.PropertyEditing;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class SuperelevationComboBoxWithAll : PropertyValueEditor
{
	public SuperelevationComboBoxWithAll()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ComboBox));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay,
			Converter = new SuperelevationArgumentConverterWithAll()
		};
		frameworkElementFactory2.SetValue(ItemsControl.ItemsSourceProperty, GetAvailableCrossSegmentTypesWithAll());
		frameworkElementFactory2.SetValue(ComboBox.TextProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}

	private Collection<string> GetAvailableCrossSegmentTypesWithAll()
	{
		Collection<string> collection = new Collection<string>();
		foreach (CrossSegmentType item in CrossSegmentType.All.GetAvailableSegmentTypesPlusAll())
		{
			collection.Add(item.GetDisplayName());
		}
		return collection;
	}
}
