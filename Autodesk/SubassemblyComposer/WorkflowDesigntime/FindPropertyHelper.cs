using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Collections.Generic;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class FindPropertyHelper : TraverseWorkflow
{
	protected Predicate<ModelProperty> propChecker;

	public List<ModelProperty> Props { get; private set; }

	public static List<ModelProperty> Execute(Predicate<ModelProperty> propChecker)
	{
		FindPropertyHelper findPropertyHelper = new FindPropertyHelper(propChecker);
		findPropertyHelper.Traverse();
		return findPropertyHelper.Props;
	}

	protected FindPropertyHelper(Predicate<ModelProperty> _propChecker)
	{
		propChecker = _propChecker;
		Props = new List<ModelProperty>();
	}

	protected FindPropertyHelper()
	{
	}

	protected override void TraverseItem(ModelItem modelItem, List<ModelItem> modelItems)
	{
		foreach (ModelProperty property in modelItem.Properties)
		{
			if (!property.IsBrowsable)
			{
				continue;
			}
			if ((typeof(Activity).IsAssignableFrom(property.PropertyType) || typeof(FlowNode).IsAssignableFrom(property.PropertyType)) && property.Value != null)
			{
				TraverseItem(property.Value, modelItems);
				continue;
			}
			if (property.IsCollection)
			{
				foreach (ModelItem item in property.Collection)
				{
					TraverseItem(item, modelItems);
				}
				continue;
			}
			if (property.IsDictionary)
			{
				foreach (KeyValuePair<ModelItem, ModelItem> item2 in property.Dictionary)
				{
					TraverseItem(item2.Value, modelItems);
				}
				continue;
			}
			string text = property?.Value?.GetCurrentValue().ToString();
			if (text != null && text.Contains("Argument"))
			{
				TraverseItem(property.Value, modelItems);
			}
			if (propChecker(property))
			{
				Props.Add(property);
			}
		}
	}
}
