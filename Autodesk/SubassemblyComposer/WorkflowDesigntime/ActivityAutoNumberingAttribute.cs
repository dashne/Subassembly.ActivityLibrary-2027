using System;
using System.Activities;
using System.Activities.Presentation.Model;
using Autodesk.SubassemblyComposer.WorkflowEngine;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
internal abstract class ActivityAutoNumberingAttribute : Attribute
{
	public abstract void SetActivityProperties(ModelItem modelItem);

	public abstract void CorrectActivityProperties(ModelItem modelItem, ModelItem compositeModelItem);

	protected bool CorrectActivityId(ModelItem modelItem)
	{
		int activityId = ActivityHelpers.GetActivityId(modelItem);
		if (activityId > 0 && FindActivityOnIdHelper.IsPastedActivity(activityId))
		{
			Activity<int> expression = AutoNumberEngine.NextActivityId;
			InArgument<int> propertyValue = new InArgument<int>
			{
				Expression = expression
			};
			ActivityHelpers.SetProperty(modelItem, "ActivityId", propertyValue);
			return true;
		}
		return false;
	}

	protected void SetActivityId(ModelItem modelItem, int activityId)
	{
		Activity<int> expression = activityId;
		InArgument<int> propertyValue = new InArgument<int>
		{
			Expression = expression
		};
		ActivityHelpers.SetProperty(modelItem, "ActivityId", propertyValue);
	}

	protected void SetDisplayName(ModelItem modelItem, string displayName)
	{
		ActivityHelpers.SetProperty(modelItem, "DisplayName", displayName);
	}

	protected void SetSubassembly(ModelItem modelItem)
	{
		InOutArgument<Geometry> propertyValue = new InOutArgument<Geometry>(new VisualBasicReference<Geometry>
		{
			ExpressionText = "Geometry"
		});
		ActivityHelpers.SetProperty(modelItem, "Geometry", propertyValue);
	}

	protected void SetSideProperty(ModelItem modelItem)
	{
		InArgument<int> propertyValue = new InArgument<int>(new VisualBasicValue<int>
		{
			ExpressionText = "Side"
		});
		ActivityHelpers.SetProperty(modelItem, "Side", propertyValue);
	}

	protected void SetErrorInfoCenter(ModelItem modelItem)
	{
		InOutArgument<SubassemblyErrorCenter> propertyValue = new InOutArgument<SubassemblyErrorCenter>(new VisualBasicReference<SubassemblyErrorCenter>
		{
			ExpressionText = "SubassemblyErrorCenter"
		});
		ActivityHelpers.SetProperty(modelItem, "SubassemblyErrorCenter", propertyValue);
	}

	protected void SetRunMode(ModelItem modelItem)
	{
		InOutArgument<SubassemblyRunMode> propertyValue = new InOutArgument<SubassemblyRunMode>(new VisualBasicReference<SubassemblyRunMode>
		{
			ExpressionText = "SubassemblyRunMode"
		});
		ActivityHelpers.SetProperty(modelItem, "SubassemblyRunMode", propertyValue);
	}
}
