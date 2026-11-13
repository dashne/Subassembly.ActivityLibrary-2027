using System.Activities.Presentation.PropertyEditing;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class VariableTypeComboBox : PropertyValueEditor
{
	public VariableTypeComboBox()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ComboBox));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay
		};
		Collection<string> collection = new Collection<string>();
		foreach (string item in ParameterTypeRegister.InoutDisplayTypeNames())
		{
			if (item.CompareTo("Slope") != 0 && item.CompareTo("Grade") != 0)
			{
				collection.Add(item);
			}
		}
		frameworkElementFactory2.SetValue(ItemsControl.ItemsSourceProperty, collection);
		frameworkElementFactory2.SetValue(ComboBox.TextProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
