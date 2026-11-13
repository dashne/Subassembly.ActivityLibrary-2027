using System.Activities;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal interface IDaylightRoundRule : ICreationRule, ICustomAccessProperty
{
	Link Execute(CodeActivityContext context, Point ptOutSurface, Point ptOnSurface, SurfaceTarget surface, int sampleCount, ref GeometryUtility.RoundResult result);
}
