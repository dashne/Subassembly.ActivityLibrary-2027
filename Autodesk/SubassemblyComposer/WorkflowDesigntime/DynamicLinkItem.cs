using System;
using System.Activities.Presentation.Model;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class DynamicLinkItem
{
	public ModelProperty modelProperty;

	public Type propertyType;

	public Type expressionType;

	public Type genericType;

	public Type argumentType;

	public string expressionText;

	public PropertyOption propertyOption;

	public bool isCode;

	public DynamicLinkItem(ModelProperty prop)
	{
		modelProperty = prop;
	}
}
