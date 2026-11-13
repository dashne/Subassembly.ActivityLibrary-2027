using System.Activities.Presentation.Model;
using System.Collections.ObjectModel;
using Autodesk.SubassemblyComposer.ActivityLibrary;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class InternalVariableDefineAutoNumberingAttribute : CustomActivityAutoNumberingAttribute
{
	public override void SetActivityProperties(ModelItem modelItem)
	{
		base.SetActivityProperties(modelItem);
		SetDisplayName(modelItem, Resource.DisplayName_InternalVariableDefine);
	}

	public override void CorrectActivityProperties(ModelItem modelItem, ModelItem compositeModelItem)
	{
		base.CorrectActivityProperties(modelItem, compositeModelItem);
		InternalVariableDefine internalVariableDefine = modelItem.GetCurrentValue() as InternalVariableDefine;
		if (!string.IsNullOrEmpty(internalVariableDefine.VariableName))
		{
			string text = internalVariableDefine.VariableName + "_copy";
			ObservableCollection<string> allUserDefinedVariableNames = ActivityDesigntime.Instance.GetAllUserDefinedVariableNames();
			int num = 1;
			string text2 = text;
			while (allUserDefinedVariableNames.Contains(text))
			{
				num++;
				text = text2 + num;
			}
			modelItem.Properties["VariableName"].SetValue(text);
			string variableType = internalVariableDefine.VariableType;
			if (internalVariableDefine.DefaultValue != null)
			{
				_ = internalVariableDefine.DefaultValue.Expression;
			}
			ActivityDesigntime.Instance.DefineOrModifyVariable(text, variableType);
			VariableDepend.SetDisplayName(modelItem, text, variableType);
			VariableDepend.SetVariable(modelItem, text, variableType);
		}
	}
}
