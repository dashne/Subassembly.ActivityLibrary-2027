using System.Activities.Presentation.PropertyEditing;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class CodeComboBoxEditor : PropertyValueEditor
{
	public CodeComboBoxEditor(IValueConverter _converter, CodeType codeType)
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(MultiComboBox));
		ObservableCollection<CodeItem> codes = CodeManager.GetCodes(codeType);
		Binding binding = new Binding(".")
		{
			Mode = BindingMode.TwoWay,
			Source = codes
		};
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay,
			Converter = _converter
		};
		frameworkElementFactory2.SetBinding(MultiComboBox.CodeItemsProperty, binding);
		frameworkElementFactory2.SetValue(MultiComboBox.SelectedItemsStringProperty, value);
		frameworkElementFactory2.SetValue(MultiComboBox.PromptProperty, Resource.Codes_Prompt);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
