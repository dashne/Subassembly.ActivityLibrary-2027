using System.Activities;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal interface ICurveRule : ICreationRule, ICustomAccessProperty
{
	Link Execute(CodeActivityContext context, Geometry geometry, Point startPoint, Point endPoint, int side, int noIncrements, IRecordError recordError);
}
