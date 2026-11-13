using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(GetAuxMarkPointDesigner))]
[ToolboxBitmap(typeof(GetMarkPointActivity), "Resources.Icons.get_aux_mark_point_toolbox.ico")]
public class GetAuxMarkPointActivity : GetMarkPointActivity
{
	[Browsable(false)]
	public override bool IsCreatingAux => true;

	static GetAuxMarkPointActivity()
	{
		AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
		attributeTableBuilder.AddCustomAttributes(typeof(GetAuxMarkPointActivity), typeof(GetAuxMarkPointActivity).GetProperty("PointNumber", typeof(InArgument<string>)), new AuxPointAutoNumberingAttribute());
		attributeTableBuilder.AddCustomAttributes(typeof(GetAuxMarkPointActivity), typeof(GetAuxMarkPointActivity).GetProperty("PointCodes", typeof(List<InArgument<string>>)), new BrowsableAttribute(browsable: false));
		MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
	}

	public GetAuxMarkPointActivity()
	{
		PropertyCollection_Set(this.PropertyName((GetAuxMarkPointActivity p) => p.IsCreatingAux), IsCreatingAux);
	}
}
