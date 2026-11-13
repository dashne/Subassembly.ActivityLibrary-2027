using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class MyPropertyBinding
{
	internal static void EnabledProperty(PropertyValueEditor target, string path, IValueConverter conv)
	{
		if (ActivityDesigntime.Instance.Selection.SelectedModelItem != null && ActivityDesigntime.Instance.Selection.SelectedModelItem.GetCurrentValue() is CreateLink createLink)
		{
			Binding binding = new Binding(path);
			binding.Mode = BindingMode.OneWay;
			binding.Source = createLink;
			binding.Converter = conv;
			binding.ConverterParameter = createLink;
			target.InlineEditorTemplate.VisualTree.FirstChild.SetBinding(UIElement.IsEnabledProperty, binding);
		}
	}
}
