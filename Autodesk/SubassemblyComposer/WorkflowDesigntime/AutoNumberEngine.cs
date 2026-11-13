using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class AutoNumberEngine : TraverseWorkflow
{
	private static AutoNumberEngine Instance;

	private int activityId;

	private Dictionary<string, int> GeometryNumbers;

	public static int NextActivityId
	{
		get
		{
			if (Instance == null)
			{
				Instance = new AutoNumberEngine();
				Instance.Run();
			}
			return Instance.GetActivityId();
		}
	}

	public static int NextGeometryNumber(Type type)
	{
		if (Instance == null)
		{
			Instance = new AutoNumberEngine();
			Instance.Run();
		}
		return Instance.GetGeometryNumber(type);
	}

	public static void Reset()
	{
		Instance = null;
	}

	private AutoNumberEngine()
	{
		activityId = 0;
		GeometryNumbers = new Dictionary<string, int>();
		GeometryNumbers.Add(typeof(PointAutoNumberingAttribute).ToString(), 0);
		GeometryNumbers.Add(typeof(AuxPointAutoNumberingAttribute).ToString(), 0);
		GeometryNumbers.Add(typeof(LinkAutoNumberingAttribute).ToString(), 0);
		GeometryNumbers.Add(typeof(AuxLinkAutoNumberingAttribute).ToString(), 0);
		GeometryNumbers.Add(typeof(ShapeAutoNumberingAttribute).ToString(), 0);
		GeometryNumbers.Add(typeof(OffsetAutoNumberingAttribute).ToString(), 0);
		GeometryNumbers.Add(typeof(LoopAutoNumberingAttribute).ToString(), 0);
	}

	private void Run()
	{
		Traverse();
		activityId++;
		GeometryNumbers[typeof(PointAutoNumberingAttribute).ToString()]++;
		GeometryNumbers[typeof(AuxPointAutoNumberingAttribute).ToString()]++;
		GeometryNumbers[typeof(LinkAutoNumberingAttribute).ToString()]++;
		GeometryNumbers[typeof(AuxLinkAutoNumberingAttribute).ToString()]++;
		GeometryNumbers[typeof(ShapeAutoNumberingAttribute).ToString()]++;
		GeometryNumbers[typeof(OffsetAutoNumberingAttribute).ToString()]++;
		GeometryNumbers[typeof(LoopAutoNumberingAttribute).ToString()]++;
	}

	private int GetActivityId()
	{
		return activityId++;
	}

	private int GetGeometryNumber(Type type)
	{
		return GeometryNumbers[type.ToString()]++;
	}

	protected override void TraverseItem(ModelItem modelItem, List<ModelItem> modelItems)
	{
		int num = ActivityHelpers.GetActivityId(modelItem);
		if (num > 0 && activityId < num)
		{
			activityId = num;
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
			if (!flag)
			{
				continue;
			}
			string stringPropertyValue = ActivityHelpers.GetStringPropertyValue(modelItem, property.Name);
			if (!string.IsNullOrEmpty(stringPropertyValue) && Regex.Match(stringPropertyValue, geometryAutoNumberingAttribute.IDPattern).Success)
			{
				int num2 = int.Parse(Regex.Match(stringPropertyValue, GeometryNamePatterns.DigitalPattern).Value);
				string key = geometryAutoNumberingAttribute.GetType().ToString();
				if (GeometryNumbers[key] < num2)
				{
					GeometryNumbers[key] = num2;
				}
			}
		}
	}
}
