using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Text.RegularExpressions;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class NextDepend : ArgumentDependenceAttribute
{
	public override void Execute(ActivityDesigntime designTime, ModelProperty modelProperty)
	{
		if (ActivityDesigntime.Instance == null || !ActivityDesigntime.Instance.CachedAutoLinkModelProperty.Contains(modelProperty))
		{
			return;
		}
		ActivityDesigntime.Instance.CachedAutoLinkModelProperty.Remove(modelProperty);
		if (!modelProperty.Name.Equals("Next") || modelProperty.Value == null || !(modelProperty.Parent.ItemType == typeof(FlowStep)) || !(modelProperty.Value.ItemType == typeof(FlowStep)))
		{
			return;
		}
		ModelItem value = modelProperty.Parent.Properties["Action"].Value;
		ModelItem value2 = modelProperty.Value.Properties["Action"].Value;
		if ((value.ItemType == typeof(CreatePoint) || value.ItemType == typeof(CreateAuxPoint)) && (value2.ItemType == typeof(CreatePoint) || value2.ItemType == typeof(CreateAuxPoint)))
		{
			string stringPropertyValue = ActivityHelpers.GetStringPropertyValue(value, "PointNumber");
			ModelProperty modelProperty2 = value2.Properties["FromPoint"];
			if (modelProperty2 != null && modelProperty2.Value == null && !string.IsNullOrEmpty(stringPropertyValue))
			{
				modelProperty2.SetValue(new InArgument<string>(stringPropertyValue));
			}
			bool constValue = ((!(value.ItemType == typeof(CreateAuxPoint)) || !(value2.ItemType == typeof(CreatePoint))) ? true : false);
			ModelProperty modelProperty3 = value2.Properties["AutoLink"];
			if (modelProperty3 != null)
			{
				modelProperty3.SetValue(new InArgument<bool>(constValue));
			}
		}
		else if ((value.ItemType == typeof(CreatePoint) || value.ItemType == typeof(CreateAuxPoint)) && value2.ItemType == typeof(CreateAuxLink))
		{
			string stringPropertyValue2 = ActivityHelpers.GetStringPropertyValue(value, "FromPoint");
			string stringPropertyValue3 = ActivityHelpers.GetStringPropertyValue(value, "PointNumber");
			SetStartAndEndPointName(stringPropertyValue2, stringPropertyValue3, value2);
		}
		else if (value.ItemType == typeof(CreatePoint) && value2.ItemType == typeof(CreateLink))
		{
			string stringPropertyValue4 = ActivityHelpers.GetStringPropertyValue(value, "FromPoint");
			string stringPropertyValue5 = ActivityHelpers.GetStringPropertyValue(value, "PointNumber");
			if (!string.IsNullOrEmpty(stringPropertyValue4) && Regex.Match(stringPropertyValue4, new PointAutoNumberingAttribute().IDPattern).Success)
			{
				SetStartAndEndPointName(stringPropertyValue4, stringPropertyValue5, value2);
			}
		}
	}

	private void SetStartAndEndPointName(string startPointName, string endPointName, ModelItem modelItem)
	{
		ModelProperty modelProperty = modelItem.Properties["StartPoint"];
		if (modelProperty != null && modelProperty.Value == null && !string.IsNullOrEmpty(startPointName))
		{
			modelProperty.SetValue(new InArgument<string>(startPointName));
		}
		ModelProperty modelProperty2 = modelItem.Properties["EndPoint"];
		if (modelProperty2 != null && modelProperty2.Value == null && !string.IsNullOrEmpty(endPointName))
		{
			modelProperty2.SetValue(new InArgument<string>(endPointName));
		}
	}
}
