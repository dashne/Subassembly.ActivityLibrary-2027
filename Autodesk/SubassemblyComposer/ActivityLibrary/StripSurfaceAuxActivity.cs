using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(AuxSurfaceLinkDesigner))]
[StripSurfaceAutoNumbering]
[ToolboxBitmap(typeof(StripSurfaceAuxActivity), "Resources.Icons.aux_surface_link_toolbox.ico")]
public class StripSurfaceAuxActivity : StripSurfaceActivity
{
	[Browsable(false)]
	public override bool IsCreatingAux => true;

	static StripSurfaceAuxActivity()
	{
		AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
		attributeTableBuilder.AddCustomAttributes(typeof(StripSurfaceAuxActivity), typeof(StripSurfaceAuxActivity).GetProperty("LinkNumber", typeof(InArgument<string>)), new AuxLinkAutoNumberingAttribute("StartPointName", "EndPointName"));
		attributeTableBuilder.AddCustomAttributes(typeof(StripSurfaceAuxActivity), typeof(StripSurfaceAuxActivity).GetProperty("StartPointName", typeof(InArgument<string>)), new AuxPointAutoNumberingAttribute());
		attributeTableBuilder.AddCustomAttributes(typeof(StripSurfaceAuxActivity), typeof(StripSurfaceAuxActivity).GetProperty("EndPointName", typeof(InArgument<string>)), new AuxPointAutoNumberingAttribute());
		attributeTableBuilder.AddCustomAttributes(typeof(StripSurfaceAuxActivity), typeof(StripSurfaceAuxActivity).GetProperty("LinkCodes", typeof(List<InArgument<string>>)), new BrowsableAttribute(browsable: false));
		attributeTableBuilder.AddCustomAttributes(typeof(StripSurfaceAuxActivity), typeof(StripSurfaceAuxActivity).GetProperty("StartPointCodes", typeof(List<InArgument<string>>)), new BrowsableAttribute(browsable: false));
		attributeTableBuilder.AddCustomAttributes(typeof(StripSurfaceAuxActivity), typeof(StripSurfaceAuxActivity).GetProperty("EndPointCodes", typeof(List<InArgument<string>>)), new BrowsableAttribute(browsable: false));
		MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
	}

	public StripSurfaceAuxActivity()
	{
		PropertyCollection_Set(this.PropertyName((StripSurfaceAuxActivity p) => p.IsCreatingAux), IsCreatingAux);
	}
}
