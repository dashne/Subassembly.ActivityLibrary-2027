using System.Activities;
using System.Activities.Presentation.Converters;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

public partial class TypeInfoCtrl : UserControl, IComponentConnector
{
	public static readonly DependencyProperty ModelItemProperty = DependencyProperty.Register("ModelItem", typeof(ModelItem), typeof(TypeInfoCtrl), new FrameworkPropertyMetadata(OnModelItemChanged));

	private bool _bindingActivityModelItem;

	public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(InArgument<double>), typeof(TypeInfoCtrl), new FrameworkPropertyMetadata(OnTextChanged));

	private bool _bindingExpression;

	private string _typeInfoPropertyName = string.Empty;

	public ModelItem ModelItem
	{
		get
		{
			return (ModelItem)GetValue(ModelItemProperty);
		}
		set
		{
			SetValue(ModelItemProperty, value);
		}
	}

	public InArgument<double> Text
	{
		get
		{
			return (InArgument<double>)GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}

	public string TypeInfoPropertyName
	{
		get
		{
			if (string.IsNullOrEmpty(_typeInfoPropertyName))
			{
				Dictionary<string, Argument> dictionary = null;
				object currentValue = ModelItem.GetCurrentValue();
				if (currentValue is CreatePoint)
				{
					dictionary = (currentValue as CreatePoint).Arguments;
				}
				else if (currentValue is LinkIntersection)
				{
					dictionary = (currentValue as LinkIntersection).Arguments;
				}
				if (dictionary != null)
				{
					foreach (KeyValuePair<string, Argument> item in dictionary)
					{
						if (item.Value == Text)
						{
							_typeInfoPropertyName = item.Key;
							break;
						}
					}
				}
				else if (currentValue is HasDaylight)
				{
					_typeInfoPropertyName = "Slope";
				}
			}
			return _typeInfoPropertyName;
		}
	}

	public TypeInfoCtrl()
	{
		InitializeComponent();
	}

	private static void OnModelItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		TypeInfoCtrl typeInfoCtrl = d as TypeInfoCtrl;
		ModelItem modelItem = e.NewValue as ModelItem;
		if (typeInfoCtrl != null && modelItem != null)
		{
			typeInfoCtrl.BindingActivityModelItem();
		}
	}

	private void BindingActivityModelItem()
	{
		if (!_bindingActivityModelItem)
		{
			Binding binding = new Binding(".");
			binding.Source = ModelItem;
			VbBox.SetBinding(ExpressionTextBox.OwnerActivityProperty, binding);
			_bindingActivityModelItem = true;
		}
	}

	private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TypeInfoCtrl typeInfoCtrl)
		{
			_ = e.NewValue;
			typeInfoCtrl.BindingExpression();
		}
	}

	private void BindingExpression()
	{
		if (!_bindingExpression && ModelItem != null && !string.IsNullOrEmpty(TypeInfoPropertyName))
		{
			Binding binding = new Binding(TypeInfoPropertyName);
			binding.Source = ModelItem;
			binding.Mode = BindingMode.TwoWay;
			binding.Converter = new ArgumentToExpressionConverter();
			binding.ConverterParameter = ArgumentDirection.In;
			VbBox.SetBinding(ExpressionTextBox.ExpressionProperty, binding);
			_bindingExpression = true;
		}
	}

	private void ETBButton_Click(object sender, RoutedEventArgs e)
	{
		ModelProperty modelProperty = ModelItem.Properties[TypeInfoPropertyName];
		if (modelProperty != null)
		{
			Argument argumentValue = ((modelProperty.Value != null) ? (modelProperty.Value.GetCurrentValue() as Argument) : null);
			ExpressionDialog.ShowDialog(ModelItem, TypeInfoPropertyName, argumentValue, typeof(double));
		}
	}
}
