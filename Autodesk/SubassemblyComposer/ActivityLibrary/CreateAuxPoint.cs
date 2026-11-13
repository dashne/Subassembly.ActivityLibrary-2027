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

[Designer(typeof(CreateAuxPointDesigner))]
[CreatePointAutoNumbering]
[ToolboxBitmap(typeof(CreateAuxPoint), "Resources.Icons.aux_point_toolbox.ico")]
public class CreateAuxPoint : CreatePoint
{
	[Browsable(false)]
	public override bool IsCreatingAux => true;

	[CustomCategory(typeof(Resource), "Category_Point", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_PointNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[AuxPointAutoNumbering]
	public new InArgument<string> PointNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateAuxPoint p) => p.PointNumber)) as InArgument<string>;
		}
		set
		{
			_pointNumber = PropertyCollection_Get(this.PropertyName((CreateAuxPoint p) => p.PointNumber)) as InArgument<string>;
			PropertyCollection_Set(this.PropertyName((CreateAuxPoint p) => p.PointNumber), value);
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
			return PropertyCollection_Get(this.PropertyName((CreateAuxPoint p) => p.PointCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateAuxPoint p) => p.PointCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_AutoAddLink", 4)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AutoLink_GeometryName", 2)]
	[Editor(typeof(AutoLinkNameTextBox), typeof(PropertyValueEditor))]
	[AuxLinkAutoNumbering("AutoLink", "FromPoint", "PointNumber", true)]
	public new InArgument<string> AutoLinkGeometryName
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateAuxPoint p) => p.AutoLinkGeometryName)) as InArgument<string>;
		}
		set
		{
			_linkName = AutoLinkGeometryName;
			PropertyCollection_Set(this.PropertyName((CreateAuxPoint p) => p.AutoLinkGeometryName), value);
		}
	}

	[Browsable(false)]
	[CustomCategory(typeof(Resource), "Category_AutoAddLink", 4)]
	[CustomDisplayName(typeof(Resource), "DisplayName_AutoLink_Codes", 3)]
	[Editor(typeof(AutoLinkCodesTextBox), typeof(PropertyValueEditor))]
	public new List<InArgument<string>> AutoLinkCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateAuxPoint p) => p.AutoLinkCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateAuxPoint p) => p.AutoLinkCodes), value);
		}
	}

	public CreateAuxPoint()
	{
		PropertyCollection_Set(this.PropertyName((CreateAuxPoint p) => p.IsCreatingAux), IsCreatingAux);
	}
}
