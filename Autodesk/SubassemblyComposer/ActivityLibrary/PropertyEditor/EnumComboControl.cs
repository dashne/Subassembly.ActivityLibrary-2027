using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

internal class EnumComboControl<T> : ComboBox where T : struct
{
	public EnumComboControl()
	{
		ObjectDataProvider source = new ObjectDataProvider
		{
			ObjectType = typeof(Enum),
			MethodName = "GetValues",
			MethodParameters = { (object)typeof(T) }
		};
		Binding binding = new Binding
		{
			Source = source,
			Converter = new PointGeometryTypeCollectionConverter()
		};
		SetBinding(ItemsControl.ItemsSourceProperty, binding);
	}

	public void BindSelectedItem(object source, string propName)
	{
		if (source == null || propName == null)
		{
			throw new ArgumentNullException();
		}
		PropertyInfo property = source.GetType().GetProperty(propName);
		if (property == null || property.PropertyType != typeof(T))
		{
			throw new InvalidOperationException();
		}
		Binding binding = new Binding(propName);
		binding.Source = source;
		binding.Mode = BindingMode.TwoWay;
		binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
		binding.Converter = new EnumGeneralConverter<T>();
		SetBinding(Selector.SelectedItemProperty, binding);
	}
}
