using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(HasDaylightDesigner))]
[HasDaylightAutoNumbering]
[ToolboxBitmap(typeof(HasDaylight), "Resources.Icons.HasDaylight_toolbox.ico")]
public class HasDaylight : CustomCondition
{
	[CustomCategory(typeof(Resource), "Category_Condition", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SurfaceTarget", 1)]
	[RequiredArgument]
	[CanReferParameterName]
	[Editor(typeof(TargetComboBox<SurfaceTarget>), typeof(PropertyValueEditor))]
	public InArgument<SurfaceTarget> SurfaceTarget { get; set; }

	[CustomCategory(typeof(Resource), "Category_Condition", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_FromPoint", 2)]
	[RequiredArgument]
	[CanReferGeometryName]
	[Editor(typeof(PointComboBox), typeof(PropertyValueEditor))]
	public InArgument<string> FromPoint { get; set; }

	[CustomCategory(typeof(Resource), "Category_Condition", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_Slope", 3)]
	[RequiredArgument]
	[CanReferParameterName]
	[Editor(typeof(TypeInfoTextBox<Grade>), typeof(PropertyValueEditor))]
	public InArgument<double> Slope { get; set; }

	[CustomCategory(typeof(Resource), "Category_Condition", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ReverseSlopeDirection", 4)]
	[RequiredArgument]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ReverseSlopeDirection { get; set; }

	[RequiredArgument]
	[Browsable(false)]
	public InOutArgument<Geometry> Geometry { get; set; }

	[RequiredArgument]
	[Browsable(false)]
	public InArgument<int> Side { get; set; }

	static HasDaylight()
	{
		AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
		attributeTableBuilder.AddCustomAttributes(typeof(HasDaylight), typeof(HasDaylight).GetProperty("DisplayName", typeof(string)), new CustomCategoryAttribute(typeof(Resource), "Category_Misc", 1));
		attributeTableBuilder.AddCustomAttributes(typeof(HasDaylight), typeof(HasDaylight).GetProperty("DisplayName", typeof(string)), new CustomDisplayNameAttribute(typeof(Resource), "DisplayName", 1));
		MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
	}

	public HasDaylight()
	{
		ReverseSlopeDirection = false;
		Slope = 1.0;
	}

	protected override void SubExecute(NativeActivityContext context)
	{
		SurfaceTarget value = context.GetValue(SurfaceTarget);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point = context.GetValue(Geometry).GetPoint(context.GetValue(FromPoint));
		double value2 = context.GetValue(Slope);
		bool value3 = context.GetValue(ReverseSlopeDirection);
		int num = ((context.GetValue(Side) != 1) ? 1 : (-1));
		bool isTowardRight = true;
		if ((num == -1 && !value3) || (num == 1 && value3))
		{
			isTowardRight = false;
		}
		if (value != null && value.IsValid && point != null)
		{
			bool value4 = value.hasDaylightPoint(point, value2, isTowardRight);
			context.SetValue(Result, value4);
		}
		else
		{
			context.SetValue(Result, value: false);
		}
	}
}
