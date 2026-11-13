using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

public enum CurveType
{
	[EnumItemProcessor(typeof(ArcGeneral))]
	[EnumDisplayName(typeof(Resource), "CurveType_ArcGeneral")]
	ArcGeneral,
	[EnumItemProcessor(typeof(Arc3Points))]
	[EnumDisplayName(typeof(Resource), "CurveType_Arc3Points")]
	Arc3Points,
	[EnumItemProcessor(typeof(ParabolaGeneral))]
	[EnumDisplayName(typeof(Resource), "CurveType_ParabolaGeneral")]
	ParabolaGeneral
}
