using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

public enum RoundOptionEnum : short
{
	[EnumItemProcessor(typeof(ArcRounding))]
	[EnumDisplayName(typeof(Resource), "DaylightRound_RoundOptionEnum_Arc")]
	ArcRounding = 1,
	[EnumItemProcessor(typeof(ParabolicRounding))]
	[EnumDisplayName(typeof(Resource), "DaylightRound_RoundOptionEnum_Parabolic")]
	ParabolicRounding
}
