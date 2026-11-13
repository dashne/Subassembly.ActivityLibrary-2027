using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Converters;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

public partial class SwitchCaseCtrl : UserControl, IComponentConnector
{
	public static readonly DependencyProperty ModelItemProperty = DependencyProperty.Register("ModelItem", typeof(ModelItem), typeof(SwitchCaseCtrl), new FrameworkPropertyMetadata(OnModelItemChanged));

	public static readonly DependencyProperty CaseLabelProperty = DependencyProperty.Register("CaseLabel", typeof(InArgument<string>), typeof(SwitchCaseCtrl), new FrameworkPropertyMetadata(OnCaseLabelChanged));

	private bool _bindingActivityModelItem;

	private bool _bindingExpression;

	private string _caseKey = string.Empty;

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

	public InArgument<string> CaseLabel
	{
		get
		{
			return (InArgument<string>)GetValue(CaseLabelProperty);
		}
		set
		{
			SetValue(CaseLabelProperty, value);
		}
	}

	private string CaseKey
	{
		get
		{
			if (string.IsNullOrEmpty(_caseKey) && ModelItem.GetCurrentValue() is SwitchCondition switchCondition)
			{
				foreach (KeyValuePair<string, InArgument<string>> caseLabelArgument in switchCondition.CaseLabelArguments)
				{
					if (caseLabelArgument.Value == CaseLabel)
					{
						_caseKey = caseLabelArgument.Key;
						break;
					}
				}
			}
			return _caseKey;
		}
	}

	private string ConditionPropertyName => (ModelItem.GetCurrentValue() as SwitchCondition).Key2ConditionPropertyName(CaseKey);

	public SwitchCaseCtrl()
	{
		InitializeComponent();
		Binding binding = new Binding("CaseLabel")
		{
			Source = this,
			Converter = new ArgumentConverter<string>()
		};
		Label.SetBinding(TextBox.TextProperty, binding);
	}

	private static void OnModelItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		SwitchCaseCtrl switchCaseCtrl = d as SwitchCaseCtrl;
		ModelItem modelItem = e.NewValue as ModelItem;
		if (switchCaseCtrl != null && modelItem != null)
		{
			switchCaseCtrl.BindingActivityModelItem();
		}
	}

	private static void OnCaseLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is SwitchCaseCtrl switchCaseCtrl)
		{
			_ = e.NewValue;
			switchCaseCtrl.BindingExpression();
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

	private void BindingExpression()
	{
		if (!_bindingExpression && ModelItem != null && ModelItem.GetCurrentValue() is SwitchCondition && !string.IsNullOrEmpty(ConditionPropertyName))
		{
			Binding binding = new Binding(ConditionPropertyName);
			binding.Source = ModelItem;
			binding.Mode = BindingMode.TwoWay;
			binding.Converter = new ArgumentToExpressionConverter();
			binding.ConverterParameter = ArgumentDirection.In;
			VbBox.SetBinding(ExpressionTextBox.ExpressionProperty, binding);
			_bindingExpression = true;
		}
	}

	private void DeleteButton_Click(object sender, RoutedEventArgs e)
	{
		if (!string.IsNullOrEmpty(CaseKey))
		{
			SwitchCondition switchCondition = ModelItem.GetCurrentValue() as SwitchCondition;
			InArgument<string> labelArgument = switchCondition.CaseLabelArguments[CaseKey];
			InArgument<bool> conditionArgument = switchCondition.CaseConditionArguments[CaseKey];
			switchCondition.CaseLabelArguments.Remove(CaseKey);
			switchCondition.CaseConditionArguments.Remove(CaseKey);
			switchCondition.RefreshCaseItemArguments();
			if (ActivityDesigntime.Instance != null)
			{
				EditingContext editingContext = ActivityDesigntime.Instance.Context.Services.GetService<ModelService>().Root.GetEditingContext();
				ActivityDesigntime.Instance.Context.Services.GetService<UndoEngine>().AddUndoUnit(new DeleteCaseItemUndoUnit(editingContext, ModelItem, CaseKey, labelArgument, conditionArgument));
				ActivityDesigntime.Instance.RefreshPropertyGrid();
				ActivityHelpers.SetPropertyValue(ModelItem, "CaseNumber", switchCondition.CaseConditionArguments.Count);
			}
		}
	}

	private void ETBButton_Click(object sender, RoutedEventArgs e)
	{
		ModelProperty modelProperty = ModelItem.Properties[ConditionPropertyName];
		if (modelProperty != null)
		{
			Argument argumentValue = ((modelProperty.Value != null) ? (modelProperty.Value.GetCurrentValue() as Argument) : null);
			ExpressionDialog.ShowDialog(ModelItem, ConditionPropertyName, argumentValue, typeof(bool));
		}
	}

	private void Label_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (sender is TextBox textBox && !textBox.Text.Equals(Resource.String_Default_CaseLabel_Prefix))
		{
			textBox.Foreground = new SolidColorBrush(Colors.Black);
			textBox.FontStyle = FontStyles.Normal;
		}
	}
}
