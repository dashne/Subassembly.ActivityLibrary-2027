using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class InlineTextBoxEditor : PropertyValueEditor
{
	public InlineTextBoxEditor(IValueConverter _converter)
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(TextBox));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay,
			Converter = _converter
		};
		frameworkElementFactory2.SetValue(TextBox.TextProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
