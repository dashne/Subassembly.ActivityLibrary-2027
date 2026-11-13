using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class EnumComboBox<T> : PropertyValueEditor where T : struct
{
	public EnumComboBox(IValueConverter converter)
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(EnumComboControl<T>));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay,
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
			Converter = converter
		};
		frameworkElementFactory2.SetValue(Selector.SelectedItemProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}

	public EnumComboBox()
		: this((IValueConverter)new EnumGeneralConverter<T>())
	{
	}
}
