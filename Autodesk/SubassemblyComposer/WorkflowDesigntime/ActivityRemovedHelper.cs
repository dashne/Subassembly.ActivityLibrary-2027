using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class ActivityRemovedHelper : ActivityChangedHelper
{
	protected ActivityDesigntime Manager { get; set; }

	public static void Execute(ActivityDesigntime manager, ModelItem targetModelItem)
	{
		new ActivityRemovedHelper(manager, targetModelItem).Run();
	}

	protected ActivityRemovedHelper(ActivityDesigntime manager, ModelItem targetModelItem)
		: base(targetModelItem)
	{
		Manager = manager;
	}

	protected override void TraverseItem(ModelItem modelItem)
	{
		if ((!modelItem.ItemType.IsSubclassOf(typeof(SubassemblyActivity)) || Manager.CodePathTree == null) && modelItem.ItemType == typeof(InternalVariableDefine) && modelItem.GetCurrentValue() is InternalVariableDefine internalVariableDefine)
		{
			Manager.DeleteVariable(internalVariableDefine.VariableName);
		}
	}
}
