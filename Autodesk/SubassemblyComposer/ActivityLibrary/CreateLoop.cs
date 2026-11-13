using System;
using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(CreateLoopDesigner))]
[CreateLoopAutoNumbering]
[ToolboxBitmap(typeof(CreateLoop), "Resources.Icons.loop_create_toolbox.ico")]
public class CreateLoop : SubassemblyActivity
{
	public const string c_Links = "Links";

	public const char linksSeparator = ',';

	private InArgument<string> _loopNumber;

	public bool InCreate;

	public List<Link> linksToLoop = new List<Link>();

	public Autodesk.SubassemblyComposer.WorkflowEngine.Point loopStart = new Autodesk.SubassemblyComposer.WorkflowEngine.Point();

	public List<Autodesk.SubassemblyComposer.WorkflowEngine.Point> loopPoints = new List<Autodesk.SubassemblyComposer.WorkflowEngine.Point>();

	public List<Link> loopLinks = new List<Link>();

	private const int loopRepetitionDefault = 0;

	private InArgument<int> _loopRepetition = new InArgument<int>
	{
		Expression = 0
	};

	public static int count;

	[Browsable(false)]
	public int TemporaryId { get; set; }

	[Browsable(false)]
	public Autodesk.SubassemblyComposer.WorkflowEngine.Point EndPoint
	{
		get
		{
			if (IsDesignTimeMode())
			{
				ActivityLibraryAppCallbacks.ComputeLoopLinksCallback(this);
			}
			if (loopPoints != null && loopPoints.Count >= 1)
			{
				return loopPoints.Last();
			}
			return loopStart;
		}
	}

