using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.GeomMath;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(HasIntersectionDesigner))]
[HasIntersectionAutoNumbering]
[ToolboxBitmap(typeof(HasIntersection), "Resources.Icons.HasIntersection_toolbox.ico")]
public class HasIntersection : CustomCondition
{
	[CustomCategory(typeof(Resource), "Category_Condition", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FirstLink", 1)]
	[RequiredArgument]
	[CanReferGeometryName]
	[Editor(typeof(LinkComboBox), typeof(PropertyValueEditor))]
	public InArgument<string> FirstLink { get; set; }

	[CustomCategory(typeof(Resource), "Category_Condition", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ExtendFirstLink", 2)]
	[RequiredArgument]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ExtendFirstLink { get; set; }

	[CustomCategory(typeof(Resource), "Category_Condition", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SecondLink", 3)]
	[RequiredArgument]
	[CanReferGeometryName]
	[Editor(typeof(LinkComboBox), typeof(PropertyValueEditor))]
	public InArgument<string> SecondLink { get; set; }

	[CustomCategory(typeof(Resource), "Category_Condition", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ExtendSecondLink", 4)]
	[RequiredArgument]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ExtendSecondLink { get; set; }

	[RequiredArgument]
	[Browsable(false)]
	public InOutArgument<Geometry> Geometry { get; set; }

	static HasIntersection()
	{
		AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
		attributeTableBuilder.AddCustomAttributes(typeof(HasIntersection), typeof(HasIntersection).GetProperty("DisplayName", typeof(string)), new CustomCategoryAttribute(typeof(Resource), "Category_Misc", 1));
		attributeTableBuilder.AddCustomAttributes(typeof(HasIntersection), typeof(HasIntersection).GetProperty("DisplayName", typeof(string)), new CustomDisplayNameAttribute(typeof(Resource), "DisplayName", 1));
		MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
	}

	public HasIntersection()
	{
		ExtendFirstLink = false;
		ExtendSecondLink = false;
	}

	protected override void SubExecute(NativeActivityContext context)
	{
		Geometry value = context.GetValue(Geometry);
		string value2 = context.GetValue(FirstLink);
		Link link = value.GetLink(value2);
		string value3 = context.GetValue(SecondLink);
		Link link2 = value.GetLink(value3);
		bool value4 = context.GetValue(ExtendFirstLink);
		bool value5 = context.GetValue(ExtendSecondLink);
		if (link == null || link2 == null)
		{
			context.SetValue(Result, value: false);
			return;
		}
		bool value6 = GeometryFactory.LinkIntersection(link, link2, value4, value5).Count > 0;
		context.SetValue(Result, value6);
	}
}
