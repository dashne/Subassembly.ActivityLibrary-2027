using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class CustomInlineSwitchCaseNumberCtrl : ExtendedPropertyValueEditor
{
	public CustomInlineSwitchCaseNumberCtrl()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		Binding value = new Binding("Value")
		{
			Converter = new IntArgumentConverter(),
			Mode = BindingMode.TwoWay
		};
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(SwitchCaseNumberCtrl));
		frameworkElementFactory2.SetValue(SwitchCaseNumberCtrl.CaseNumberValueProperty, value);
		frameworkElementFactory2.SetValue(SwitchCaseNumberCtrl.ModelItemProperty, ActivityDesigntime.Instance.Selection.SelectedModelItem);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
