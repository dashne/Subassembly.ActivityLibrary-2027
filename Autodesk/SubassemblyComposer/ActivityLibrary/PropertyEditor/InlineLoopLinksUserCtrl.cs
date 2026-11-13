using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class InlineLoopLinksUserCtrl : ExtendedPropertyValueEditor
{
	public InlineLoopLinksUserCtrl()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		Binding value = new Binding("Value")
		{
			Converter = new ArgumentConverter<string>(),
			Mode = BindingMode.TwoWay
		};
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(LoopLinksUserCtrl));
		frameworkElementFactory2.SetValue(LoopLinksUserCtrl.StringValueProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
