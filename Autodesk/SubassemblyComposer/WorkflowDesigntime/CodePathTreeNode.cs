using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class CodePathTreeNode
{
	private HashSet<CodePathTreeNode> _ChildNodes;

	private ReadOnlyCollection<CodePathTreeNode> _childNodesCache;

	private HashSet<CodePathTreeNode> _ParentNodes;

	private ReadOnlyCollection<CodePathTreeNode> _parentNodesCache;

	public int ActivityId { get; set; }

	public string Name { get; set; }

	public List<string> GeometryNames { get; set; }

	public List<string> VariableNames { get; set; }

	public ReadOnlyCollection<CodePathTreeNode> ChildNodes
	{
		get
		{
			if (_childNodesCache == null)
			{
				_childNodesCache = new List<CodePathTreeNode>(_ChildNodes).AsReadOnly();
			}
			return _childNodesCache;
		}
	}

	public ReadOnlyCollection<CodePathTreeNode> ParentNodes
	{
		get
		{
			if (_parentNodesCache == null)
			{
				_parentNodesCache = new List<CodePathTreeNode>(_ParentNodes).AsReadOnly();
			}
			return _parentNodesCache;
		}
	}

	public CodePathTreeNode(string name, int id = 0)
	{
		Name = name;
		ActivityId = id;
		GeometryNames = new List<string>();
		VariableNames = new List<string>();
		_ChildNodes = new HashSet<CodePathTreeNode>();
		_ParentNodes = new HashSet<CodePathTreeNode>();
	}

	public void ClearChildNodes()
	{
		_ChildNodes.Clear();
		_childNodesCache = null;
	}

	public void AppendChildNode(CodePathTreeNode childNode)
	{
		if (childNode != null)
		{
			if (childNode._ParentNodes.Add(this))
			{
				childNode._parentNodesCache = null;
			}
			if (_ChildNodes.Add(childNode))
			{
				_childNodesCache = null;
			}
		}
	}

	public CodePathTreeNode clone()
	{
		CodePathTreeNode codePathTreeNode = new CodePathTreeNode(Name, ActivityId);
		codePathTreeNode.GeometryNames.AddRange(GeometryNames);
		codePathTreeNode.VariableNames.AddRange(VariableNames);
		foreach (CodePathTreeNode childNode in _ChildNodes)
		{
			if (childNode != null)
			{
				AppendChildNode(childNode.clone());
			}
		}
		return codePathTreeNode;
	}

	private CodePathTreeNode FindChildNode(Func<CodePathTreeNode, bool> pred)
	{
		Stack<CodePathTreeNode> stack = new Stack<CodePathTreeNode>();
		stack.Push(this);
		HashSet<int> hashSet = new HashSet<int>();
		while (stack.Count > 0)
		{
			CodePathTreeNode codePathTreeNode = stack.Peek();
			if (pred(codePathTreeNode))
			{
				return codePathTreeNode;
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
		return null;
	}

	public bool FindByName(string targetName)
	{
		return FindChildNode((CodePathTreeNode node) => node.GeometryNames.Contains(targetName)) != null;
	}

	public CodePathTreeNode FindChildNodeById(int id)
	{
		return FindChildNode((CodePathTreeNode node) => node.ActivityId == id);
	}

	public List<CodePathTreeNode> GetAllParentNodes()
	{
		List<CodePathTreeNode> list = new List<CodePathTreeNode>();
		Stack<CodePathTreeNode> stack = new Stack<CodePathTreeNode>();
		foreach (CodePathTreeNode parentNode in _ParentNodes)
		{
			if (parentNode != null)
			{
				stack.Push(parentNode);
			}
		}
		HashSet<int> hashSet = new HashSet<int>();
		while (stack.Count > 0)
		{
			CodePathTreeNode codePathTreeNode = stack.Peek();
			if (!hashSet.Contains(codePathTreeNode.GetHashCode()))
			{
				list.Add(codePathTreeNode);
				hashSet.Add(codePathTreeNode.GetHashCode());
			}
			bool flag = true;
			foreach (CodePathTreeNode parentNode2 in codePathTreeNode.ParentNodes)
			{
				if (parentNode2 != null && !hashSet.Contains(parentNode2.GetHashCode()))
				{
					stack.Push(parentNode2);
					flag = false;
					break;
				}
			}
			if (flag)
			{
				stack.Pop();
			}
		}
		return list;
	}

	public IEnumerable<List<CodePathTreeNode>> Paths(List<CodePathTreeNode> path, string startPtNumber, string endPtNumber)
	{
		CodePathTreeNode codePathTreeNode = FindChildNode((CodePathTreeNode node) => node.GeometryNames.Contains(startPtNumber));
		CodePathTreeNode codePathTreeNode2 = FindChildNode((CodePathTreeNode node) => node.GeometryNames.Contains(endPtNumber));
		if (codePathTreeNode == null || codePathTreeNode2 == null || codePathTreeNode == codePathTreeNode2)
		{
			yield break;
		}
		foreach (List<CodePathTreeNode> item in codePathTreeNode.Paths(path, codePathTreeNode2))
		{
			yield return item;
		}
	}

	public IEnumerable<List<CodePathTreeNode>> Paths(List<CodePathTreeNode> path, CodePathTreeNode endNode)
	{
		if (this == endNode)
		{
			path.Add(this);
			yield return path;
			path.Remove(this);
			yield break;
		}
		foreach (CodePathTreeNode childNode in _ChildNodes)
		{
			if (childNode == null)
			{
				continue;
			}
			foreach (List<CodePathTreeNode> subPath in childNode.Paths(path, endNode))
			{
				subPath.Add(this);
				yield return subPath;
				subPath.Remove(this);
			}
		}
	}
}
