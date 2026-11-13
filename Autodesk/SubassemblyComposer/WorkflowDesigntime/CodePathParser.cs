#define TRACE
using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Autodesk.SubassemblyComposer.ActivityLibrary;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class CodePathParser
{
	public const string c_FlowDecision = "FlowDecision";

	public const string c_FlowSwitch = "FlowSwitch";

	public const string c_Switch = "Switch";

	public const string c_Condition = "Condition";

	public const string c_CustomActivity = "CustomActivity";

	private static readonly Dictionary<Type, PropertyDescriptorCollection> s_propertyDescriptorCache = new Dictionary<Type, PropertyDescriptorCollection>();

	public List<int> ActivitiesInLoop { get; private set; }

	public CodePathParser()
	{
		ActivitiesInLoop = new List<int>();
	}

	public CodePathTreeNode Traverse()
	{
		CodePathTreeNode result = null;
		if (ActivityDesigntime.Instance.Context != null)
		{
			ModelProperty modelProperty = ActivityDesigntime.Instance.Context.Services.GetService<ModelService>().Root.Properties["Implementation"];
			if (modelProperty != null)
			{
				try
				{
					result = Traverse(modelProperty.Value.GetCurrentValue());
				}
				catch (ApplicationException ex)
				{
					if (!string.IsNullOrEmpty(ex.Message))
					{
						ActivitiesInLoop.Add(0);
						MessageBox.Show(ex.Message, Resource.Message_Caption_Error, MessageBoxButton.OK, MessageBoxImage.Hand);
					}
				}
			}
		}
		return result;
	}

	private CodePathTreeNode Traverse(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		Tree tree = new Tree(new TreeNode(obj as Flowchart));
		ExpandTreeNodes(tree);
		while (!tree.IsBasic())
		{
			tree = RebuildTree(tree);
			ExpandTreeNodes(tree);
		}
		return CreateCodePathTree(tree);
	}

	private void ExpandTreeNodes(Tree tree)
	{
		Dictionary<int, object> dictionary = new Dictionary<int, object>();
		Queue<TreeNode> queue = new Queue<TreeNode>();
		queue.Enqueue(tree.Root);
		while (queue.Count > 0)
		{
			TreeNode treeNode = queue.Dequeue();
			if (treeNode == null)
			{
				continue;
			}
			if (dictionary.ContainsKey(treeNode.Obj.GetHashCode()))
			{
				TreeNode treeNode2 = dictionary[treeNode.Obj.GetHashCode()] as TreeNode;
				treeNode.Obj = treeNode2.Obj;
				treeNode.SubNodes = treeNode2.SubNodes;
				treeNode.ExpandedTree = treeNode2.ExpandedTree;
				continue;
			}
			object obj = treeNode.Obj;
			dictionary.Add(obj.GetHashCode(), treeNode.Obj);
			if (treeNode == null)
			{
				break;
			}
			foreach (TreeNode subNode in treeNode.SubNodes)
			{
				if (subNode != null)
				{
					queue.Enqueue(subNode);
				}
			}
			Type type = treeNode.Obj.GetType();
			if (type == typeof(FlowStep))
			{
				FlowStep flowStep = treeNode.Obj as FlowStep;
				SubExpandTreeNodes(treeNode, flowStep.Action);
			}
			else if (type == typeof(FlowDecision))
			{
				_ = treeNode.Obj;
				CodePathTreeNode obj2 = new CodePathTreeNode("FlowDecision");
				treeNode.Obj = obj2;
			}
			else if (type == typeof(FlowSwitch<string>))
			{
				_ = treeNode.Obj;
				CodePathTreeNode obj3 = new CodePathTreeNode("FlowSwitch");
				treeNode.Obj = obj3;
			}
			else
			{
				SubExpandTreeNodes(treeNode, treeNode.Obj);
			}
			dictionary[obj.GetHashCode()] = treeNode;
		}
	}

	private void SubExpandTreeNodes(TreeNode currentNode, object obj)
	{
		Tree tree = new Tree(null);
		Type type = obj.GetType();
		if (type == typeof(Flowchart))
		{
			Flowchart flowchart = obj as Flowchart;
			if (flowchart.StartNode != null)
			{
				tree = BuildFlowChartTree(flowchart.StartNode);
			}
			currentNode.ExpandedTree = tree;
		}
		else if (type == typeof(Sequence))
		{
			Sequence sequence = obj as Sequence;
			if (sequence.Activities.Count != 0)
			{
				List<TreeNode> list = new List<TreeNode>();
				for (int i = 0; i < sequence.Activities.Count; i++)
				{
					TreeNode item = new TreeNode(sequence.Activities[i]);
					list.Add(item);
				}
				tree.Root = list[0];
				for (int j = 0; j < list.Count - 1; j++)
				{
					list[j].SubNodes.Add(list[j + 1]);
				}
			}
			currentNode.ExpandedTree = tree;
		}
		else if (type.IsSubclassOf(typeof(SubassemblyActivity)))
		{
			CodePathTreeNode codePathTreeNode = null;
			SubassemblyActivity subassemblyActivity = obj as SubassemblyActivity;
			int activityId = ActivityHelpers.GetActivityId(subassemblyActivity.ActivityId);
			if (activityId > 0)
			{
				codePathTreeNode = GetNodeOfSubassemblyActivity(subassemblyActivity, activityId);
			}
			if (codePathTreeNode == null)
			{
				return;
			}
			currentNode.Obj = codePathTreeNode;
			if (!(obj is CreateOffset) || !(obj is CreateOffset createOffset))
			{
				return;
			}
			List<Activity> list2 = new List<Activity>();
			list2.AddRange(createOffset.Activities);
			if (list2.Count != 0)
			{
				List<TreeNode> list3 = new List<TreeNode>();
				for (int k = 0; k < createOffset.Activities.Count; k++)
				{
					TreeNode item2 = new TreeNode(createOffset.Activities[k]);
					list3.Add(item2);
				}
				tree.Root = list3[0];
				for (int l = 0; l < list3.Count - 1; l++)
				{
					list3[l].SubNodes.Add(list3[l + 1]);
				}
			}
			TreeNode treeNode = new TreeNode(new Sequence());
			treeNode.ExpandedTree = tree;
			treeNode.SubNodes.AddRange(currentNode.SubNodes);
			currentNode.SubNodes.Clear();
			currentNode.SubNodes.Add(treeNode);
		}
		else if (type.IsSubclassOf(typeof(CustomCondition)))
		{
			int activityId2 = ActivityHelpers.GetActivityId((obj as CustomCondition).ActivityId);
			if (activityId2 > 0)
			{
				currentNode.Obj = new CodePathTreeNode("Condition", activityId2);
			}
		}
		else if (type.IsSubclassOf(typeof(ConditionBase<string>)))
		{
			int activityId3 = ActivityHelpers.GetActivityId((obj as ConditionBase<string>).ActivityId);
			if (activityId3 > 0)
			{
				currentNode.Obj = new CodePathTreeNode("Switch", activityId3);
			}
		}
		else
		{
			if (!type.IsSubclassOf(typeof(CustomActivity)))
			{
				return;
			}
			CustomActivity customActivity = obj as CustomActivity;
			int activityId4 = ActivityHelpers.GetActivityId(customActivity.ActivityId);
			if (activityId4 > 0)
			{
				CodePathTreeNode codePathTreeNode2 = new CodePathTreeNode("CustomActivity", activityId4);
				if (customActivity is InternalVariableDefine internalVariableDefine)
				{
					codePathTreeNode2.VariableNames.Add(internalVariableDefine.VariableName);
				}
				currentNode.Obj = codePathTreeNode2;
			}
		}
	}

	private Tree RebuildTree(Tree tree)
	{
		ComputeExpandedLeaves(tree);
		TreeNode treeNode = ResolveEffectiveRoot(tree.Root);
		if (treeNode == null)
		{
			return new Tree(null);
		}
		Tree result = new Tree(treeNode.ExpandedTree?.Root ?? treeNode);
		Queue<TreeNode> queue = new Queue<TreeNode>();
		HashSet<int> hashSet = new HashSet<int>();
		queue.Enqueue(tree.Root);
		while (queue.Count > 0)
		{
			TreeNode treeNode2 = queue.Dequeue();
			hashSet.Add(treeNode2.GetHashCode());
			List<TreeNode> list = new List<TreeNode>();
			foreach (TreeNode subNode in treeNode2.SubNodes)
			{
				TreeNode treeNode3 = ResolveEffectiveRoot(subNode);
				if (treeNode3 != null)
				{
					list.Add(treeNode3.ExpandedTree?.Root ?? treeNode3);
					if (!hashSet.Contains(treeNode3.GetHashCode()))
					{
						queue.Enqueue(treeNode3);
					}
				}
			}
			List<TreeNode> list2;
			if (treeNode2.ExpandedTree != null)
			{
				list2 = treeNode2.ExpandedTree.Leafs;
			}
			else
			{
				treeNode2.SubNodes.Clear();
				list2 = new List<TreeNode> { treeNode2 };
			}
			foreach (TreeNode item in list2)
			{
				foreach (TreeNode item2 in list)
				{
					if (!item.SubNodes.Contains(item2))
					{
						item.SubNodes.Add(item2);
					}
				}
			}
		}
		return result;
	}

	private void ComputeExpandedLeaves(Tree tree)
	{
		Queue<TreeNode> queue = new Queue<TreeNode>();
		HashSet<int> hashSet = new HashSet<int>();
		queue.Enqueue(tree.Root);
		while (queue.Count > 0)
		{
			TreeNode treeNode = queue.Dequeue();
			hashSet.Add(treeNode.GetHashCode());
			if (treeNode.ExpandedTree != null)
			{
				treeNode.ExpandedTree.Leafs = GetLeafNodes(treeNode.ExpandedTree);
				foreach (TreeNode leaf in treeNode.ExpandedTree.Leafs)
				{
					leaf.SubNodes.Clear();
				}
			}
			foreach (TreeNode subNode in treeNode.SubNodes)
			{
				if (subNode != null && !hashSet.Contains(subNode.GetHashCode()))
				{
					queue.Enqueue(subNode);
				}
			}
		}
	}

	private static TreeNode ResolveEffectiveRoot(TreeNode node)
	{
		HashSet<int> hashSet = new HashSet<int>();
		while (node != null)
		{
			if (node.ExpandedTree == null || node.ExpandedTree.Root != null)
			{
				return node;
			}
			if (node.SubNodes.Count == 0)
			{
				return null;
			}
			if (!hashSet.Add(node.GetHashCode()))
			{
				Trace.TraceError("[RebuildTree] Cycle detected in ResolveEffectiveRoot.");
				return null;
			}
			node = node.SubNodes[0];
		}
		return null;
	}

	private void CheckForLinkAOR(object root)
	{
	}

	private void ReportFlowChartCycle(object obj)
	{
		Type type = obj.GetType();
		if (type == typeof(FlowDecision))
		{
			throw new ApplicationException(string.Format(Resource.Message_Loop, "Decision"));
		}
		if (type == typeof(FlowSwitch<string>))
		{
			throw new ApplicationException(string.Format(Resource.Message_Loop, "Switch"));
		}
		if (type == typeof(FlowStep))
		{
			FlowStep flowStep = (FlowStep)obj;
			string arg = ((flowStep.Action != null && !string.IsNullOrEmpty(flowStep.Action.DisplayName)) ? flowStep.Action.DisplayName : "Step");
			throw new ApplicationException(string.Format(Resource.Message_Loop, arg));
		}
		throw new ApplicationException(string.Format(Resource.Message_Loop, obj.GetType().Name));
	}

	private Tree BuildFlowChartTree(object root)
	{
		HashSet<int> inProgress = new HashSet<int>();
		HashSet<int> done = new HashSet<int>();
		BuildFlowChartTreeCycleDfs(root, inProgress, done);
		TreeNode treeNode = new TreeNode(root);
		Queue<TreeNode> queue = new Queue<TreeNode>();
		HashSet<int> hashSet = new HashSet<int>();
		queue.Enqueue(treeNode);
		while (queue.Count > 0)
		{
			TreeNode treeNode2 = queue.Dequeue();
			if (treeNode2 == null)
			{
				continue;
			}
			hashSet.Add(treeNode2.GetHashCode());
			Type type = treeNode2.Obj.GetType();
			if (type == typeof(FlowStep))
			{
				FlowStep flowStep = treeNode2.Obj as FlowStep;
				if (flowStep.Next != null)
				{
					TreeNode treeNode3 = new TreeNode(flowStep.Next);
					treeNode2.SubNodes.Add(treeNode3);
					if (treeNode3 != null && !hashSet.Contains(treeNode3.GetHashCode()))
					{
						queue.Enqueue(treeNode3);
					}
				}
			}
			else if (type == typeof(FlowDecision))
			{
				FlowDecision flowDecision = treeNode2.Obj as FlowDecision;
				if (flowDecision.True != null)
				{
					TreeNode treeNode4 = new TreeNode(flowDecision.True);
					treeNode2.SubNodes.Add(treeNode4);
					if (treeNode4 != null && !hashSet.Contains(treeNode4.GetHashCode()))
					{
						queue.Enqueue(treeNode4);
					}
				}
				if (flowDecision.False != null)
				{
					TreeNode treeNode5 = new TreeNode(flowDecision.False);
					treeNode2.SubNodes.Add(treeNode5);
					if (treeNode5 != null && !hashSet.Contains(treeNode5.GetHashCode()))
					{
						queue.Enqueue(treeNode5);
					}
				}
			}
			else
			{
				if (!(type == typeof(FlowSwitch<string>)))
				{
					continue;
				}
				FlowSwitch<string> flowSwitch = treeNode2.Obj as FlowSwitch<string>;
				if (flowSwitch.Default != null)
				{
					TreeNode treeNode6 = new TreeNode(flowSwitch.Default);
					treeNode2.SubNodes.Add(treeNode6);
					if (treeNode6 != null && !hashSet.Contains(treeNode6.GetHashCode()))
					{
						queue.Enqueue(treeNode6);
					}
				}
				if (flowSwitch.Cases == null)
				{
					continue;
				}
				foreach (KeyValuePair<string, FlowNode> @case in flowSwitch.Cases)
				{
					TreeNode treeNode7 = new TreeNode(@case.Value);
					treeNode2.SubNodes.Add(treeNode7);
					if (treeNode7 != null && !hashSet.Contains(treeNode7.GetHashCode()))
					{
						queue.Enqueue(treeNode7);
					}
				}
			}
		}
		return new Tree(treeNode);
	}

	private void BuildFlowChartTreeCycleDfs(object root, HashSet<int> inProgress, HashSet<int> done)
	{
		Stack<Tuple<object, bool>> stack = new Stack<Tuple<object, bool>>();
		if (root != null)
		{
			stack.Push(Tuple.Create(root, item2: false));
		}
		while (stack.Count > 0)
		{
			Tuple<object, bool> tuple = stack.Pop();
			object item = tuple.Item1;
			bool item2 = tuple.Item2;
			if (item == null)
			{
				continue;
			}
			int hashCode = item.GetHashCode();
			if (item2)
			{
				inProgress.Remove(hashCode);
				done.Add(hashCode);
			}
			else
			{
				if (done.Contains(hashCode))
				{
					continue;
				}
				if (inProgress.Contains(hashCode))
				{
					ReportFlowChartCycle(item);
					break;
				}
				inProgress.Add(hashCode);
				stack.Push(Tuple.Create(item, item2: true));
				Type type = item.GetType();
				if (type == typeof(FlowStep))
				{
					FlowStep flowStep = item as FlowStep;
					if (flowStep.Next != null)
					{
						stack.Push(Tuple.Create((object)flowStep.Next, false));
					}
				}
				else if (type == typeof(FlowDecision))
				{
					FlowDecision flowDecision = item as FlowDecision;
					if (flowDecision.False != null)
					{
						stack.Push(Tuple.Create((object)flowDecision.False, false));
					}
					if (flowDecision.True != null)
					{
						stack.Push(Tuple.Create((object)flowDecision.True, false));
					}
				}
				else
				{
					if (!(type == typeof(FlowSwitch<string>)))
					{
						continue;
					}
					FlowSwitch<string> flowSwitch = item as FlowSwitch<string>;
					if (flowSwitch.Cases != null)
					{
						List<FlowNode> list = new List<FlowNode>();
						foreach (KeyValuePair<string, FlowNode> @case in flowSwitch.Cases)
						{
							if (@case.Value != null)
							{
								list.Add(@case.Value);
							}
						}
						for (int num = list.Count - 1; num >= 0; num--)
						{
							stack.Push(Tuple.Create((object)list[num], false));
						}
					}
					if (flowSwitch.Default != null)
					{
						stack.Push(Tuple.Create((object)flowSwitch.Default, false));
					}
				}
			}
		}
	}

	private static List<TreeNode> GetLeafNodes(Tree subtree)
	{
		if (subtree.Root == null)
		{
			return new List<TreeNode>();
		}
		List<TreeNode> list = new List<TreeNode>();
		Queue<TreeNode> queue = new Queue<TreeNode>();
		HashSet<int> hashSet = new HashSet<int>();
		queue.Enqueue(subtree.Root);
		while (queue.Count > 0)
		{
			TreeNode treeNode = queue.Dequeue();
			if (treeNode == null || !hashSet.Add(treeNode.GetHashCode()))
			{
				continue;
			}
			if (treeNode.SubNodes.Count == 0)
			{
				list.Add(treeNode);
				continue;
			}
			foreach (TreeNode subNode in treeNode.SubNodes)
			{
				if (subNode != null && !hashSet.Contains(subNode.GetHashCode()))
				{
					queue.Enqueue(subNode);
				}
			}
		}
		return list;
	}

	private CodePathTreeNode CreateCodePathTree(Tree tree)
	{
		CodePathTreeNode result = null;
		if (tree.Root != null)
		{
			result = tree.Root.Obj as CodePathTreeNode;
			Queue<TreeNode> queue = new Queue<TreeNode>();
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(tree.Root.GetHashCode());
			queue.Enqueue(tree.Root);
			while (queue.Count > 0)
			{
				TreeNode treeNode = queue.Dequeue();
				if (treeNode == null || !(treeNode.Obj is CodePathTreeNode codePathTreeNode))
				{
					continue;
				}
				codePathTreeNode.ClearChildNodes();
				foreach (TreeNode subNode in treeNode.SubNodes)
				{
					if (subNode != null)
					{
						if (hashSet.Add(subNode.GetHashCode()))
						{
							queue.Enqueue(subNode);
						}
						CodePathTreeNode childNode = subNode.Obj as CodePathTreeNode;
						codePathTreeNode.AppendChildNode(childNode);
					}
				}
			}
		}
		return result;
	}

	private CodePathTreeNode GetNodeOfSubassemblyActivity(SubassemblyActivity activity, int activityId)
	{
		CodePathTreeNode codePathTreeNode = null;
		Type type = activity.GetType();
		if (!s_propertyDescriptorCache.TryGetValue(type, out var value))
		{
			value = TypeDescriptor.GetProperties(type);
			s_propertyDescriptorCache[type] = value;
		}
		foreach (PropertyDescriptor item in value)
		{
			if (!(item.Attributes[typeof(GeometryAutoNumberingAttribute)] is GeometryAutoNumberingAttribute geometryAutoNumberingAttribute))
			{
				continue;
			}
			Type type2 = activity.GetType();
			bool flag = true;
			if (!string.IsNullOrEmpty(geometryAutoNumberingAttribute.CheckPropertyName) && type2.GetProperty(geometryAutoNumberingAttribute.CheckPropertyName).GetValue(activity, null) is InArgument<bool> { Expression: { } expression })
			{
				flag = bool.Parse(expression.ToString());
			}
			if (!flag || !(type2.GetProperty(item.Name).GetValue(activity, null) is InArgument<string> { Expression: not null } inArgument2))
			{
				continue;
			}
			string text = inArgument2.Expression.ToString();
			if (!string.IsNullOrEmpty(text))
			{
				if (codePathTreeNode == null)
				{
					codePathTreeNode = new CodePathTreeNode(activity.DisplayName, activityId);
					codePathTreeNode.GeometryNames.Add(text);
				}
				else
				{
					codePathTreeNode.GeometryNames.Add(text);
				}
			}
		}
		return codePathTreeNode;
	}
}
