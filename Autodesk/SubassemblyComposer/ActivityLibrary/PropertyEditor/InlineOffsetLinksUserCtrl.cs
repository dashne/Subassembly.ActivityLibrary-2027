using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class InlineOffsetLinksUserCtrl : ExtendedPropertyValueEditor
{
	public InlineOffsetLinksUserCtrl()
	{
		InlineEditorTemplate = new DataTemplate();
		FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(StackPanel));
		Binding value = new Binding("Value")
		{
			Converter = new ArgumentConverter<string>(),
			Mode = BindingMode.TwoWay
		};
		FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(OffsetLinksUserCtrl));
		frameworkElementFactory2.SetValue(OffsetLinksUserCtrl.StringValueProperty, value);
		frameworkElementFactory.AppendChild(frameworkElementFactory2);
		InlineEditorTemplate.VisualTree = frameworkElementFactory;
	}
}
