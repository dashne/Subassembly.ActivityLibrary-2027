using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

public enum RoundByEnum
{
	[EnumDisplayName(typeof(Resource), "DaylightRound_RoundByEnum_ByLength")]
	ByLength,
	[EnumDisplayName(typeof(Resource), "DaylightRound_RoundByEnum_ByRadius")]
	ByRadius
}
