using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class InlineCheckBox : PropertyValueEditor
{
	public InlineCheckBox()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(CheckBox));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay,
			Converter = new BoolArgumentConverter()
		};
		frameworkElementFactory2.SetValue(ToggleButton.IsCheckedProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
