using System.Activities.Presentation.Model;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal abstract class ConditionActivityAutoNumberingAttribute : ActivityAutoNumberingAttribute
{
	public override void SetActivityProperties(ModelItem modelItem)
	{
		SetActivityId(modelItem, AutoNumberEngine.NextActivityId);
		SetErrorInfoCenter(modelItem);
		SetRunMode(modelItem);
	}

	public override void CorrectActivityProperties(ModelItem modelItem, ModelItem compositeModelItem)
	{
		CorrectActivityId(modelItem);
	}
}
