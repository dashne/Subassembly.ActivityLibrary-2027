using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(CustomErrorReportDesigner))]
[CustomErrorReportActivityAutoNumbering]
[ToolboxBitmap(typeof(CustomErrorReportActivity), "Resources.Icons.custom_error_report_toolbox.ico")]
public class CustomErrorReportActivity : CustomActivity
{
	[RequiredArgument]
	[CustomCategory(typeof(Resource), "Category_General", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_CustomErrorReport_Message", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	public InArgument<string> Message { get; set; }

	[RequiredArgument]
	[CustomCategory(typeof(Resource), "Category_General", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_CustomErrorReport_Level", 2)]
	[Editor(typeof(EnumComboBox<ErrorLevel>), typeof(PropertyValueEditor))]
	public ErrorLevel ErrorLevel { get; set; }

	public CustomErrorReportActivity()
	{
		ErrorLevel = ErrorLevel.Error;
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
		string value = context.GetValue(Message);
		RecordErrorInfo(context, ErrorLevel, value);
	}
}
