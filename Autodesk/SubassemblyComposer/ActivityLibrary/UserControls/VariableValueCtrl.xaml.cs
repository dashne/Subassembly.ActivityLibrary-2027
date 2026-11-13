using System;
using System.Activities;
using System.Activities.Presentation.Converters;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

public partial class VariableValueCtrl : UserControl, IComponentConnector
{
	public static readonly DependencyProperty ModelItemProperty = DependencyProperty.Register("ModelItem", typeof(ModelItem), typeof(VariableValueCtrl), new FrameworkPropertyMetadata(OnModelItemChanged));

	public static readonly DependencyProperty VariableTypeProperty = DependencyProperty.Register("VariableType", typeof(Type), typeof(VariableValueCtrl), new FrameworkPropertyMetadata(OnVariableTypeChanged));

	public static readonly DependencyProperty VariableValueTextProperty = DependencyProperty.Register("VariableValueText", typeof(string), typeof(VariableValueCtrl), new FrameworkPropertyMetadata(OnVariableValueTextChanged));

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

	public Type VariableType
	{
		get
		{
			return (Type)GetValue(VariableTypeProperty);
		}
		set
		{
			SetValue(VariableTypeProperty, value);
		}
	}

	public string VariableValueText
	{
		get
		{
			return (string)GetValue(VariableValueTextProperty);
		}
		set
		{
			SetValue(VariableValueTextProperty, value);
		}
	}

	public VariableValueCtrl()
	{
		InitializeComponent();
		Refresh();
	}

	private static void OnModelItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		VariableValueCtrl variableValueCtrl = d as VariableValueCtrl;
		ModelItem modelItem = e.NewValue as ModelItem;
		if (variableValueCtrl != null && modelItem != null)
		{
			variableValueCtrl.BindingExpressionTextBox();
		}
	}

	private static void OnVariableTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		VariableValueCtrl variableValueCtrl = d as VariableValueCtrl;
		Type type = e.NewValue as Type;
		if (variableValueCtrl != null && type != null)
		{
			variableValueCtrl.Refresh();
		}
	}

	private static void OnVariableValueTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		_ = d is VariableValueCtrl;
		_ = e.NewValue;
	}

	private void Refresh()
	{
		WarngingColumn.Width = new GridLength(0.0);
		if (VariableType == typeof(Slope) || VariableType == typeof(Grade))
		{
			ExpressionBoxColumn.Width = new GridLength(0.0);
			TextColumn.Width = new GridLength(1.0, GridUnitType.Star);
		}
		else
		{
			TextColumn.Width = new GridLength(0.0);
			ExpressionBoxColumn.Width = new GridLength(1.0, GridUnitType.Star);
			VB_Box.ExpressionType = VariableType;
		}
	}

	private void BindingExpressionTextBox()
	{
		if (ModelItem != null)
		{
			Binding binding = new Binding(".");
			binding.Source = ModelItem;
			VB_Box.SetBinding(ExpressionTextBox.OwnerActivityProperty, binding);
			Binding binding2 = new Binding("VariableValue");
			binding2.Source = ModelItem;
			binding2.Mode = BindingMode.TwoWay;
			binding2.Converter = new ArgumentToExpressionConverter();
			binding2.ConverterParameter = ArgumentDirection.In;
			VB_Box.SetBinding(ExpressionTextBox.ExpressionProperty, binding2);
			VB_Box.UseLocationExpression = false;
		}
	}

	private void ETBButton_Click(object sender, RoutedEventArgs e)
	{
	}

	private void Text_Box_LostFocus(object sender, RoutedEventArgs e)
	{
	}
}
