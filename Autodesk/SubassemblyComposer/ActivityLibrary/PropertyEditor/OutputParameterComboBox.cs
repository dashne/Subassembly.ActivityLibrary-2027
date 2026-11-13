using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class OutputParameterComboBox : PropertyValueEditor
{
	public OutputParameterComboBox()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ComboBox));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay,
			Converter = new OutputParameterArgumentConverter()
		};
		frameworkElementFactory2.SetValue(ItemsControl.ItemsSourceProperty, ActivityDesigntime.Instance.GetOutputParamterNames());
		frameworkElementFactory2.SetValue(ComboBox.TextProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
