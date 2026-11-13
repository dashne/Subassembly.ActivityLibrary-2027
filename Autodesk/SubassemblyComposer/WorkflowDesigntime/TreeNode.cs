using System.Collections.Generic;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class TreeNode
{
	public object Obj { get; set; }

	public List<TreeNode> SubNodes { get; set; }

	public Tree ExpandedTree { get; set; }

	public TreeNode(object obj)
	{
		Obj = obj;
		SubNodes = new List<TreeNode>();
	}

	public override bool Equals(object obj)
	{
		if (obj is TreeNode treeNode)
		{
			return Obj.Equals(treeNode.Obj);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Obj.GetHashCode();
	}
}
