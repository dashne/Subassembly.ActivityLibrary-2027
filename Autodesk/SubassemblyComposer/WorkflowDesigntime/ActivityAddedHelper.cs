using System.Activities.Presentation.Model;
using System.ComponentModel;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class ActivityAddedHelper : ActivityChangedHelper
{
	protected bool InPasting { get; set; }

	public static void Execute(ModelItem targetModelItem, bool inPasting)
	{
		new ActivityAddedHelper(targetModelItem, inPasting).Run();
	}

	protected ActivityAddedHelper(ModelItem targetModelItem, bool inPasting)
		: base(targetModelItem)
	{
		InPasting = inPasting;
	}

	protected override void TraverseItem(ModelItem modelItem)
	{
		if (TypeDescriptor.GetAttributes(modelItem.GetCurrentValue())[typeof(ActivityAutoNumberingAttribute)] is ActivityAutoNumberingAttribute activityAutoNumberingAttribute)
		{
			if (InPasting)
			{
				activityAutoNumberingAttribute.CorrectActivityProperties(modelItem, TargetModelItem);
			}
			else
			{
				activityAutoNumberingAttribute.SetActivityProperties(modelItem);
			}
		}
	}
}
