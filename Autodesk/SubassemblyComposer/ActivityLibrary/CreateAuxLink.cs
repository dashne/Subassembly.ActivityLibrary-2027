using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(CreateAuxLinkDesigner))]
[CreateLinkAutoNumbering]
[ToolboxBitmap(typeof(CreateAuxLink), "Resources.Icons.aux_link_toolbox.ico")]
public class CreateAuxLink : CreateLink
{
	[CustomCategory(typeof(Resource), "Category_Position", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StartPoint", 1)]
	[Editor(typeof(IsEnabledMixPointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public override InArgument<string> StartPoint
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateAuxLink p) => p.StartPoint)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateAuxLink p) => p.StartPoint), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Position", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_EndPoint", 2)]
	[Editor(typeof(IsEnabledMixPointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public override InArgument<string> EndPoint
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateAuxLink p) => p.EndPoint)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateAuxLink p) => p.EndPoint), value);
		}
	}

	[Browsable(false)]
	public override bool IsCreatingAux => true;

	public CreateAuxLink()
	{
		PropertyCollection_Set(this.PropertyName((CreateAuxLink p) => p.IsCreatingAux), IsCreatingAux);
	}

	static CreateAuxLink()
	{
		AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
		attributeTableBuilder.AddCustomAttributes(typeof(CreateAuxLink), typeof(CreateAuxLink).GetProperty("LinkNumber", typeof(InArgument<string>)), new AuxLinkAutoNumberingAttribute("StartPoint", "EndPoint"));
		attributeTableBuilder.AddCustomAttributes(typeof(CreateAuxLink), typeof(CreateAuxLink).GetProperty("LinkCodes", typeof(List<InArgument<string>>)), new BrowsableAttribute(browsable: false));
		MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
	}
}
