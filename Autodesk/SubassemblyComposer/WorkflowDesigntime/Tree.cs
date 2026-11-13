using System.Collections.Generic;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class Tree
{
	public TreeNode Root { get; set; }

	public List<TreeNode> Leafs { get; set; }

	public Tree(TreeNode root)
	{
		Root = root;
		Leafs = new List<TreeNode>();
	}

	public bool IsBasic()
	{
		bool result = true;
		if (Root != null)
		{
			Queue<TreeNode> queue = new Queue<TreeNode>();
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(Root.GetHashCode());
			queue.Enqueue(Root);
			while (queue.Count > 0)
			{
				TreeNode treeNode = queue.Dequeue();
				if (treeNode.ExpandedTree != null)
				{
					result = false;
					break;
				}
				foreach (TreeNode subNode in treeNode.SubNodes)
				{
					if (subNode != null && hashSet.Add(subNode.GetHashCode()))
					{
						queue.Enqueue(subNode);
					}
				}
			}
		}
		return result;
	}
}
