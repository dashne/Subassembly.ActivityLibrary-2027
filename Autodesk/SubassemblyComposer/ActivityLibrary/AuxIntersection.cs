using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(AuxLinkIntersectionDesigner))]
[CreatePointAutoNumbering]
[ToolboxBitmap(typeof(CreatePoint), "Resources.Icons.aux_intersection_toolbox.ico")]
public class AuxIntersection : LinkIntersection
{
	[CustomCategory(typeof(Resource), "Category_Point", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_PointNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[AuxPointAutoNumbering]
	public override InArgument<string> PointNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((AuxIntersection p) => p.PointNumber)) as InArgument<string>;
		}
		set
		{
			_pointNumber = PropertyCollection_Get(this.PropertyName((AuxIntersection p) => p.PointNumber)) as InArgument<string>;
			PropertyCollection_Set(this.PropertyName((AuxIntersection p) => p.PointNumber), value);
		}
	}

	[Browsable(false)]
	[CustomCategory(typeof(Resource), "Category_Point", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_PointCodes", 2)]
	[Editor(typeof(PointCodeComboBox), typeof(PropertyValueEditor))]
	public new List<InArgument<string>> PointCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((AuxIntersection p) => p.PointCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((AuxIntersection p) => p.PointCodes), value);
		}
	}

	[Browsable(false)]
	public override bool IsCreatingAux => true;

	public AuxIntersection()
	{
		PropertyCollection_Set(this.PropertyName((AuxIntersection p) => p.IsCreatingAux), IsCreatingAux);
	}
}
