using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class GeomertyNameCheckHelper : TraverseWorkflow
{
	private string TargetName { get; set; }

	private int ActivityId { get; set; }

	public static bool Execute(CodePathTreeNode codePathTree, string newGeomertyName, int activtiyId)
	{
		return new GeomertyNameCheckHelper(newGeomertyName, activtiyId).Run(codePathTree);
	}

	public GeomertyNameCheckHelper(string geomertyName, int activtiyId)
	{
		TargetName = geomertyName;
		ActivityId = activtiyId;
	}

	public bool Run(CodePathTreeNode codePathTree)
	{
		if ((ActivityDesigntime.Instance.CodePathTreeDirty || codePathTree != ActivityDesigntime.Instance.CodePathTree) && Traverse().Count == 0)
		{
			return true;
		}
		return TraverseCodePathNode(codePathTree);
	}

	private bool TraverseCodePathNode(CodePathTreeNode node)
	{
		if (node == null)
		{
			return true;
		}
		HashSet<int> hashSet = new HashSet<int>();
		Stack<CodePathTreeNode> stack = new Stack<CodePathTreeNode>();
		stack.Push(node);
		while (stack.Count > 0)
		{
			CodePathTreeNode codePathTreeNode = stack.Peek();
			if (!hashSet.Contains(codePathTreeNode.GetHashCode()) && (codePathTreeNode.ActivityId == ActivityId || codePathTreeNode.GeometryNames.Contains(TargetName)))
			{
				CodePathTreeNode[] array = stack.ToArray();
				for (int i = 1; i < stack.Count; i++)
				{
					CodePathTreeNode codePathTreeNode2 = array[i];
					if (codePathTreeNode2.ActivityId == ActivityId || codePathTreeNode2.GeometryNames.Contains(TargetName))
					{
						return false;
					}
				}
			}
			hashSet.Add(codePathTreeNode.GetHashCode());
			bool flag = true;
			foreach (CodePathTreeNode childNode in codePathTreeNode.ChildNodes)
			{
				if (childNode != null && !hashSet.Contains(childNode.GetHashCode()))
				{
					stack.Push(childNode);
					flag = false;
					break;
				}
			}
			if (flag)
			{
				stack.Pop();
			}
		}
		return true;
	}

	protected override void TraverseItem(ModelItem modelItem, List<ModelItem> modelItems)
	{
		if (!modelItem.ItemType.IsSubclassOf(typeof(SubassemblyActivity)) || ActivityHelpers.GetActivityId(modelItem) == ActivityId)
		{
			return;
		}
		foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(modelItem.GetCurrentValue()))
		{
			if (!(property.Attributes[typeof(GeometryAutoNumberingAttribute)] is GeometryAutoNumberingAttribute geometryAutoNumberingAttribute))
			{
				continue;
			}
			bool flag = true;
			if (!string.IsNullOrEmpty(geometryAutoNumberingAttribute.CheckPropertyName))
			{
				flag = ActivityHelpers.GetBooleanPropertyValue(modelItem, geometryAutoNumberingAttribute.CheckPropertyName);
			}
			if (flag)
			{
				string stringPropertyValue = ActivityHelpers.GetStringPropertyValue(modelItem, property.Name);
				if (!string.IsNullOrEmpty(stringPropertyValue) && TargetName.Equals(stringPropertyValue))
				{
					modelItems.Add(modelItem);
					break;
				}
			}
		}
	}
}
