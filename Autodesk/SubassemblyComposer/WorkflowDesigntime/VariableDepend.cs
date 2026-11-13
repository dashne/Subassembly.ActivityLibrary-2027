using System.Activities;
using System.Activities.Presentation.Model;
using System.Text.RegularExpressions;
using System.Windows;
using Autodesk.SubassemblyComposer.ActivityLibrary;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class VariableDepend : ArgumentDependenceAttribute
{
	public override void Execute(ActivityDesigntime designTime, ModelProperty modelProperty)
	{
		ModelItem parent = modelProperty.Parent;
		InternalVariableDefine internalVariableDefine = parent.GetCurrentValue() as InternalVariableDefine;
		string oldVariableName = internalVariableDefine.OldVariableName;
		string variableName = internalVariableDefine.VariableName;
		string variableType = internalVariableDefine.VariableType;
		if (modelProperty.Name.Equals("VariableName"))
		{
			if (string.Compare(variableName, oldVariableName) == 0)
			{
				return;
			}
			if (!VariableNameValidation(designTime, variableName, oldVariableName))
			{
				RestoreOldName(internalVariableDefine, parent, oldVariableName);
				return;
			}
			bool flag = !string.IsNullOrEmpty(oldVariableName);
			if (flag && DynamicLinkHelper.BeginFindCurrent(ItemType.Variable, oldVariableName))
			{
				CorrectVariableReferencesHelper.ExecuteFind(variableName, oldVariableName, variableType);
			}
			if (DynamicLinkHelper.Proceed() && CorrectVariableReferencesHelper.Proceed())
			{
				if (flag)
				{
					designTime.DeleteVariable(oldVariableName);
				}
				if (OnVariableNameOrTypeChanged(parent))
				{
					OnVariableChanged(parent);
					DynamicLinkHelper.EndAndUpdateForCurrent(oldVariableName, variableName);
					CorrectVariableReferencesHelper.UpdateSetVarValue(variableName, oldVariableName, variableType);
				}
			}
			else
			{
				RestoreOldName(internalVariableDefine, parent, oldVariableName);
			}
		}
		else if (modelProperty.Name.Equals("VariableType"))
		{
			if (OnVariableNameOrTypeChanged(parent))
			{
				OnVariableChanged(parent);
				CorrectVariableReferencesHelper.Execute(variableName, variableName, variableType);
			}
		}
		else if (modelProperty.Name.Equals("DefaultValue"))
		{
			OnVariableChanged(parent);
		}
	}

	private void RestoreOldName(InternalVariableDefine activity, ModelItem modelItem, string oldName)
	{
		activity.VariableName = oldName;
		activity.OldVariableName = oldName;
		modelItem.Properties["VariableName"].SetValue(oldName);
	}

	private bool VariableNameValidation(ActivityDesigntime designTime, string newName, string oldName)
	{
		if (string.IsNullOrEmpty(newName))
		{
			return true;
		}
		if (designTime.IsVaribiableExisting(newName) && string.Compare(newName, oldName, ignoreCase: true) != 0)
		{
			MessageBox.Show(string.Format(Resource.Message_VariableName_Duplicate, newName), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return false;
		}
		if (designTime.IsDuplicatedWithParameter(newName))
		{
			MessageBox.Show(string.Format(Resource.Message_VariableName_DuplicateWithParam, newName), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return false;
		}
		string pattern = "^\\w+$";
		if (!Regex.Match(newName, pattern).Success)
		{
			MessageBox.Show(string.Format(Resource.Message_VariableName_Invalid, newName), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return false;
		}
		return true;
	}

	private bool OnVariableNameOrTypeChanged(ModelItem modelItem)
	{
		InternalVariableDefine internalVariableDefine = modelItem.GetCurrentValue() as InternalVariableDefine;
		SetDisplayName(modelItem, internalVariableDefine.VariableName, internalVariableDefine.VariableType);
		if (!string.IsNullOrEmpty(internalVariableDefine.VariableName) && !string.IsNullOrEmpty(internalVariableDefine.VariableType))
		{
			ActivityDesigntime.Instance.DefineOrModifyVariable(internalVariableDefine.VariableName, internalVariableDefine.VariableType);
			return true;
		}
		return false;
	}

	private bool OnVariableChanged(ModelItem modelItem)
	{
		InternalVariableDefine internalVariableDefine = modelItem.GetCurrentValue() as InternalVariableDefine;
		int activityId = ActivityHelpers.GetActivityId(modelItem);
		if (internalVariableDefine.SubValidateInDesigntime(activityId).Count == 0)
		{
			SetVariable(modelItem, internalVariableDefine.VariableName, internalVariableDefine.VariableType);
			return true;
		}
		return false;
	}

	public static void SetDisplayName(ModelItem modelItem, string variableName, string variableType)
	{
		string empty = string.Empty;
		if (string.IsNullOrEmpty(variableName))
		{
			empty = Resource.DisplayName_InternalVariableDefine;
		}
		else
		{
			empty = (string.IsNullOrEmpty(variableType) ? variableName : $"{variableName} <{variableType}>");
		}
		ModelProperty modelProperty = modelItem.Properties["DisplayName"];
		string newComment = CommentsConverter.ExtractComment(modelProperty.Value.GetCurrentValue() as string);
		empty = CommentsConverter.InjectComment(empty, newComment);
		modelProperty.SetValue(empty);
	}

	public static void SetVariable(ModelItem modelItem, string valueString, string typeString)
	{
		IParameter parameter = ParameterTypeRegister.CreateDefaultParameter("dummy", typeString, ParameterDirection.Output, valueString);
		parameter.ParamValue = valueString;
		OutArgument value = parameter.ArgumentValue as OutArgument;
		modelItem.Properties["Variable"].SetValue(value);
	}
}
