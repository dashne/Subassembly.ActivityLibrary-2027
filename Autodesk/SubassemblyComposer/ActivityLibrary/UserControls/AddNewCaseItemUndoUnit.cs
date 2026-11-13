using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary.UserControls;

internal class AddNewCaseItemUndoUnit : UndoUnit
{
	private ModelItem _modelItem;

	private string _key;

	private string _caseLabel;

	internal AddNewCaseItemUndoUnit(EditingContext context, ModelItem modelItem, string key, string caseLabel)
		: base(context)
	{
		_key = key;
		_modelItem = modelItem;
		_caseLabel = caseLabel;
	}

	public override void Undo()
	{
		if (_modelItem != null && _key != null && _modelItem.GetCurrentValue() is SwitchCondition switchCondition)
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

	public override void Redo()
	{
		if (_modelItem != null && _key != null && _modelItem.GetCurrentValue() is SwitchCondition { CaseLabelArguments: var caseLabelArguments } switchCondition)
		{
			caseLabelArguments.Add(_key, new InArgument<string>
			{
				Expression = _caseLabel
			});
			switchCondition.CaseConditionArguments.Add(_key, new InArgument<bool>(constValue: false));
			switchCondition.RefreshCaseItemArguments();
			if (ActivityDesigntime.Instance != null)
			{
				ActivityDesigntime.Instance.RefreshPropertyGrid();
				ActivityHelpers.SetPropertyValue(_modelItem, "CaseNumber", switchCondition.CaseConditionArguments.Count);
			}
		}
	}
}
