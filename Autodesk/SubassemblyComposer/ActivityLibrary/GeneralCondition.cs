using System.Activities;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(GeneralConditionDesigner))]
[GeneralConditionAutoNumbering]
[ToolboxBitmap(typeof(GeneralCondition), "Resources.Icons.general_condition_toolbox.ico")]
public class GeneralCondition : CustomCondition
{
	public const string ExpressionArgumentName = "Expression";

	[CustomCategory(typeof(Resource), "Category_Condition", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_GeneralCondition_Expression", 1)]
	[RequiredArgument]
	[CanReferParameterName]
	public InArgument<bool> Expression { get; set; }

	static GeneralCondition()
	{
		AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
		attributeTableBuilder.AddCustomAttributes(typeof(GeneralCondition), typeof(GeneralCondition).GetProperty("DisplayName", typeof(string)), new CustomCategoryAttribute(typeof(Resource), "Category_Misc", 1));
		attributeTableBuilder.AddCustomAttributes(typeof(GeneralCondition), typeof(GeneralCondition).GetProperty("DisplayName", typeof(string)), new CustomDisplayNameAttribute(typeof(Resource), "DisplayName", 1));
		MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
	}

	protected override void SubExecute(NativeActivityContext context)
	{
		bool value = context.GetValue(Expression);
		context.SetValue(Result, value);
	}
}
