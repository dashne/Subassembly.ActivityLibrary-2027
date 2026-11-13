using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using Autodesk.SubassemblyComposer.ActivityLibrary;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class GeometryNameChangedHelper
{
	private string _propertyName = string.Empty;

	private GeometryAutoNumberingAttribute _geometryAttribute;

	private CodePathTreeNode _codePathTree;

	private ModelItem _modelItem;

	public static void Execute(CodePathTreeNode codePathTree, ModelItem modelItem, string propertyName, string oldName, string newName)
	{
		new GeometryNameChangedHelper(codePathTree, modelItem, propertyName).Run(oldName, newName);
	}

	public GeometryNameChangedHelper(CodePathTreeNode codePathTree, ModelItem modelItem, string propertyName)
	{
		_codePathTree = codePathTree;
		_modelItem = modelItem;
		_propertyName = propertyName;
		PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(modelItem.GetCurrentValue())[_propertyName];
		if (propertyDescriptor != null)
		{
			_geometryAttribute = propertyDescriptor.Attributes[typeof(GeometryAutoNumberingAttribute)] as GeometryAutoNumberingAttribute;
			if (_geometryAttribute == null)
			{
				throw new NotSupportedException();
			}
		}
	}

	public void Run(string oldName, string newName)
	{
		if (newName == null || oldName.Equals(newName))
		{
			return;
		}
		CorrectGeometryNameReferencesHelper.Execute(oldName, newName);
		ActivityLibraryAppCallbacks.RenameGeometryIdCallback(oldName, newName);
		string stringPropertyValue = ActivityHelpers.GetStringPropertyValue(_modelItem, "DisplayName");
		if (!string.IsNullOrEmpty(stringPropertyValue))
		{
			string text = string.Empty;
			string[] array = stringPropertyValue.Split(SubassemblyActivityAutoNumberingAttribute.DisplayNameSeparator);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				if (Regex.Match(text2, _geometryAttribute.IDPattern).Success && text2.Equals(oldName))
				{
					text2 = newName;
				}
				if (!string.IsNullOrEmpty(text2))
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += SubassemblyActivityAutoNumberingAttribute.DisplayNameSeparator;
						text += text2;
					}
					else
					{
						text = text2;
					}
				}
			}
			if (!stringPropertyValue.Equals(text))
			{
				_modelItem.Properties["DisplayName"].SetValue(text);
				ActivityDesigntime.Instance.RefreshPropertyGrid();
			}
		}
		ActivityDesigntime.Instance.CodePathTreeDirty = true;
	}

	public bool VerifyGeometryName(ref string newName)
	{
		if (string.IsNullOrEmpty(newName) && _geometryAttribute.AllowNull)
		{
			return true;
		}
		if (!Regex.Match(newName, _geometryAttribute.IDPattern).Success)
		{
			MessageBox.Show(_geometryAttribute.ValidatePromptMessage, Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return false;
		}
		try
		{
			int.Parse(Regex.Match(newName, GeometryNamePatterns.DigitalPattern).Value);
		}
		catch (OverflowException)
		{
			MessageBox.Show(string.Format(Resource.Message_GeometryNumExceedLimit, _geometryAttribute.IDPrefix, int.MaxValue), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return false;
		}
		if (!GeomertyNameCheckHelper.Execute(_codePathTree, newName, ActivityHelpers.GetActivityId(_modelItem)))
		{
			string nextGeometryName = _geometryAttribute.GetNextGeometryName(_geometryAttribute.GetType());
			if (MessageBox.Show(string.Format(Resource.Message_UsedName, nextGeometryName), Resource.Message_Caption_Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
			{
				return false;
			}
			ModelProperty modelProperty = _modelItem.Properties[_propertyName];
			if (modelProperty != null)
			{
				modelProperty.SetValue(new InArgument<string>
				{
					Expression = nextGeometryName
				});
				newName = nextGeometryName;
			}
		}
		return true;
	}
}
