using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class InlineSwitchCaseCtrl : ExtendedPropertyValueEditor
{
	public InlineSwitchCaseCtrl()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		Binding value = new Binding("Value")
		{
			Mode = BindingMode.TwoWay
		};
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(SwitchCaseCtrl));
		frameworkElementFactory2.SetValue(SwitchCaseCtrl.CaseLabelProperty, value);
		frameworkElementFactory2.SetValue(SwitchCaseCtrl.ModelItemProperty, ActivityDesigntime.Instance.Selection.SelectedModelItem);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
