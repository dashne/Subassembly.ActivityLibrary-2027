using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class PropertyBinding
{
	internal static void EnabledProperty(PropertyValueEditor target, string path, IValueConverter conv)
	{
		if (ActivityDesigntime.Instance.Selection.SelectedModelItem != null && ActivityDesigntime.Instance.Selection.SelectedModelItem.GetCurrentValue() is CreatePoint createPoint)
		{
			Binding binding = new Binding(path);
			binding.Mode = BindingMode.OneWay;
			binding.Source = createPoint;
			binding.Converter = conv;
			binding.ConverterParameter = createPoint;
			target.InlineEditorTemplate.VisualTree.FirstChild.SetBinding(UIElement.IsEnabledProperty, binding);
		}
	}
}
