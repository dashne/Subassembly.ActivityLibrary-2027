using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

internal class DeleteCaseItemUndoUnit : UndoUnit
{
	private ModelItem _modelItem;

	private string _key;

	private InArgument<string> _labelArgument;

	private InArgument<bool> _conditionArgument;

	internal DeleteCaseItemUndoUnit(EditingContext context, ModelItem modelItem, string key, InArgument<string> labelArgument, InArgument<bool> conditionArgument)
		: base(context)
	{
		_key = key;
		_labelArgument = labelArgument;
		_conditionArgument = conditionArgument;
		_modelItem = modelItem;
	}

	public override void Redo()
	{
		if (_modelItem != null && _labelArgument != null && _conditionArgument != null && _key != null && _modelItem.GetCurrentValue() is SwitchCondition switchCondition)
		{
			switchCondition.CaseLabelArguments.Remove(_key);
			switchCondition.CaseConditionArguments.Remove(_key);
			switchCondition.RefreshCaseItemArguments();
			if (ActivityDesigntime.Instance != null)
			{
				ActivityDesigntime.Instance.RefreshPropertyGrid();
				ActivityHelpers.SetPropertyValue(_modelItem, "CaseNumber", switchCondition.CaseConditionArguments.Count);
			}
		}
	}

	public override void Undo()
	{
		if (_modelItem != null && _labelArgument != null && _conditionArgument != null && _key != null && _modelItem.GetCurrentValue() is SwitchCondition switchCondition)
		{
			switchCondition.CaseLabelArguments.Add(_key, _labelArgument);
			switchCondition.CaseConditionArguments.Add(_key, _conditionArgument);
			switchCondition.RefreshCaseItemArguments();
			if (ActivityDesigntime.Instance != null)
			{
				ActivityDesigntime.Instance.RefreshPropertyGrid();
				ActivityHelpers.SetPropertyValue(_modelItem, "CaseNumber", switchCondition.CaseConditionArguments.Count);
			}
		}
	}
}
