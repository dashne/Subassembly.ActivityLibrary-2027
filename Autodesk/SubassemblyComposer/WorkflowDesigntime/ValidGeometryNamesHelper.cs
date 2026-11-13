using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class ValidGeometryNamesHelper
{
	private Func<string, bool> Predicate { get; set; }

	private static List<string> GetRegularPointNamesInternal(CodePathTreeNode codePathTree, int activityId, bool includeOrigin)
	{
		return new ValidGeometryNamesHelper(GeometryNamePatterns.IsRegularPointName).Run(codePathTree, activityId, includeOrigin);
	}

	private static List<string> GetLoopEndPointNamesInternal(CodePathTreeNode codePathTree, int activityId)
	{
		return (from name in new ValidGeometryNamesHelper(GeometryNamePatterns.IsLoopName).Run(codePathTree, activityId, includeOrigin: false)
			select $"{name}.EndPoint").ToList();
	}

	private static List<string> GetAllValidPointNamesInternal(CodePathTreeNode codePathTree, int activityId, bool includeOrigin)
	{
		List<string> list = new List<string>();
		list.AddRange(GetRegularPointNamesInternal(codePathTree, activityId, includeOrigin));
		list.AddRange(GetLoopEndPointNamesInternal(codePathTree, activityId));
		return list;
	}

	private static List<string> GetAllValidNonAuxPointNamesInternal(CodePathTreeNode codePathTree, int activityId, bool includeOrigin)
	{
		List<string> list = new List<string>();
		ValidGeometryNamesHelper validGeometryNamesHelper = new ValidGeometryNamesHelper(GeometryNamePatterns.IsNonAuxRegularPointName);
		list.AddRange(validGeometryNamesHelper.Run(codePathTree, activityId, includeOrigin));
		list.AddRange(GetLoopEndPointNamesInternal(codePathTree, activityId));
		return list;
	}

	private static List<string> GetLinkNamesInternal(CodePathTreeNode codePathTree, int activityId)
	{
		return new ValidGeometryNamesHelper(GeometryNamePatterns.IsLinkName).Run(codePathTree, activityId, includeOrigin: false);
	}

	private static List<string> GetLoopNamesInternal(CodePathTreeNode codePathTree, int activityId)
	{
		return new ValidGeometryNamesHelper(GeometryNamePatterns.IsLoopName).Run(codePathTree, activityId, includeOrigin: false);
	}

	public static ObservableCollection<string> GetRegularPointNames(CodePathTreeNode codePathTree, int activityId, bool includeOrigin)
	{
		return new ObservableCollection<string>(GetRegularPointNamesInternal(codePathTree, activityId, includeOrigin));
	}

	public static ObservableCollection<string> GetLoopEndPointNames(CodePathTreeNode codePathTree, int activityId)
	{
		return new ObservableCollection<string>(GetLoopEndPointNamesInternal(codePathTree, activityId));
	}

	public static ObservableCollection<string> GetAllValidPointNames(CodePathTreeNode codePathTree, int activityId, bool includeOrigin)
	{
		return new ObservableCollection<string>(GetAllValidPointNamesInternal(codePathTree, activityId, includeOrigin));
	}

	public static ObservableCollection<string> GetAllValidNonAuxPointNames(CodePathTreeNode codePathTree, int activityId, bool includeOrigin)
	{
		return new ObservableCollection<string>(GetAllValidNonAuxPointNamesInternal(codePathTree, activityId, includeOrigin));
	}

	public static ObservableCollection<string> GetLinkNames(CodePathTreeNode codePathTree, int activityId)
	{
		return new ObservableCollection<string>(GetLinkNamesInternal(codePathTree, activityId));
	}

	public static ObservableCollection<string> GetLoopNames(CodePathTreeNode codePathTree, int activityId)
	{
		return new ObservableCollection<string>(GetLoopNamesInternal(codePathTree, activityId));
	}

	public static bool RefreshAllValidPointNames(List<string> pointNames, CodePathTreeNode codePathTree, int activityId, bool includeOrigin)
	{
		bool result = CopyCollection(GetAllValidPointNamesInternal(codePathTree, activityId, includeOrigin), pointNames);
		pointNames.Sort(new GeometryNameComparer());
		return result;
	}

	public static bool RefreshNonAuxPointNames(List<string> pointNames, CodePathTreeNode codePathTree, int activityId, bool includeOrigin)
	{
		bool result = CopyCollection(GetAllValidNonAuxPointNamesInternal(codePathTree, activityId, includeOrigin), pointNames);
		pointNames.Sort(new GeometryNameComparer());
		return result;
	}

	public static bool RefreshLinkNames(List<string> linkNames, CodePathTreeNode codePathTree, int activityId)
	{
		bool result = CopyCollection(GetLinkNamesInternal(codePathTree, activityId), linkNames);
		linkNames.Sort(new GeometryNameComparer());
		return result;
	}

	private ValidGeometryNamesHelper(Func<string, bool> predicate)
	{
		Predicate = predicate;
	}

	private List<string> Run(CodePathTreeNode codePathTree, int activityId, bool includeOrigin)
	{
		List<string> list = new List<string>();
		if (codePathTree != null)
		{
			CodePathTreeNode node = codePathTree.FindChildNodeById(activityId);
			FindParentGeometryNames(node, list);
			if (includeOrigin)
			{
				list.Insert(0, GlobleString.String_Origin);
			}
		}
		return list;
	}

	private void FindParentGeometryNames(CodePathTreeNode node, List<string> names)
	{
		if (node == null)
		{
			return;
		}
		foreach (CodePathTreeNode allParentNode in node.GetAllParentNodes())
		{
			if (allParentNode == null)
			{
				continue;
			}
			foreach (string geometryName in allParentNode.GeometryNames)
			{
				if (Predicate(geometryName))
				{
					names.Add(geometryName);
				}
			}
		}
	}

	private static bool CopyCollection(List<string> from, List<string> to)
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
