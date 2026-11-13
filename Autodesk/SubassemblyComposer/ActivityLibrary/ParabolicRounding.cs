using System.Collections.Generic;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class ParabolicRounding : GeneralRounding
{
	protected override GeometryUtility.RoundResult GetRoundingCurve(Point cornerPt, Point pt1, Point pt2, bool roundByLenth, double roundParam, int sampleCount, ref List<Point> curveRet)
	{
		return GeometryUtility.GetRoundingParabola(cornerPt, pt1, pt2, roundByLenth, roundParam, sampleCount, ref curveRet);
	}
}