	[Browsable(false)]
	public InArgument<int> Side
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateLoop p) => p.Side)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateLoop p) => p.Side), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Loop", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_LoopNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[LoopAutoNumbering]
	public InArgument<string> LoopNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateLoop p) => p.LoopNumber)) as InArgument<string>;
		}
		set
		{
			_loopNumber = LoopNumber;
			PropertyCollection_Set(this.PropertyName((CreateLoop p) => p.LoopNumber), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Loop", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_Loop_Repetition", 3)]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	[LoopRepetition]
	public InArgument<int> LoopDRepetition
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateLoop p) => p.LoopDRepetition)) as InArgument<int>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateLoop p) => p.LoopDRepetition), value);
			if (LoopNumber != null)
			{
				InternalComputeGeometry();
			}
		}
	}

	[CustomCategory(typeof(Resource), "Category_Source_Geometry", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_Links_to_Loop", 1)]
	[Editor(typeof(InlineLoopLinksUserCtrl), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> Links
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateLoop p) => p.Links)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateLoop p) => p.Links), value);
			if (LoopNumber != null)
			{
				InternalComputeGeometry();
			}
		}
	}

	[CustomCategory(typeof(Resource), "Category_Loop_Condition", 3)]
	[CustomDisplayName(typeof(Resource), "DisplayName_SurfaceTarget", 1)]
	[CanReferParameterName]
	[Editor(typeof(TargetComboBox<SurfaceTarget>), typeof(PropertyValueEditor))]
	public InArgument<SurfaceTarget> SurfaceTarget
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateLoop p) => p.SurfaceTarget)) as InArgument<SurfaceTarget>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateLoop p) => p.SurfaceTarget), value);
			if (SurfaceTarget != null)
			{
				InternalComputeGeometry();
			}
		}
	}

	public CreateLoop()
	{
		PropertyCollection_Set(this.PropertyName((CreateLoop p) => p.LoopNumber), _loopNumber, LoopNumberOnChanged);
		PropertyCollection_Set(this.PropertyName((CreateLoop p) => p.LoopDRepetition), _loopRepetition);
		count++;
		TemporaryId = count;
	}

	protected void LoopNumberOnChanged(object value)
	{
		if (value != null && (InArgument<string>)value != _loopNumber)
		{
			InArgument<string> loopNumber = _loopNumber;
			_loopNumber = (InArgument<string>)value;
			OnGeometryNameChanged(this.PropertyName((CreateLoop p) => p.LoopNumber), loopNumber, _loopNumber);
		}
	}

	private bool IsDesignTimeMode()
	{
		return ActivityDesigntime.Instance != null;
	}

	protected override void LayoutExecute(CodeActivityContext context)
	{
		InternalExecute(context, Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.LayoutMode);
	}

	protected override void RoadwayExecute(CodeActivityContext context)
	{
		InternalExecute(context, Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.RoadwayMode);
	}

	protected void ComputeLoopInExecutetime(CodeActivityContext context)
	{
		Geometry value = context.GetValue(Geometry);
		int value2 = context.GetValue(ActivityId);
		string value3 = context.GetValue(LoopNumber);
		int num = context.GetValue(LoopDRepetition);
		if (num < 0)
		{
			num = 0;
			RecordErrorInfo(context, ErrorLevel.Warning, string.Format(Resource.ErrorHandling_LoopRepetitionTooSmall, 0));
		}
		SurfaceTarget value4 = context.GetValue(SurfaceTarget);
		string[] array = context.GetValue(Links).Split(',');
		linksToLoop.Clear();
		string[] array2 = array;
		foreach (string geometryId in array2)
		{
			if (value != null)
			{
				Link link = value.GetLink(geometryId);
				if (link != null)
				{
					linksToLoop.Add(link);
				}
			}
		}
		InternalComputeLoop(linksToLoop, num, value4, ref loopStart, loopPoints, loopLinks, context);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point point = null;
		int num2 = 0;
		foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point loopPoint in loopPoints)
		{
			loopPoint.ActivityId = value2;
			if (num2 == loopPoints.Count - 1)
			{
				point = loopPoint;
			}
			value.Points.Add(loopPoint);
			num2++;
		}
		if (point != null)
		{
			value.LoopLastPoint[value3] = point;
		}
		else
		{
			value.LoopLastPoint[value3] = loopStart;
		}
		foreach (Link loopLink in loopLinks)
		{
			loopLink.ActivityId = value2;
			value.Links.Add(loopLink);
		}
	}

	private void InternalExecute(CodeActivityContext context, SubassemblyRunMode runMode)
	{
		ComputeLoopInExecutetime(context);
	}

	private bool CalDistance(double slope, double height, bool isRightSide, out double distance)
	{
		distance = 0.0;
		if (slope > 0.0)
		{
			if (height <= 0.0)
			{
				return false;
			}
			distance = Math.Abs(height / slope);
		}
		else if (slope < 0.0)
		{
			if (height >= 0.0)
			{
				return false;
			}
			distance = Math.Abs(height / slope);
		}
		else
		{
			if (height < 0.0 || height > 0.0)
			{
				return false;
			}
			distance = 0.0;
		}
		if (!isRightSide)
		{
			distance = 0.0 - distance;
		}
		return true;
	}

	private bool FindNextLink(int previous, int current, ref int next, int[,] adjacentMatrix, int count)
	{
		for (int i = 0; i < count; i++)
		{
			if (adjacentMatrix[current, i] == 1 && i != previous)
			{
				next = i;
				return true;
			}
		}
		next = -1;
		return false;
	}

	private bool IsAdjacent(Link A, Link B)
	{
		if (A.StartPoint == B.StartPoint || A.StartPoint == B.EndPoint || A.EndPoint == B.StartPoint || A.EndPoint == B.EndPoint)
		{
			return true;
		}
		return false;
	}

	private bool SortLinks(List<Link> links, List<Link> sortedLinks, ref bool isPolyLine, ref bool isPolyGon)
	{
		int[,] array = new int[links.Count, links.Count];
		for (int i = 0; i < links.Count; i++)
		{
			for (int j = 0; j < links.Count; j++)
			{
				if (i != j)
				{
					if (IsAdjacent(links[i], links[j]))
					{
						array[i, j] = 1;
					}
					else
					{
						array[i, j] = 0;
					}
				}
				else
				{
					array[i, j] = 0;
				}
			}
		}
		for (int k = 0; k < links.Count; k++)
		{
			int num = 0;
			for (int l = 0; l < links.Count; l++)
			{
				num += array[k, l];
			}
			if (num > 2)
			{
				isPolyLine = false;
				isPolyGon = false;
			}
		}
		for (int m = 0; m < links.Count; m++)
		{
			int num2 = 0;
			for (int n = 0; n < links.Count; n++)
			{
				num2 += array[m, n];
			}
			switch (num2)
			{
			case 0:
				isPolyLine = false;
				isPolyGon = false;
				break;
			case 1:
			{
				sortedLinks.Add(links[m]);
				int previous2 = -1;
				int num5 = m;
				int next2 = -1;
				for (int num6 = 0; num6 < links.Count; num6++)
				{
					if (FindNextLink(previous2, num5, ref next2, array, links.Count))
					{
						previous2 = num5;
						num5 = next2;
						sortedLinks.Add(links[next2]);
					}
					else if (num6 < links.Count - 1)
					{
						isPolyLine = false;
						break;
					}
					if (num6 == links.Count - 1)
					{
						if (next2 != -1)
						{
							isPolyLine = false;
						}
						else
						{
							isPolyLine = true;
						}
					}
				}
				break;
			}
			default:
			{
				if (m != links.Count - 1 || num2 != 2)
				{
					continue;
				}
				int previous = -1;
				int num3 = m;
				int next = -1;
				for (int num4 = 0; num4 < links.Count; num4++)
				{
					if (FindNextLink(previous, num3, ref next, array, links.Count))
					{
						previous = num3;
						num3 = next;
						sortedLinks.Add(links[next]);
						if (num4 == links.Count - 1)
						{
							if (next != m)
							{
								isPolyGon = false;
							}
							else
							{
								isPolyGon = true;
							}
						}
						continue;
					}
					isPolyGon = false;
					break;
				}
				continue;
			}
			}
			break;
		}
		if (isPolyLine | isPolyGon)
		{
			return true;
		}
		return false;
	}

	private bool IsValueBetween(double x, double x1, double x2)
	{
		if (!(x1 <= x) || !(x <= x2))
		{
			if (x2 <= x)
			{
				return x <= x1;
			}
			return false;
		}
		return true;
	}

	private bool XPointOnLink(Link link, Autodesk.SubassemblyComposer.WorkflowEngine.Point point)
	{
		if (link.StartPoint.Y == link.EndPoint.Y)
		{
			return IsValueBetween(point.X, link.StartPoint.X, link.EndPoint.X);
		}
		return IsValueBetween(point.Y, link.StartPoint.Y, link.EndPoint.Y);
	}

	private bool MeetLoopCondition(SurfaceTarget surfaceTarget, List<Link> oneLoopLinks, bool bTowardRight, CodeActivityContext context = null)
	{
		if (surfaceTarget == null)
		{
			return true;
		}
		if (oneLoopLinks.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < oneLoopLinks.Count; i++)
		{
			Link link = oneLoopLinks[i];
			double slope = (link.EndPoint.Y - link.StartPoint.Y) / Math.Abs(link.EndPoint.X - link.StartPoint.X);
			if (link.EndPoint.X == link.StartPoint.X)
			{
				slope = (link.EndPoint.Y - link.StartPoint.Y) / 1E-06;
			}
			Autodesk.SubassemblyComposer.WorkflowEngine.Point point = null;
			try
			{
				point = surfaceTarget.getDaylightPoint(link.StartPoint, slope, bTowardRight);
			}
			catch
			{
			}
			if (point != null && XPointOnLink(link, point))
			{
				return false;
			}
		}
		return true;
	}

	public void InternalComputeLoop(List<Link> links, int loopRepetition, SurfaceTarget surfaceTarget, ref Autodesk.SubassemblyComposer.WorkflowEngine.Point loopStart, List<Autodesk.SubassemblyComposer.WorkflowEngine.Point> loopPoints, List<Link> loopLinks, CodeActivityContext context = null)
	{
		InCreate = true;
		loopPoints.Clear();
		loopLinks.Clear();
		int num = ((WorkflowHost.RunningWorkflowHost.Side != SASide.Left) ? 1 : (-1));
		List<Link> list = new List<Link>(links);
		string text = context?.GetValue(LoopNumber);
		int num2 = 1;
		int num3 = 1;
		if (list.Count == 0)
		{
			return;
		}
		if (list.Count == 1)
		{
			Autodesk.SubassemblyComposer.WorkflowEngine.Point startPoint = list[0].StartPoint;
			Autodesk.SubassemblyComposer.WorkflowEngine.Point endPoint = list[0].EndPoint;
			double num4 = endPoint.X - startPoint.X;
			bool bTowardRight = num4 > 0.0;
			if (WorkflowHost.RunningWorkflowHost.Side != SASide.None)
			{
				num4 = (double)num * Math.Abs(num4);
				bTowardRight = WorkflowHost.RunningWorkflowHost.Side == SASide.Right;
			}
			double num5 = endPoint.Y - startPoint.Y;
			loopStart = endPoint;
			Autodesk.SubassemblyComposer.WorkflowEngine.Point point = null;
			if (surfaceTarget != null && surfaceTarget.IsValid)
			{
				point = surfaceTarget.getPointOnOffset(endPoint.Offset);
			}
			if (point != null && ((num5 < 0.0 && point.Elevation > endPoint.Elevation) || (num5 > 0.0 && point.Elevation < endPoint.Elevation)))
			{
				return;
			}
			new List<string>().Add(list[0].GeometryId);
			Autodesk.SubassemblyComposer.WorkflowEngine.Point point2 = endPoint;
			for (int i = 1; i <= Math.Abs(loopRepetition); i++)
			{
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point3 = null;
				point3 = ((i != 1) ? new Autodesk.SubassemblyComposer.WorkflowEngine.Point(startPoint.X + num4 * (double)i, startPoint.Y + num5 * (double)i) : point2);
				point3.Codes = startPoint.Codes;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point4 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(endPoint.X + num4 * (double)i, endPoint.Y + num5 * (double)i);
				point4.Codes = endPoint.Codes;
				Link link = new Link(point3, point4);
				List<Link> list2 = new List<Link>();
				list2.Add(link);
				link.Codes = list[0].Codes;
				if (!MeetLoopCondition(surfaceTarget, list2, bTowardRight, context))
				{
					break;
				}
				if (context != null)
				{
					int value = context.GetValue(ActivityId);
					point4.ActivityId = value;
					point4.DisplayName = (point4.GeometryId = text + "P" + num2++);
					link.DisplayName = (link.GeometryId = text + "L" + num3++);
				}
				loopPoints.Add(point4);
				loopLinks.Add(link);
				point2 = point4;
			}
		}
		else
		{
			List<Link> list3 = new List<Link>(list.Count);
			bool isPolyLine = false;
			bool isPolyGon = false;
			SortLinks(list, list3, ref isPolyLine, ref isPolyGon);
			if (isPolyGon)
			{
				return;
			}
			if (isPolyLine)
			{
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point5 = null;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point6 = null;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point startPoint2 = list3[0].StartPoint;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point endPoint2 = list3[0].EndPoint;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point startPoint3 = list3[1].StartPoint;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point endPoint3 = list3[1].EndPoint;
				point5 = ((startPoint2 != startPoint3 && startPoint2 != endPoint3) ? startPoint2 : endPoint2);
				startPoint2 = list3[list3.Count - 1].StartPoint;
				endPoint2 = list3[list3.Count - 1].EndPoint;
				startPoint3 = list3[list3.Count - 2].StartPoint;
				endPoint3 = list3[list3.Count - 2].EndPoint;
				point6 = ((startPoint2 != startPoint3 && startPoint2 != endPoint3) ? startPoint2 : endPoint2);
				bool flag = false;
				if (WorkflowHost.RunningWorkflowHost.Side == SASide.None)
				{
					List<Link> links2 = WorkflowHost.RunningWorkflowHost.SubassemblyGeometry.Links;
					flag = links2.IndexOf(list3.First()) > links2.IndexOf(list3.Last());
				}
				else
				{
					flag = (point5.X > point6.X && num == 1) || (point5.X < point6.X && num == -1);
				}
				if (flag)
				{
					list3.Reverse();
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point7 = point6;
					point6 = point5;
					point5 = point7;
				}
				double num6 = point6.X - point5.X;
				bool bTowardRight2 = num6 > 0.0;
				if (WorkflowHost.RunningWorkflowHost.Side != SASide.None)
				{
					num6 = (double)num * Math.Abs(num6);
					bTowardRight2 = WorkflowHost.RunningWorkflowHost.Side == SASide.Right;
				}
				double num7 = point6.Y - point5.Y;
				loopStart = point6;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point8 = null;
				if (surfaceTarget != null && surfaceTarget.IsValid)
				{
					point8 = surfaceTarget.getPointOnOffset(point6.Offset);
				}
				if (point8 != null && ((num7 < 0.0 && point8.Elevation > point6.Elevation) || (num7 > 0.0 && point8.Elevation < point6.Elevation)))
				{
					return;
				}
				List<string> list4 = new List<string>();
				foreach (Link item in list3)
				{
					list4.Add(item.GeometryId);
				}
				for (int j = 1; j <= Math.Abs(loopRepetition); j++)
				{
					List<Autodesk.SubassemblyComposer.WorkflowEngine.Point> list5 = new List<Autodesk.SubassemblyComposer.WorkflowEngine.Point>();
					List<Link> list6 = new List<Link>();
					for (int k = 0; k < list3.Count - 1; k++)
					{
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point9 = list3[k].StartPoint;
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point10 = list3[k].EndPoint;
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point11 = list3[k + 1].StartPoint;
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point12 = list3[k + 1].EndPoint;
						if (point9 == point11)
						{
							Autodesk.SubassemblyComposer.WorkflowEngine.Point point13 = point10;
							point10 = point9;
							point9 = point13;
						}
						if (point9 == point12)
						{
							Autodesk.SubassemblyComposer.WorkflowEngine.Point point14 = point10;
							point10 = point9;
							point9 = point14;
							Autodesk.SubassemblyComposer.WorkflowEngine.Point point15 = point12;
							point12 = point11;
							point11 = point15;
						}
						if (point10 == point12)
						{
							Autodesk.SubassemblyComposer.WorkflowEngine.Point point16 = point12;
							point12 = point11;
							point11 = point16;
						}
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point17 = point6;
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point18 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point10.X + num6 * (double)j, point10.Y + num7 * (double)j);
						point17.Codes = point9.Codes;
						point18.Codes = point10.Codes;
						Link link2 = new Link(point17, point18);
						if (context != null)
						{
							int value2 = context.GetValue(ActivityId);
							point18.ActivityId = value2;
							point18.DisplayName = (point18.GeometryId = text + "P" + num2++);
						}
						link2.Codes = list3[k].Codes;
						list5.Add(point18);
						list6.Add(link2);
						point6 = point18;
						if (k == list3.Count - 2)
						{
							Autodesk.SubassemblyComposer.WorkflowEngine.Point point19 = point6;
							Autodesk.SubassemblyComposer.WorkflowEngine.Point point20 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point12.X + num6 * (double)j, point12.Y + num7 * (double)j);
							point19.Codes = point11.Codes;
							point20.Codes = point12.Codes;
							Link link3 = new Link(point19, point20);
							if (context != null)
							{
								int value3 = context.GetValue(ActivityId);
								point20.ActivityId = value3;
							}
							if (context != null)
							{
								point20.DisplayName = (point20.GeometryId = text + "P" + num2++);
							}
							list5.Add(point20);
							list6.Add(link3);
							link3.Codes = list3[k + 1].Codes;
							point6 = point20;
						}
					}
					if (!MeetLoopCondition(surfaceTarget, list6, bTowardRight2, context))
					{
						break;
					}
					loopPoints.AddRange(list5);
					if (context != null)
					{
						foreach (Link item2 in list6)
						{
							item2.DisplayName = (item2.GeometryId = text + "L" + num3++);
						}
					}
					loopLinks.AddRange(list6);
				}
			}
		}
		InCreate = false;
	}

	public void ComputeLoopInDesignTime(List<Link> links, int loopRepetition, ref Autodesk.SubassemblyComposer.WorkflowEngine.Point loopStart, List<Autodesk.SubassemblyComposer.WorkflowEngine.Point> loopPoints, List<Link> loopLinks, string sSurfaceTargetVal)
	{
		SurfaceTarget surfaceTarget = null;
		if (sSurfaceTargetVal != null)
		{
			try
			{
				if (sSurfaceTargetVal.Contains("new PreviewSurfaceTarget("))
				{
					string[] array = sSurfaceTargetVal.Replace("new PreviewSurfaceTarget(", "").TrimEnd(')').Split(',');
					if (array.Length >= 6)
					{
						string targetName = array[0].Trim().Trim('"');
						bool bIsEnabled = bool.Parse(array[1].Trim());
						double[] surfacePoints = (from p in array.Skip(2)
							select double.Parse(p.Trim())).ToArray();
						surfaceTarget = new PreviewSurfaceTarget(targetName, bIsEnabled, surfacePoints);
					}
				}
			}
			catch
			{
			}
			finally
			{
				if (surfaceTarget == null)
				{
					surfaceTarget = RuntimeExpressionEvaluator<Autodesk.SubassemblyComposer.WorkflowEngine.SurfaceTarget>.Evaluate(sSurfaceTargetVal);
				}
			}
		}
		InternalComputeLoop(links, loopRepetition, surfaceTarget, ref loopStart, loopPoints, loopLinks);
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
	}

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = new List<ValidationError>();
		if (linksToLoop.Count > 1)
		{
			List<Link> sortedLinks = new List<Link>();
			bool isPolyLine = false;
			bool isPolyGon = false;
			SortLinks(linksToLoop, sortedLinks, ref isPolyLine, ref isPolyGon);
			if (!isPolyLine)
			{
				string message = string.Format(Resource.ErrorHandling_InLoop_LinksToLoop_NotValid);
				list.Add(new ValidationError(message, isWarning: false, ""));
			}
		}
		return list;
	}

	private void InternalComputeGeometry()
	{
		if (LoopDRepetition != null && Links != null && IsDesignTimeMode())
		{
			ActivityLibraryAppCallbacks.ComputeLoopLinksCallback(this);
		}
	}
}
