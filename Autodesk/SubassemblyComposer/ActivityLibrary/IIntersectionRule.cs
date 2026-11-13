using System.Activities;
using System.Collections.Generic;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal interface IIntersectionRule : ICreationRule, ICustomAccessProperty
{
	List<Point> Execute(CodeActivityContext context, Geometry geom, int side, IRecordError recordError);
}
