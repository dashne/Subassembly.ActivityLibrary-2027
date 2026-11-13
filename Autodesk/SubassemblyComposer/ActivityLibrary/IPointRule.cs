using System.Activities;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal interface IPointRule : ICreationRule, ICustomAccessProperty
{
	Point LayoutExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int sider, IRecordError recordError);

	Point RoadwayExecute(CodeActivityContext context, Geometry geom, Point fromPoint, int sider, IRecordError recordError);
}
