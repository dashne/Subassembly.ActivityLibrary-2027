using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

public partial class SwitchCaseNumberCtrl : UserControl, IComponentConnector
{
	public static readonly DependencyProperty CaseNumberValueProperty = DependencyProperty.Register("CaseNumberValue", typeof(int), typeof(SwitchCaseNumberCtrl));

	public static readonly DependencyProperty ModelItemProperty = DependencyProperty.Register("ModelItem", typeof(ModelItem), typeof(SwitchCaseNumberCtrl), new FrameworkPropertyMetadata(OnModelItemChanged));

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

	public int CaseNumberValue
	{
		get
		{
			return (int)GetValue(CaseNumberValueProperty);
		}
		set
		{
			SetValue(CaseNumberValueProperty, value);
		}
	}

	public SwitchCaseNumberCtrl()
	{
		InitializeComponent();
	}

	private static void OnModelItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
	}

	private void AddCase_Click(object sender, RoutedEventArgs e)
	{
		if (ModelItem != null && ModelItem.GetCurrentValue() is SwitchCondition switchCondition)
		{
			string nextCaseItemKey = switchCondition.GetNextCaseItemKey();
			string text = Resource.String_Default_CaseLabel_Prefix + (switchCondition.CaseLabelArguments.Count + 1);
			switchCondition.CaseLabelArguments.Add(nextCaseItemKey, new InArgument<string>
			{
				Expression = text
			});
			switchCondition.CaseConditionArguments.Add(nextCaseItemKey, new InArgument<bool>(constValue: false));
			if (ActivityDesigntime.Instance != null)
			{
				EditingContext editingContext = ActivityDesigntime.Instance.Context.Services.GetService<ModelService>().Root.GetEditingContext();
				ActivityDesigntime.Instance.Context.Services.GetService<UndoEngine>().AddUndoUnit(new AddNewCaseItemUndoUnit(editingContext, ModelItem, nextCaseItemKey, text));
				ActivityDesigntime.Instance.RefreshPropertyGrid();
				ActivityHelpers.SetPropertyValue(ModelItem, "CaseNumber", switchCondition.CaseConditionArguments.Count);
			}
		}
	}
}
