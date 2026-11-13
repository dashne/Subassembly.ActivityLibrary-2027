using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

public enum IntersectionType
{
	[EnumItemProcessor(typeof(IntersectionTwoLinks))]
	[EnumDisplayName(typeof(Resource), "IntersectionType_DisplayName_IntersectionTwoLinks")]
	IntersectionTwoLinks = 1,
	[EnumItemProcessor(typeof(IntersectionLinkPointSlope))]
	[EnumDisplayName(typeof(Resource), "IntersectionType_DisplayName_IntersectionLinkPointSlope")]
	IntersectionLinkPointSlope,
	[EnumItemProcessor(typeof(IntersectionTwoPointsSlope))]
	[EnumDisplayName(typeof(Resource), "IntersectionType_DisplayName_IntersectionTwoPointsSlope")]
	IntersectionTwoPointsSlope
}
