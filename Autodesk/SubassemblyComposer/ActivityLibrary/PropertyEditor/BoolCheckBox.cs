using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class BoolCheckBox : PropertyValueEditor
{
	public BoolCheckBox()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(CheckBox));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay
		};
		frameworkElementFactory2.SetValue(ToggleButton.IsCheckedProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
