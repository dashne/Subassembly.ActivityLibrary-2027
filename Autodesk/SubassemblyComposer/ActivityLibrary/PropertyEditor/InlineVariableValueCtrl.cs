using System;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class InlineVariableValueCtrl : ExtendedPropertyValueEditor
{
	public InlineVariableValueCtrl()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(VariableValueCtrl));
		ModelItem selectedModelItem = ActivityDesigntime.Instance.Selection.SelectedModelItem;
		if (selectedModelItem == null)
		{
			throw new NotSupportedException();
		}
		frameworkElementFactory2.SetValue(VariableValueCtrl.ModelItemProperty, selectedModelItem);
		Binding binding = new Binding
		{
			Source = selectedModelItem,
			Mode = BindingMode.OneWay
		};
		if (selectedModelItem.ItemType == typeof(SetVariableValue))
		{
			binding.Path = new PropertyPath("VariableName");
			binding.Converter = new VariableName2VariableTypeConverter();
		}
		else
		{
			if (!(selectedModelItem.ItemType == typeof(InternalVariableDefine)))
			{
				throw new NotSupportedException();
			}
			binding.Path = new PropertyPath("VariableType");
			binding.Converter = new VariableTypeDisplayName2TypeConverter();
		}
		frameworkElementFactory2.SetValue(VariableValueCtrl.VariableTypeProperty, binding);
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay,
			Converter = new VariableValueTextConverter()
		};
		frameworkElementFactory2.SetValue(VariableValueCtrl.VariableValueTextProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
