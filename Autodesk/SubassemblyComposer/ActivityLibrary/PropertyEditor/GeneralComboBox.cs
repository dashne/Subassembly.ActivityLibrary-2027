using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class GeneralComboBox : PropertyValueEditor
{
	public GeneralComboBox(IValueConverter converter, object itemsSource)
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ComboBox));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay,
			Converter = converter
		};
		frameworkElementFactory2.SetValue(ItemsControl.ItemsSourceProperty, itemsSource);
		frameworkElementFactory2.SetValue(Selector.SelectedItemProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
