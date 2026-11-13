using System.Activities;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal interface IRecordError
{
	bool ShowErrors { get; set; }

	void RecordErrorInfo(ActivityContext context, ErrorLevel level, string msg);
}
