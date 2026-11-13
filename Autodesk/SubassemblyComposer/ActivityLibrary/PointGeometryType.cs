using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

public enum PointGeometryType
{
	[EnumItemProcessor(typeof(DeltaXAndDeltaY))]
	[EnumDisplayName(typeof(Resource), "PointGeometryType_DisplayName_DeltaXAndDeltaY")]
	DeltaXAndDeltaY = 1,
	[EnumItemProcessor(typeof(SlopeAndDeltaX))]
	[EnumDisplayName(typeof(Resource), "PointGeometryType_DisplayName_SlopeAndDeltaX")]
	SlopeAndDeltaX,
	[EnumItemProcessor(typeof(SlopeToSurface))]
	[EnumDisplayName(typeof(Resource), "PointGeometryType_DisplayName_SlopeToSurface")]
	SlopeToSurface,
	[EnumItemProcessor(typeof(DeltaXOnSurface))]
	[EnumDisplayName(typeof(Resource), "PointGeometryType_DisplayName_DeltaXOnSurface")]
	DeltaXOnSurface,
	[EnumItemProcessor(typeof(SlopeAndDeltaY))]
	[EnumDisplayName(typeof(Resource), "PointGeometryType_DisplayName_SlopeAndDeltaY")]
	SlopeAndDeltaY,
	[EnumItemProcessor(typeof(AngleAndDeltaX))]
	[EnumDisplayName(typeof(Resource), "PointGeometryType_DisplayName_AngleAndDeltaX")]
	AngleAndDeltaX,
	[EnumItemProcessor(typeof(AngleAndDeltaY))]
	[EnumDisplayName(typeof(Resource), "PointGeometryType_DisplayName_AngleAndDeltaY")]
	AngleAndDeltaY,
	[EnumItemProcessor(typeof(AngleAndDistance))]
	[EnumDisplayName(typeof(Resource), "PointGeometryType_DisplayName_AngleAndDistance")]
	AngleAndDistance,
	[EnumItemProcessor(typeof(InterpolatePoint))]
	[EnumDisplayName(typeof(Resource), "PointGeometryType_DisplayName_InterpolatePoint")]
	InterpolatePoint
}
