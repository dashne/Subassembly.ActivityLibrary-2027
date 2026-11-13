using System;
using System.Activities.Expressions;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class CorrectCodeReferenceHelper : TraverseWorkflow
{
	private string OldCodeName { get; set; }

	private string NewCodeName { get; set; }

	private Type CodeTypeAttribute { get; set; }

	public static void Execute(string oldValue, string newValue, CodeType type)
	{
		new CorrectCodeReferenceHelper(oldValue, newValue, type).Run();
	}

	public CorrectCodeReferenceHelper(string oldValue, string newValue, CodeType type)
	{
		OldCodeName = oldValue;
		NewCodeName = newValue;
		switch (type)
		{
		case CodeType.Point:
			CodeTypeAttribute = typeof(CanReferPointCodeNameAttribute);
			break;
		case CodeType.Link:
			CodeTypeAttribute = typeof(CanReferLinkCodeNameAttribute);
			break;
		case CodeType.Shape:
			CodeTypeAttribute = typeof(CanReferShapeCodeNameAttribute);
			break;
		}
	}

	public void Run()
	{
		if (!string.IsNullOrEmpty(OldCodeName))
		{
			Traverse();
		}
	}

	protected override void TraverseItem(ModelItem modelItem, List<ModelItem> modelItems)
	{
		foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(modelItem.GetCurrentValue()))
		{
			Attribute attr = property.Attributes[CodeTypeAttribute];
			if (CastType(attr) != null)
			{
				ModelProperty modelProperty = modelItem.Properties[property.Name];
				if (modelProperty != null && modelProperty.Value != null)
				{
					DynamicLinkHelper.CheckForReference(modelProperty, OldCodeName, typeof(Literal<>));
				}
			}
		}
	}

	private object CastType(Attribute attr)
	{
		if (CodeTypeAttribute == typeof(CanReferPointCodeNameAttribute))
		{
			return attr as CanReferPointCodeNameAttribute;
		}
		if (CodeTypeAttribute == typeof(CanReferLinkCodeNameAttribute))
		{
			return attr as CanReferLinkCodeNameAttribute;
		}
		if (CodeTypeAttribute == typeof(CanReferShapeCodeNameAttribute))
		{
			return attr as CanReferShapeCodeNameAttribute;
		}
		return null;
	}
}
