using System.Collections.Generic;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class GetVariableNamesHelper
{
	public static bool Refresh(List<string> variableNames, CodePathTreeNode codePathTree, int activityId)
	{
		GetVariableNamesHelper getVariableNamesHelper = new GetVariableNamesHelper();
		List<string> list = getVariableNamesHelper.Run(codePathTree, activityId);
		bool result = getVariableNamesHelper.CopyCollection(list, variableNames);
		variableNames.Sort(new GeometryNameComparer());
		return result;
	}

	private List<string> Run(CodePathTreeNode codePathTree, int activityId)
	{
		List<string> list = new List<string>();
		if (codePathTree != null && activityId > 0)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int num = traverseSubnode(codePathTree, activityId, dictionary);
			foreach (KeyValuePair<string, int> item in dictionary)
			{
				if (item.Value == num)
				{
					list.Add(item.Key);
				}
			}
		}
		return list;
	}

	private int traverseSubnode(CodePathTreeNode node, int activityId, Dictionary<string, int> result)
	{
		if (node == null || activityId == 0)
		{
			return 0;
		}
		int num = 0;
		List<int> list = new List<int>();
		Stack<CodePathTreeNode> stack = new Stack<CodePathTreeNode>();
		stack.Push(node);
		while (stack.Count > 0)
		{
			CodePathTreeNode codePathTreeNode = stack.Peek();
			if (!list.Contains(codePathTreeNode.GetHashCode()) && codePathTreeNode.ActivityId > 0 && codePathTreeNode.ActivityId == activityId)
			{
				num++;
				CodePathTreeNode[] array = stack.ToArray();
				for (int i = 1; i < stack.Count; i++)
				{
					foreach (string variableName in array[i].VariableNames)
					{
						if (result.ContainsKey(variableName))
						{
							result[variableName]++;
						}
						else
						{
							result.Add(variableName, 1);
						}
					}
				}
			}
			list.Add(codePathTreeNode.GetHashCode());
			bool flag = true;
			foreach (CodePathTreeNode childNode in codePathTreeNode.ChildNodes)
			{
				if (childNode != null && !list.Contains(childNode.GetHashCode()))
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
		return num;
	}

	private bool CopyCollection(List<string> from, List<string> to)
	{
		bool result = false;
		List<string> list = new List<string>();
		foreach (string item in to)
		{
			if (!from.Contains(item))
			{
				list.Add(item);
			}
		}
		foreach (string item2 in list)
		{
			to.Remove(item2);
			result = true;
		}
		foreach (string item3 in from)
		{
			if (!to.Contains(item3))
			{
				to.Add(item3);
				result = true;
			}
		}
		return result;
	}
}
