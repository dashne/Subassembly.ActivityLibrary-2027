using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class TypeInfoTextBox<T> : PropertyValueEditor
{
	public TypeInfoTextBox()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(DockPanel));
		frameworkElementFactory.SetValue(DockPanel.LastChildFillProperty, true);
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(TypeInfoCtrl));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay
		};
		frameworkElementFactory2.SetValue(TypeInfoCtrl.ModelItemProperty, ActivityDesigntime.Instance.Selection.SelectedModelItem);
		frameworkElementFactory2.SetValue(TypeInfoCtrl.TextProperty, value);
		frameworkElementFactory2.SetValue(DockPanel.DockProperty, Dock.Right);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		FrameworkElementFactory frameworkElementFactory3 = new FrameworkElementFactory(typeof(TextBox));
		Binding value2 = new Binding("Value")
		{
			Mode = BindingMode.TwoWay,
			Converter = new TypeInfoArgumentConverter<T>()
		};
		frameworkElementFactory3.SetValue(TextBox.TextProperty, value2);
		frameworkElementFactory.AppendChild(frameworkElementFactory3);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
