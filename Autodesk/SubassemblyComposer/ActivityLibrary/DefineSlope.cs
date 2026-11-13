using System.Activities;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(DefineSlopeDesigner))]
[DefineSlopeAutoNumbering]
[ToolboxBitmap(typeof(DefineSlope), "Resources.Icons.define_slope_toolbox.ico")]
public class DefineSlope : CustomActivity
{
	static DefineSlope()
	{
		AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
		attributeTableBuilder.AddCustomAttributes(typeof(DefineSlope), typeof(DefineSlope).GetProperty("DisplayName", typeof(string)), new CustomCategoryAttribute(typeof(Resource), "Category_Misc", 1));
		attributeTableBuilder.AddCustomAttributes(typeof(InternalVariableDefine), typeof(InternalVariableDefine).GetProperty("DisplayName", typeof(string)), new CustomDisplayNameAttribute(typeof(Resource), "DisplayName", 1));
		MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
	}

	protected override void LayoutExecute(CodeActivityContext context)
	{
		InternalExecute(context);
	}

	protected override void RoadwayExecute(CodeActivityContext context)
	{
		InternalExecute(context);
	}

	private void InternalExecute(CodeActivityContext context)
	{
	}
}
