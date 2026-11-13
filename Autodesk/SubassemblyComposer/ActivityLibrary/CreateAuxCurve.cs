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

[Designer(typeof(CreateAuxCurveDesigner))]
[CreateCurveAutoNumbering]
[ToolboxBitmap(typeof(CreateAuxCurve), "Resources.Icons.aux_curve_toolbox.ico")]
public class CreateAuxCurve : CreateCurve
{
	[Browsable(false)]
	public override bool IsCreatingAux => true;

	[CustomCategory(typeof(Resource), "Category_Link", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_LinkNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[AuxLinkAutoNumbering("StartPoint", "EndPoint")]
	public new InArgument<string> LinkNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateAuxCurve p) => p.LinkNumber)) as InArgument<string>;
		}
		set
		{
			_linkNumber = LinkNumber;
			PropertyCollection_Set(this.PropertyName((CreateAuxCurve p) => p.LinkNumber), value);
		}
	}

	[Browsable(false)]
	[CustomCategory(typeof(Resource), "Category_Link", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_LinkCodes", 2)]
	[Editor(typeof(LinkCodeComboBox), typeof(PropertyValueEditor))]
	public new List<InArgument<string>> LinkCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateAuxCurve p) => p.LinkCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateAuxCurve p) => p.LinkCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StartPoint", 1)]
	[Editor(typeof(IsEnabledMixPointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public new InArgument<string> StartPoint
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateAuxCurve p) => p.StartPoint)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateAuxCurve p) => p.StartPoint), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_GeometryProperties", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_EndPoint", 10)]
	[Editor(typeof(IsEnabledMixPointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public new InArgument<string> EndPoint
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateAuxCurve p) => p.EndPoint)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateAuxCurve p) => p.EndPoint), value);
		}
	}

	public CreateAuxCurve()
	{
		PropertyCollection_Set(this.PropertyName((CreateAuxCurve p) => p.IsCreatingAux), IsCreatingAux);
	}
}
