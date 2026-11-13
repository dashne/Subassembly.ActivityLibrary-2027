using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Statements;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.GeomMath;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer("System.Activities.Core.Presentation.SequenceDesigner, System.Activities.Core.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
[CreateOffsetAutoNumbering]
[ToolboxBitmap(typeof(CreateOffset), "Resources.Icons.offset_create_toolbox.ico")]
public class CreateOffset : SubassemblyActivity
{
	public const string c_Links = "Links";

	public const char linksSeparator = ',';

	private InArgument<string> _offsetNumber;

	public bool InCreate;

	public Sequence innerSequence = new Sequence();

	public List<Link> linksToOffset = new List<Link>();

	public List<Link> offsetLinks = new List<Link>();

	public List<Autodesk.SubassemblyComposer.WorkflowEngine.Point> offsetPoints = new List<Autodesk.SubassemblyComposer.WorkflowEngine.Point>();

	[CustomCategory(typeof(Resource), "Category_Offset", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_OffsetNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[OffsetAutoNumbering]
	public InArgument<string> OffsetNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateOffset p) => p.OffsetNumber)) as InArgument<string>;
		}
		set
		{
			_offsetNumber = OffsetNumber;
			PropertyCollection_Set(this.PropertyName((CreateOffset p) => p.OffsetNumber), value);
		}
	}

	[Browsable(false)]
	public Collection<Activity> Activities => innerSequence.Activities;

	[CustomCategory(typeof(Resource), "Category_Offset", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_OffsetCodes", 2)]
	[Editor(typeof(LinkCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferLinkCodeName]
	public List<InArgument<string>> OffsetCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateOffset p) => p.OffsetCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateOffset p) => p.OffsetCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Offset", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_OffsetDistance", 3)]
	[Editor(typeof(DoubleTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferParameterName]
	[CanReferVariableName]
	public InArgument<double> OffsetDistance
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateOffset p) => p.OffsetDistance)) as InArgument<double>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateOffset p) => p.OffsetDistance), value);
			if (OffsetNumber != null)
			{
				CreateOffsetActivities();
			}
		}
	}

	[CustomCategory(typeof(Resource), "Category_Source_Geometry", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_Links_to_Offset", 1)]
	[Editor(typeof(InlineOffsetLinksUserCtrl), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> Links
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateOffset p) => p.Links)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateOffset p) => p.Links), value);
			if (OffsetNumber != null)
			{
				CreateOffsetActivities();
			}
		}
	}

	[Browsable(false)]
	public List<string> ComponentNames
	{
		get
		{
			if (Links != null && Links.Expression != null)
			{
				return new List<string>((Links.Expression as Literal<string>).Value.Split(','));
			}
			return null;
		}
	}

	[Browsable(false)]
	public List<Link> _offsetLinks { get; set; }

	[Browsable(false)]
	public List<Autodesk.SubassemblyComposer.WorkflowEngine.Point> _offsetPoints { get; set; }

	public void ComputeOffset(List<Link> links, double OffsetDistance, List<Link> offsetLinks, List<Autodesk.SubassemblyComposer.WorkflowEngine.Point> offsetPoints)
	{
		InCreate = true;
		offsetLinks.Clear();
		offsetPoints.Clear();
		if (links.Count == 0)
		{
			return;
		}
		if (links.Count == 1)
		{
			Autodesk.SubassemblyComposer.WorkflowEngine.Point point = links[0].StartPoint;
			Autodesk.SubassemblyComposer.WorkflowEngine.Point point2 = links[0].EndPoint;
			if (point.Y > point2.Y)
			{
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point3 = point;
				point = point2;
				point2 = point3;
			}
			if (point.Y == point2.Y && point.X < point2.X)
			{
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point4 = point;
				point = point2;
				point2 = point4;
			}
			double x = point.X;
			double y = point.Y;
			double x2 = point2.X;
			double y2 = point2.Y;
			Autodesk.SubassemblyComposer.WorkflowEngine.Point point5 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(x + OffsetDistance * (y2 - y) / Math.Sqrt(Math.Pow(y2 - y, 2.0) + Math.Pow(x2 - x, 2.0)), y + OffsetDistance * (x - x2) / Math.Sqrt(Math.Pow(y2 - y, 2.0) + Math.Pow(x2 - x, 2.0)));
			Autodesk.SubassemblyComposer.WorkflowEngine.Point point6 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(x2 + OffsetDistance * (y2 - y) / Math.Sqrt(Math.Pow(y2 - y, 2.0) + Math.Pow(x2 - x, 2.0)), y2 + OffsetDistance * (x - x2) / Math.Sqrt(Math.Pow(y2 - y, 2.0) + Math.Pow(x2 - x, 2.0)));
			Link link = new Link(point5, point6);
			point5.ActivityId = AutoNumberEngine.NextActivityId;
			point5.DisplayName = (point5.GeometryId = "P" + AutoNumberEngine.NextGeometryNumber(typeof(PointAutoNumberingAttribute)));
			point6.ActivityId = AutoNumberEngine.NextActivityId;
			point6.DisplayName = (point6.GeometryId = "P" + AutoNumberEngine.NextGeometryNumber(typeof(PointAutoNumberingAttribute)));
			offsetPoints.Add(point5);
			offsetPoints.Add(point6);
			link.ActivityId = AutoNumberEngine.NextActivityId;
			link.DisplayName = (link.GeometryId = "L" + AutoNumberEngine.NextGeometryNumber(typeof(LinkAutoNumberingAttribute)));
			offsetLinks.Add(link);
		}
		else
		{
			List<Link> list = new List<Link>(links.Count);
			bool isPloyLine = false;
			bool isPloyGon = false;
			SortLinks(links, list, ref isPloyLine, ref isPloyGon);
			if (isPloyGon)
			{
				Autodesk.SubassemblyComposer.WorkflowEngine.Point endPoint = null;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point7 = null;
				double num = 0.0;
				for (int i = 0; i < list.Count; i++)
				{
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point8 = list[i].StartPoint;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point9 = list[i].EndPoint;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point10 = null;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point11 = null;
					if (i < list.Count - 1)
					{
						point10 = list[i + 1].StartPoint;
						point11 = list[i + 1].EndPoint;
					}
					else
					{
						point10 = list[0].StartPoint;
						point11 = list[0].EndPoint;
					}
					if (point8 == point10)
					{
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point12 = point8;
						point8 = point9;
						point9 = point12;
					}
					if (point8 == point11)
					{
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point13 = point8;
						point8 = point9;
						point9 = point13;
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point14 = point10;
						point10 = point11;
						point11 = point14;
					}
					if (point9 == point11)
					{
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point15 = point10;
						point10 = point11;
						point11 = point15;
					}
					double num2 = point9.X - point8.X;
					double num3 = point9.Y - point8.Y;
					double num4 = point11.X - point9.X;
					double num5 = point11.Y - point9.Y;
					double num6 = 0.0;
					double num7 = (num2 * num5 - num4 * num3) / (Math.Sqrt(Math.Pow(num2, 2.0) + Math.Pow(num3, 2.0)) * Math.Sqrt(Math.Pow(num4, 2.0) + Math.Pow(num5, 2.0)));
					if ((num2 * num3 + num4 * num5) / (Math.Sqrt(Math.Pow(num2, 2.0) + Math.Pow(num3, 2.0)) * Math.Sqrt(Math.Pow(num4, 2.0) + Math.Pow(num5, 2.0))) >= 0.0)
					{
						num6 = Math.Asin(num7);
					}
					else
					{
						num6 = ((!(num7 >= 0.0)) ? (-Math.PI - Math.Asin(num7)) : (Math.PI - Math.Asin(num7)));
					}
					num += num6;
				}
				if (num < 0.0)
				{
					list.Reverse();
				}
				for (int j = 0; j < list.Count; j++)
				{
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point16 = list[j].StartPoint;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point17 = list[j].EndPoint;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point18 = null;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point19 = null;
					if (j < list.Count - 1)
					{
						point18 = list[j + 1].StartPoint;
						point19 = list[j + 1].EndPoint;
					}
					else
					{
						point18 = list[0].StartPoint;
						point19 = list[0].EndPoint;
					}
					if (point16 == point18)
					{
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point20 = point16;
						point16 = point17;
						point17 = point20;
					}
					if (point16 == point19)
					{
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point21 = point16;
						point16 = point17;
						point17 = point21;
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point22 = point18;
						point18 = point19;
						point19 = point22;
					}
					if (point17 == point19)
					{
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point23 = point18;
						point18 = point19;
						point19 = point23;
					}
					double x3 = point16.X;
					double y3 = point16.Y;
					double x4 = point17.X;
					double y4 = point17.Y;
					double x5 = point19.X;
					double y5 = point19.Y;
					if (j == 0)
					{
						point7 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(x3 + OffsetDistance * (y4 - y3) / Math.Sqrt(Math.Pow(y4 - y3, 2.0) + Math.Pow(x4 - x3, 2.0)), y3 + OffsetDistance * (x3 - x4) / Math.Sqrt(Math.Pow(y4 - y3, 2.0) + Math.Pow(x4 - x3, 2.0)));
					}
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point24 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(x4 + OffsetDistance * (y4 - y3) / Math.Sqrt(Math.Pow(y4 - y3, 2.0) + Math.Pow(x4 - x3, 2.0)), y4 + OffsetDistance * (x3 - x4) / Math.Sqrt(Math.Pow(y4 - y3, 2.0) + Math.Pow(x4 - x3, 2.0)));
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point25 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(x4 + OffsetDistance * (y5 - y4) / Math.Sqrt(Math.Pow(y5 - y4, 2.0) + Math.Pow(x5 - x4, 2.0)), y4 + OffsetDistance * (x4 - x5) / Math.Sqrt(Math.Pow(y5 - y4, 2.0) + Math.Pow(x5 - x4, 2.0)));
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point26 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(x5 + OffsetDistance * (y5 - y4) / Math.Sqrt(Math.Pow(y5 - y4, 2.0) + Math.Pow(x5 - x4, 2.0)), y5 + OffsetDistance * (x4 - x5) / Math.Sqrt(Math.Pow(y5 - y4, 2.0) + Math.Pow(x5 - x4, 2.0)));
					Autodesk.SubassemblyComposer.WorkflowEngine.Point intersectPoint = GeometryUtility.GetIntersectPoint(point7, point24, point25, point26);
					if (OffsetDistance < 0.0)
					{
						if (GeometryUtility.FuzzyEqual(intersectPoint.X, point7.X) && GeometryUtility.FuzzyEqual(point24.X, point7.X))
						{
							if ((intersectPoint.Y - point7.Y) * (point24.Y - point7.Y) <= 0.0)
							{
								offsetLinks.Clear();
								offsetPoints.Clear();
								MessageBox.Show(string.Format(Resource.Message_CreateOffset_OffsetDiatance_NotValid, OffsetDistance), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
								break;
							}
						}
						else if ((intersectPoint.X - point7.X) * (point24.X - point7.X) <= 0.0)
						{
							offsetLinks.Clear();
							offsetPoints.Clear();
							MessageBox.Show(string.Format(Resource.Message_CreateOffset_OffsetDiatance_NotValid, OffsetDistance), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
							break;
						}
						if (GeometryUtility.FuzzyEqual(intersectPoint.X, point26.X) && GeometryUtility.FuzzyEqual(point25.X, point26.X))
						{
							if ((intersectPoint.Y - point26.Y) * (point25.Y - point26.Y) <= 0.0)
							{
								offsetLinks.Clear();
								offsetPoints.Clear();
								MessageBox.Show(string.Format(Resource.Message_CreateOffset_OffsetDiatance_NotValid, OffsetDistance), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
								break;
							}
						}
						else if ((intersectPoint.X - point26.X) * (point25.X - point26.X) <= 0.0)
						{
							offsetLinks.Clear();
							offsetPoints.Clear();
							MessageBox.Show(string.Format(Resource.Message_CreateOffset_OffsetDiatance_NotValid, OffsetDistance), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
							break;
						}
					}
					if (j == 0)
					{
						endPoint = intersectPoint;
						intersectPoint.ActivityId = AutoNumberEngine.NextActivityId;
						intersectPoint.DisplayName = (intersectPoint.GeometryId = "P" + AutoNumberEngine.NextGeometryNumber(typeof(PointAutoNumberingAttribute)));
						offsetPoints.Add(intersectPoint);
					}
					else
					{
						intersectPoint.ActivityId = AutoNumberEngine.NextActivityId;
						intersectPoint.DisplayName = (intersectPoint.GeometryId = "P" + AutoNumberEngine.NextGeometryNumber(typeof(PointAutoNumberingAttribute)));
						offsetPoints.Add(intersectPoint);
						Link link2 = new Link(point7, intersectPoint);
						link2.ActivityId = AutoNumberEngine.NextActivityId;
						link2.DisplayName = (link2.GeometryId = "L" + AutoNumberEngine.NextGeometryNumber(typeof(LinkAutoNumberingAttribute)));
						offsetLinks.Add(link2);
					}
					if (j == links.Count - 1)
					{
						Link link3 = new Link(intersectPoint, endPoint);
						link3.ActivityId = AutoNumberEngine.NextActivityId;
						link3.DisplayName = (link3.GeometryId = "L" + AutoNumberEngine.NextGeometryNumber(typeof(LinkAutoNumberingAttribute)));
						offsetLinks.Add(link3);
					}
					point7 = intersectPoint;
				}
			}
			else if (isPloyLine)
			{
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point27 = null;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point28 = null;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point29 = null;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point startPoint = list[0].StartPoint;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point endPoint2 = list[0].EndPoint;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point startPoint2 = list[1].StartPoint;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point endPoint3 = list[1].EndPoint;
				point28 = ((startPoint != startPoint2 && startPoint != endPoint3) ? startPoint : endPoint2);
				startPoint = list[list.Count - 1].StartPoint;
				endPoint2 = list[list.Count - 1].EndPoint;
				startPoint2 = list[list.Count - 2].StartPoint;
				endPoint3 = list[list.Count - 2].EndPoint;
				point29 = ((startPoint != startPoint2 && startPoint != endPoint3) ? startPoint : endPoint2);
				if (point28.X > point29.X)
				{
					list.Reverse();
				}
				for (int k = 0; k < list.Count - 1; k++)
				{
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point30 = list[k].StartPoint;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point31 = list[k].EndPoint;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point32 = list[k + 1].StartPoint;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point33 = list[k + 1].EndPoint;
					if (point30 == point32)
					{
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point34 = point30;
						point30 = point31;
						point31 = point34;
					}
					if (point30 == point33)
					{
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point35 = point30;
						point30 = point31;
						point31 = point35;
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point36 = point32;
						point32 = point33;
						point33 = point36;
					}
					if (point31 == point33)
					{
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point37 = point32;
						point32 = point33;
						point33 = point37;
					}
					double x6 = point30.X;
					double y6 = point30.Y;
					double x7 = point31.X;
					double y7 = point31.Y;
					double x8 = point33.X;
					double y8 = point33.Y;
					if (k == 0)
					{
						point27 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(x6 + OffsetDistance * (y7 - y6) / Math.Sqrt(Math.Pow(y7 - y6, 2.0) + Math.Pow(x7 - x6, 2.0)), y6 + OffsetDistance * (x6 - x7) / Math.Sqrt(Math.Pow(y7 - y6, 2.0) + Math.Pow(x7 - x6, 2.0)));
					}
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point38 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(x7 + OffsetDistance * (y7 - y6) / Math.Sqrt(Math.Pow(y7 - y6, 2.0) + Math.Pow(x7 - x6, 2.0)), y7 + OffsetDistance * (x6 - x7) / Math.Sqrt(Math.Pow(y7 - y6, 2.0) + Math.Pow(x7 - x6, 2.0)));
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point39 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(x7 + OffsetDistance * (y8 - y7) / Math.Sqrt(Math.Pow(y8 - y7, 2.0) + Math.Pow(x8 - x7, 2.0)), y7 + OffsetDistance * (x7 - x8) / Math.Sqrt(Math.Pow(y8 - y7, 2.0) + Math.Pow(x8 - x7, 2.0)));
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point40 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(x8 + OffsetDistance * (y8 - y7) / Math.Sqrt(Math.Pow(y8 - y7, 2.0) + Math.Pow(x8 - x7, 2.0)), y8 + OffsetDistance * (x7 - x8) / Math.Sqrt(Math.Pow(y8 - y7, 2.0) + Math.Pow(x8 - x7, 2.0)));
					Autodesk.SubassemblyComposer.WorkflowEngine.Point intersectPoint2 = GeometryUtility.GetIntersectPoint(point27, point38, point39, point40);
					if (GeometryUtility.FuzzyEqual(intersectPoint2.X, point27.X) && GeometryUtility.FuzzyEqual(point38.X, point27.X))
					{
						if ((intersectPoint2.Y - point27.Y) * (point38.Y - point27.Y) <= 0.0)
						{
							offsetLinks.Clear();
							offsetPoints.Clear();
							MessageBox.Show(string.Format(Resource.Message_CreateOffset_OffsetDiatance_NotValid, OffsetDistance), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
							break;
						}
					}
					else if ((intersectPoint2.X - point27.X) * (point38.X - point27.X) <= 0.0)
					{
						offsetLinks.Clear();
						offsetPoints.Clear();
						MessageBox.Show(string.Format(Resource.Message_CreateOffset_OffsetDiatance_NotValid, OffsetDistance), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
						break;
					}
					if (GeometryUtility.FuzzyEqual(intersectPoint2.X, point40.X) && GeometryUtility.FuzzyEqual(point39.X, point40.X))
					{
						if ((intersectPoint2.Y - point40.Y) * (point39.Y - point40.Y) <= 0.0)
						{
							offsetLinks.Clear();
							offsetPoints.Clear();
							MessageBox.Show(string.Format(Resource.Message_CreateOffset_OffsetDiatance_NotValid, OffsetDistance), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
							break;
						}
					}
					else if ((intersectPoint2.X - point40.X) * (point39.X - point40.X) <= 0.0)
					{
						offsetLinks.Clear();
						offsetPoints.Clear();
						MessageBox.Show(string.Format(Resource.Message_CreateOffset_OffsetDiatance_NotValid, OffsetDistance), Resource.Message_Caption_Warning, MessageBoxButton.OK, MessageBoxImage.Exclamation);
						break;
					}
					if (k == 0)
					{
						point27.ActivityId = AutoNumberEngine.NextActivityId;
						point27.DisplayName = (point27.GeometryId = "P" + AutoNumberEngine.NextGeometryNumber(typeof(PointAutoNumberingAttribute)));
						intersectPoint2.ActivityId = AutoNumberEngine.NextActivityId;
						intersectPoint2.DisplayName = (intersectPoint2.GeometryId = "P" + AutoNumberEngine.NextGeometryNumber(typeof(PointAutoNumberingAttribute)));
						offsetPoints.Add(point27);
						offsetPoints.Add(intersectPoint2);
						Link link4 = new Link(point27, intersectPoint2);
						link4.ActivityId = AutoNumberEngine.NextActivityId;
						link4.DisplayName = (link4.GeometryId = "L" + AutoNumberEngine.NextGeometryNumber(typeof(LinkAutoNumberingAttribute)));
						offsetLinks.Add(link4);
					}
					else
					{
						intersectPoint2.ActivityId = AutoNumberEngine.NextActivityId;
						intersectPoint2.DisplayName = (intersectPoint2.GeometryId = "P" + AutoNumberEngine.NextGeometryNumber(typeof(PointAutoNumberingAttribute)));
						offsetPoints.Add(intersectPoint2);
						Link link5 = new Link(point27, intersectPoint2);
						link5.ActivityId = AutoNumberEngine.NextActivityId;
						link5.DisplayName = (link5.GeometryId = "L" + AutoNumberEngine.NextGeometryNumber(typeof(LinkAutoNumberingAttribute)));
						offsetLinks.Add(link5);
					}
					if (k == links.Count - 2)
					{
						point40.ActivityId = AutoNumberEngine.NextActivityId;
						point40.DisplayName = (point40.GeometryId = "P" + AutoNumberEngine.NextGeometryNumber(typeof(PointAutoNumberingAttribute)));
						offsetPoints.Add(point40);
						Link link6 = new Link(intersectPoint2, point40);
						link6.ActivityId = AutoNumberEngine.NextActivityId;
						link6.DisplayName = (link6.GeometryId = "L" + AutoNumberEngine.NextGeometryNumber(typeof(LinkAutoNumberingAttribute)));
						offsetLinks.Add(link6);
					}
					point27 = intersectPoint2;
				}
			}
		}
		InCreate = false;
	}

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = new List<ValidationError>();
		if (linksToOffset.Count > 1)
		{
			List<Link> sortedLinks = new List<Link>();
			bool isPloyLine = false;
			bool isPloyGon = false;
			if (!SortLinks(linksToOffset, sortedLinks, ref isPloyLine, ref isPloyGon))
			{
				string message = string.Format(Resource.ErrorHandling_InOffset_LinksToOffset_NotValid);
				list.Add(new ValidationError(message, isWarning: false, ""));
			}
		}
		foreach (Activity activity in Activities)
		{
			if (activity is CreateLink && ((CreateLink)activity).LinkCodes != null)
			{
				string value = ((Literal<string>)((CreateLink)activity).LinkNumber.Expression).Value;
				int count = ((CreateLink)activity).LinkCodes.Count;
				for (int i = 0; i < count; i++)
				{
					if (((CreateLink)activity).LinkCodes[i].Expression.ToString() == ": VisualBasicValue<String>")
					{
						string expressionText = ((VisualBasicValue<string>)((CreateLink)activity).LinkCodes[i].Expression).ExpressionText;
						string message2 = string.Format(Resource.Message_Validate_CodeName_InOffset_NotArgument, expressionText, value);
						list.Add(new ValidationError(message2, isWarning: false, ""));
					}
				}
			}
			if (!(activity is CreatePoint))
			{
				continue;
			}
			if (((CreatePoint)activity).PointCodes != null)
			{
				string value2 = ((Literal<string>)((CreatePoint)activity).PointNumber.Expression).Value;
				int count2 = ((CreatePoint)activity).PointCodes.Count;
				for (int j = 0; j < count2; j++)
				{
					if (((CreatePoint)activity).PointCodes[j].Expression.ToString() == ": VisualBasicValue<String>")
					{
						string expressionText2 = ((VisualBasicValue<string>)((CreatePoint)activity).PointCodes[j].Expression).ExpressionText;
						string message3 = string.Format(Resource.Message_Validate_CodeName_InOffset_NotArgument, expressionText2, value2);
						list.Add(new ValidationError(message3, isWarning: false, ""));
					}
				}
			}
			if (!((Literal<bool>)((CreatePoint)activity).AutoLink.Expression).Value || ((CreatePoint)activity).AutoLinkCodes == null)
			{
				continue;
			}
			string value3 = ((Literal<string>)((CreatePoint)activity).AutoLinkGeometryName.Expression).Value;
			int count3 = ((CreatePoint)activity).AutoLinkCodes.Count;
			for (int k = 0; k < count3; k++)
			{
				if (((CreatePoint)activity).AutoLinkCodes[k].Expression.ToString() == ": VisualBasicValue<String>")
				{
					string expressionText3 = ((VisualBasicValue<string>)((CreatePoint)activity).AutoLinkCodes[k].Expression).ExpressionText;
					string message4 = string.Format(Resource.Message_Validate_CodeName_InOffset_NotArgument, expressionText3, value3);
					list.Add(new ValidationError(message4, isWarning: false, ""));
				}
			}
		}
		foreach (Activity activity2 in Activities)
		{
			if (!(activity2 is CreateLink))
			{
				continue;
			}
			string value4 = ((Literal<string>)((CreateLink)activity2).LinkNumber.Expression).Value;
			string value5 = ((Literal<string>)((CreateLink)activity2).StartPoint.Expression).Value;
			string value6 = ((Literal<string>)((CreateLink)activity2).EndPoint.Expression).Value;
			bool flag = false;
			bool flag2 = false;
			foreach (Activity activity3 in Activities)
			{
				if (activity3 is CreatePoint)
				{
					string value7 = ((Literal<string>)((CreatePoint)activity3).PointNumber.Expression).Value;
					if (value5 == value7)
					{
						flag = true;
					}
					if (value6 == value7)
					{
						flag2 = true;
					}
				}
			}
			ObservableCollection<string> allValidPointNames = ValidGeometryNamesHelper.GetAllValidPointNames(ActivityDesigntime.Instance.CodePathTree, activityId, includeOrigin: true);
			if (allValidPointNames.Contains(value5))
			{
				flag = true;
			}
			if (allValidPointNames.Contains(value6))
			{
				flag2 = true;
			}
			if (!flag)
			{
				string message5 = string.Format(Resource.ErrorHandling_CreateLink_InOffset_StartPointNotFound, value5, value4);
				list.Add(new ValidationError(message5, isWarning: false, value5));
			}
			if (!flag2)
			{
				string message6 = string.Format(Resource.ErrorHandling_CreateLink_InOffset_EndPointNotFound, value6, value4);
				list.Add(new ValidationError(message6, isWarning: false, value6));
			}
		}
		return list;
	}

	private ValidationError ConstructTempOffset(Offset offset, List<string> linkNames, List<CodePathTreeNode> path)
	{
		foreach (string linkName in linkNames)
		{
			if (string.IsNullOrEmpty(linkName))
			{
				continue;
			}
			Link link = new Link();
			link.GeometryId = linkName;
			foreach (CodePathTreeNode item in path)
			{
				if (item.GeometryNames.Contains(linkName))
				{
					FindStartAndEndPointNames(link, linkName, item.ActivityId);
					break;
				}
			}
			if (link.Points.Count > 0)
			{
				offset.Links.Add(link);
				continue;
			}
			return new ValidationError(string.Format(Resource.Message_Validate_Offset_WrongLink, linkName), isWarning: true, "Links");
		}
		return null;
	}

	private void FindStartAndEndPointNames(Link link, string linkName, int activityId)
	{
		ModelItem modelItem = FindActivityOnIdHelper.Execute(activityId);
		if (modelItem == null)
		{
			return;
		}
		foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(modelItem.GetCurrentValue()))
		{
			if (!(property.Attributes[typeof(GeometryAutoNumberingAttribute)] is GeometryAutoNumberingAttribute geometryAutoNumberingAttribute) || !(geometryAutoNumberingAttribute.GetType() == typeof(LinkAutoNumberingAttribute)))
			{
				continue;
			}
			bool flag = true;
			if (!string.IsNullOrEmpty(geometryAutoNumberingAttribute.CheckPropertyName))
			{
				flag = ActivityHelpers.GetBooleanPropertyValue(modelItem, geometryAutoNumberingAttribute.CheckPropertyName);
			}
			if (flag)
			{
				string stringPropertyValue = ActivityHelpers.GetStringPropertyValue(modelItem, property.Name);
				if (linkName.Equals(stringPropertyValue))
				{
					LinkAutoNumberingAttribute linkAutoNumberingAttribute = geometryAutoNumberingAttribute as LinkAutoNumberingAttribute;
					AddPoint(modelItem, linkAutoNumberingAttribute.StartPointPropertyName, link);
					AddPoint(modelItem, linkAutoNumberingAttribute.EndPointPropertyName, link);
					break;
				}
			}
		}
	}

	private void AddPoint(ModelItem modelItem, string proeprtyName, Link link)
	{
		ModelProperty modelProperty = modelItem.Properties[proeprtyName];
		if (modelProperty != null && modelProperty.Value != null)
		{
			string geometryId = (modelProperty.Value.GetCurrentValue() as InArgument<string>).Expression.ToString();
			Autodesk.SubassemblyComposer.WorkflowEngine.Point point = new Autodesk.SubassemblyComposer.WorkflowEngine.Point();
			point.GeometryId = geometryId;
			link.Points.Add(point);
		}
	}

	private void CreateOffsetActivities()
	{
		if (OffsetDistance == null || Links == null)
		{
			return;
		}
		AutoNumberEngine.Reset();
		ModelItem modelItem = FindActivityOnIdHelper.Execute((ActivityId.Expression as Literal<int>).Value);
		using ModelEditingScope modelEditingScope = modelItem.BeginEdit();
		modelItem.Properties["Activities"].Collection.Clear();
		ActivityLibraryAppCallbacks.ComputeOffsetLinksCallback(this);
		_offsetLinks = offsetLinks;
		_offsetPoints = offsetPoints;
		for (int i = 0; i < offsetPoints.Count; i++)
		{
			CreatePoint createPoint = new CreatePoint
			{
				ActivityId = offsetPoints[i].ActivityId,
				PointNumber = offsetPoints[i].GeometryId,
				DisplayName = offsetPoints[i].GeometryId,
				Positioning = PointGeometryType.DeltaXAndDeltaY,
				FromPoint = GlobleString.String_Origin,
				Side = 1,
				Arguments = new Dictionary<string, Argument>()
			};
			InArgument<double> value = offsetPoints[i].X;
			InArgument<double> value2 = offsetPoints[i].Y;
			InArgument<double> value3 = -0.02;
			InArgument<double> value4 = 0.0;
			InArgument<double> value5 = 0.5;
			InArgument<double> value6 = 5.0;
			InArgument<double> value7 = 10.0;
			InArgument<string> value8 = GlobleString.String_None;
			InArgument<bool> value9 = false;
			InArgument<bool> value10 = true;
			InArgument<OffsetTarget> value11 = new InArgument<OffsetTarget>();
			InArgument<SurfaceTarget> value12 = new InArgument<SurfaceTarget>();
			InArgument<ElevationTarget> value13 = new InArgument<ElevationTarget>();
			InArgument<CrossSegmentType> value14 = new InArgument<CrossSegmentType>();
			createPoint.Arguments.Add("AngleReferencePoint6", value8);
			createPoint.Arguments.Add("ReverseAngleReference6", value9);
			createPoint.Arguments.Add("Angle6", value4);
			createPoint.Arguments.Add("DeltaX6", value4);
			createPoint.Arguments.Add("OffsetTarget6", value11);
			createPoint.Arguments.Add("AngleReferencePoint7", value8);
			createPoint.Arguments.Add("ReverseAngleReference7", value9);
			createPoint.Arguments.Add("Angle7", value4);
			createPoint.Arguments.Add("DeltaY7", value4);
			createPoint.Arguments.Add("ElevationTarget7", value13);
			createPoint.Arguments.Add("AngleReferencePoint8", value8);
			createPoint.Arguments.Add("ReverseAngleReference8", value9);
			createPoint.Arguments.Add("Angle8", value4);
			createPoint.Arguments.Add("Distance8", value4);
			createPoint.Arguments.Add("DeltaX1", value);
			createPoint.Arguments.Add("DeltaY1", value2);
			createPoint.Arguments.Add("DeltaX4", value4);
			createPoint.Arguments.Add("SurfaceTarget4", value12);
			createPoint.Arguments.Add("OffsetTarget4", value11);
			createPoint.Arguments.Add("DeltaYForLayout4", value4);
			createPoint.Arguments.Add("DestPoint", value8);
			createPoint.Arguments.Add("InterpolateParam", value5);
			createPoint.Arguments.Add("Slope2", value3);
			createPoint.Arguments.Add("DeltaX2", value7);
			createPoint.Arguments.Add("OffsetTarget2", value11);
			createPoint.Arguments.Add("ElevationTarget2", value13);
			createPoint.Arguments.Add("Superelevation2", value14);
			createPoint.Arguments.Add("Slope5", value5);
			createPoint.Arguments.Add("DeltaY5", value7);
			createPoint.Arguments.Add("OffsetTarget5", value11);
			createPoint.Arguments.Add("ElevationTarget5", value13);
			createPoint.Arguments.Add("Superelevation5", value14);
			createPoint.Arguments.Add("ReverseSlopeDirection5", value9);
			createPoint.Arguments.Add("Slope3", value3);
			createPoint.Arguments.Add("SurfaceTarget3", value12);
			createPoint.Arguments.Add("ReverseSlopeDirection3", value9);
			createPoint.Arguments.Add("DeltaXForLayout3", value6);
			createPoint.Arguments.Add("ShowErrors", value10);
			modelItem.Properties["Activities"].Collection.Add(createPoint);
		}
		for (int j = 0; j < offsetLinks.Count; j++)
		{
			CreateLink value15 = new CreateLink
			{
				ActivityId = offsetLinks[j].ActivityId,
				LinkNumber = offsetLinks[j].GeometryId,
				StartPoint = offsetLinks[j].StartPoint.GeometryId,
				EndPoint = offsetLinks[j].EndPoint.GeometryId,
				DisplayName = offsetLinks[j].GeometryId,
				IsEnabled = true
			};
			modelItem.Properties["Activities"].Collection.Add(value15);
		}
		ModelTreeManager service = ActivityDesigntime.Instance.Context.Services.GetService<ModelTreeManager>();
		foreach (Activity activity in Activities)
		{
			ModelItem modelItem2 = service.CreateModelItem(modelItem, activity);
			ActivityDesigntime.Instance.ActivityAdded(modelItem2);
		}
		modelEditingScope.Complete();
	}

	public CreateOffset()
	{
		PropertyCollection_Set(this.PropertyName((CreateOffset p) => p.OffsetNumber), _offsetNumber, OffsetNumberOnChanged);
	}

	protected void OffsetNumberOnChanged(object value)
	{
		if (value != null && (InArgument<string>)value != _offsetNumber)
		{
			InArgument<string> offsetNumber = _offsetNumber;
			_offsetNumber = (InArgument<string>)value;
			OnGeometryNameChanged(this.PropertyName((CreateOffset p) => p.OffsetNumber), offsetNumber, _offsetNumber);
		}
	}

	protected override void LayoutExecute(CodeActivityContext context)
	{
		InternalExecute(context, Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.LayoutMode);
	}

	protected override void RoadwayExecute(CodeActivityContext context)
	{
		InternalExecute(context, Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.RoadwayMode);
	}

	private void InternalExecute(CodeActivityContext context, SubassemblyRunMode runMode)
	{
		Geometry value = context.GetValue(Geometry);
		Offset offset = new Offset();
		offset.Links = _offsetLinks;
		offset.Points = _offsetPoints;
		offset.ActivityId = context.GetValue(ActivityId);
		offset.GeometryId = context.GetValue(OffsetNumber);
		offset.Codes = GetCodes(context, OffsetCodes);
		if (ActivityDesigntime.Instance != null)
		{
			offset.DisplayName = DisplayName;
		}
		foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point21 in offset.Points)
		{
			foreach (Activity activity in Activities)
			{
				if (!(activity is CreatePoint) || point21.ActivityId != (((CreatePoint)activity).ActivityId.Expression as Literal<int>).Value)
				{
					continue;
				}
				point21.GeometryId = ((Literal<string>)((CreatePoint)activity).PointNumber.Expression).Value;
				if (((CreatePoint)activity).PointCodes != null)
				{
					int count = ((CreatePoint)activity).PointCodes.Count;
					point21.Codes = new string[count];
					for (int i = 0; i < count; i++)
					{
						point21.Codes[i] = ((Literal<string>)((CreatePoint)activity).PointCodes[i].Expression).Value;
					}
				}
				string value2 = ((Literal<string>)((CreatePoint)activity).FromPoint.Expression).Value;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point = null;
				point = ((!(value2 == GlobleString.String_Origin)) ? value.GetPoint(value2) : new Autodesk.SubassemblyComposer.WorkflowEngine.Point(0.0, 0.0));
				int num = ((WorkflowHost.RunningWorkflowHost.Side != SASide.Left) ? 1 : (-1));
				Dictionary<string, Argument> dictionary = ((CreatePoint)activity).Arguments;
				switch (((CreatePoint)activity).Positioning)
				{
				case PointGeometryType.AngleAndDeltaX:
				{
					string value20 = ((Literal<string>)dictionary["AngleReferencePoint6"].Expression).Value;
					bool value21 = ((Literal<bool>)dictionary["ReverseAngleReference6"].Expression).Value;
					double value22 = ((Literal<double>)dictionary["Angle6"].Expression).Value;
					double value23 = ((Literal<double>)dictionary["DeltaX6"].Expression).Value;
					OffsetTarget offsetTarget5 = null;
					if ((Literal<OffsetTarget>)dictionary["OffsetTarget6"].Expression != null)
					{
						offsetTarget5 = ((Literal<OffsetTarget>)dictionary["OffsetTarget6"].Expression).Value;
					}
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point14 = value.GetPoint(value20);
					bool reverse2 = value21;
					double num18 = value22;
					double offsetX = value23;
					double num19 = num18;
					num18 = GeometryFactory.AdjustAngel_Degree(num18);
					if (Math.Abs(num18 - 90.0) < 1E-05)
					{
						((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_AngleCannotZero, Resource.PointGeometryType_DisplayName_AngleAndDeltaX, num19));
					}
					else if (Math.Abs(num18 - 270.0) < 1E-05)
					{
						((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_AngleCannotZero, Resource.PointGeometryType_DisplayName_AngleAndDeltaX, num19));
					}
					if (runMode == Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.RoadwayMode)
					{
						try
						{
							OffsetTarget offsetTarget6 = offsetTarget5;
							if (offsetTarget6 != null && offsetTarget6.IsValid && point != null)
							{
								offsetX = offsetTarget6.Value - point.Offset;
							}
						}
						catch
						{
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetOffsetFromOffsetTarget);
						}
					}
					GeomPoint geoFromPt2 = new GeomPoint(point.X, point.Y);
					GeomPoint geomPoint2;
					if (point14 == null)
					{
						geomPoint2 = new GeomPoint(geoFromPt2.X + (double)(num * 10), geoFromPt2.Y);
					}
					else
					{
						geomPoint2 = new GeomPoint(point14.X, point14.Y);
						if (Math.Abs(geoFromPt2.Distance(geomPoint2)) < 1E-05)
						{
							string msg3 = string.Format(Resource.ErrorHandling_OverlappedPoints, point14.GeometryId, point.GeometryId);
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, msg3);
						}
					}
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point15 = GeometryFactory.AddPointByAngleOffset(geoFromPt2, geomPoint2, num18 * (double)num, offsetX, reverse2);
					point21.X = point15.X;
					point21.Y = point15.Y;
					break;
				}
				case PointGeometryType.AngleAndDeltaY:
				{
					string value26 = ((Literal<string>)dictionary["AngleReferencePoint7"].Expression).Value;
					bool value27 = ((Literal<bool>)dictionary["ReverseAngleReference7"].Expression).Value;
					double value28 = ((Literal<double>)dictionary["Angle7"].Expression).Value;
					double value29 = ((Literal<double>)dictionary["DeltaY7"].Expression).Value;
					ElevationTarget elevationTarget5 = null;
					if ((Literal<ElevationTarget>)dictionary["ElevationTarget7"].Expression != null)
					{
						elevationTarget5 = ((Literal<ElevationTarget>)dictionary["ElevationTarget7"].Expression).Value;
					}
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point19 = value.GetPoint(value26);
					bool reverse3 = value27;
					double num23 = value28;
					double offsetY = value29;
					double num24 = num23;
					num23 = GeometryFactory.AdjustAngel_Degree(num23);
					if (Math.Abs(num23 - 0.0) < 1E-05)
					{
						((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_AngleCannotZero, Resource.PointGeometryType_DisplayName_AngleAndDeltaY, num24));
					}
					else if (Math.Abs(num23 - 180.0) < 1E-05)
					{
						((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, string.Format(Resource.ErrorHandling_AngleCannotZero, Resource.PointGeometryType_DisplayName_AngleAndDeltaY, num24));
					}
					if (runMode == Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.RoadwayMode)
					{
						try
						{
							ElevationTarget elevationTarget6 = elevationTarget5;
							if (elevationTarget6 != null && elevationTarget6.IsValid && point != null)
							{
								offsetY = elevationTarget6.Value - point.Elevation;
							}
						}
						catch
						{
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetElevationFromElevationTarget);
						}
					}
					GeomPoint geoFromPt3 = new GeomPoint(point.X, point.Y);
					GeomPoint geomPoint3;
					if (point19 == null)
					{
						geomPoint3 = new GeomPoint(geoFromPt3.X + (double)(num * 10), geoFromPt3.Y);
					}
					else
					{
						geomPoint3 = new GeomPoint(point19.X, point19.Y);
						if (Math.Abs(geoFromPt3.Distance(geomPoint3)) < 1E-05)
						{
							string msg4 = string.Format(Resource.ErrorHandling_OverlappedPoints, point19.GeometryId, point.GeometryId);
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, msg4);
						}
					}
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point20 = GeometryFactory.AddPointByAngleElevation(geoFromPt3, geomPoint3, num23 * (double)num, offsetY, reverse3);
					point21.X = point20.X;
					point21.Y = point20.Y;
					break;
				}
				case PointGeometryType.AngleAndDistance:
				{
					string value14 = ((Literal<string>)dictionary["AngleReferencePoint8"].Expression).Value;
					bool value15 = ((Literal<bool>)dictionary["ReverseAngleReference8"].Expression).Value;
					double value16 = ((Literal<double>)dictionary["Angle8"].Expression).Value;
					double value17 = ((Literal<double>)dictionary["Distance8"].Expression).Value;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point10 = null;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point11 = value.GetPoint(value14);
					bool reverse = value15;
					double angle = value16;
					double radius = value17;
					angle = GeometryFactory.AdjustAngel_Degree(angle);
					GeomPoint geoFromPt = new GeomPoint(point.X, point.Y);
					if (point11 == null)
					{
						point10 = GeometryFactory.AddPointByAngleDistance(geoFromPt, GeomPoint.Empty, angle * (double)num, radius, reverse);
					}
					else
					{
						GeomPoint geomPoint = new GeomPoint(point11.X, point11.Y);
						if (Math.Abs(geoFromPt.Distance(geomPoint)) < 1E-05)
						{
							string msg2 = string.Format(Resource.ErrorHandling_OverlappedPoints, point11.GeometryId, point.GeometryId);
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, msg2);
						}
						point10 = GeometryFactory.AddPointByAngleDistance(geoFromPt, geomPoint, angle * (double)num, radius, reverse);
					}
					point21.X = point10.X;
					point21.Y = point10.Y;
					break;
				}
				case PointGeometryType.DeltaXAndDeltaY:
				{
					double value10 = ((Literal<double>)dictionary["DeltaX1"].Expression).Value;
					double value11 = ((Literal<double>)dictionary["DeltaY1"].Expression).Value;
					double num8 = (double)num * value10;
					double num9 = value11;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point7 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point.X + num8, point.Y + num9);
					point21.X = point7.X;
					point21.Y = point7.Y;
					break;
				}
				case PointGeometryType.DeltaXOnSurface:
				{
					double value24 = ((Literal<double>)dictionary["DeltaX4"].Expression).Value;
					SurfaceTarget surfaceTarget3 = null;
					if ((Literal<SurfaceTarget>)dictionary["SurfaceTarget4"].Expression != null)
					{
						surfaceTarget3 = ((Literal<SurfaceTarget>)dictionary["SurfaceTarget4"].Expression).Value;
					}
					OffsetTarget offsetTarget7 = null;
					if ((Literal<OffsetTarget>)dictionary["OffsetTarget4"].Expression != null)
					{
						offsetTarget7 = ((Literal<OffsetTarget>)dictionary["OffsetTarget4"].Expression).Value;
					}
					double value25 = ((Literal<double>)dictionary["DeltaYForLayout4"].Expression).Value;
					if (runMode == Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.LayoutMode)
					{
						double num20 = value24 * (double)num;
						double num21 = value25;
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point16 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point.X + num20, point.Y + num21);
						point21.X = point16.X;
						point21.Y = point16.Y;
						break;
					}
					double num22 = value24 * (double)num;
					SurfaceTarget surfaceTarget4 = surfaceTarget3;
					OffsetTarget offsetTarget8 = offsetTarget7;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point17 = null;
					if (offsetTarget8 != null && offsetTarget8.IsValid && point != null)
					{
						try
						{
							num22 = offsetTarget8.Value - point.Offset;
						}
						catch
						{
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetOffsetFromOffsetTarget);
						}
					}
					if (surfaceTarget4 != null && surfaceTarget4.IsValid && point != null)
					{
						try
						{
							point17 = surfaceTarget4.getPointOnOffset(point.Offset + num22);
						}
						catch
						{
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, Resource.ErrorHandling_CannotGetElevationFromSurfaceTarget);
						}
					}
					else
					{
						((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, Resource.ErrorHandling_NoSpecifiedSurfaceTarget);
					}
					if (point17 == null && point != null)
					{
						((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, Resource.ErrorHandling_CannotGetElevationFromSurfaceTarget);
					}
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point18 = point17;
					point21.X = point18.X;
					point21.Y = point18.Y;
					break;
				}
				case PointGeometryType.InterpolatePoint:
				{
					string value12 = ((Literal<string>)dictionary["DestPoint"].Expression).Value;
					double value13 = ((Literal<double>)dictionary["InterpolateParam"].Expression).Value;
					string text = value12;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point8 = value.GetPoint(text);
					if (point8 == null)
					{
						if (!string.IsNullOrEmpty(text))
						{
							string msg = string.Format(Resource.ErrorHandling_CreatePoint_TargetPointNotFound, text);
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, msg);
						}
						else
						{
							point8 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(0.0, 0.0);
						}
					}
					double para = value13;
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point9 = GeometryUtility.InterpolatePoint(point, point8, para);
					point21.X = point9.X;
					point21.Y = point9.Y;
					break;
				}
				case PointGeometryType.SlopeAndDeltaX:
				{
					double value18 = ((Literal<double>)dictionary["Slope2"].Expression).Value;
					double value19 = ((Literal<double>)dictionary["DeltaX2"].Expression).Value;
					OffsetTarget offsetTarget3 = null;
					if ((Literal<OffsetTarget>)dictionary["OffsetTarget2"].Expression != null)
					{
						offsetTarget3 = ((Literal<OffsetTarget>)dictionary["OffsetTarget2"].Expression).Value;
					}
					ElevationTarget elevationTarget3 = null;
					if ((Literal<ElevationTarget>)dictionary["ElevationTarget2"].Expression != null)
					{
						elevationTarget3 = ((Literal<ElevationTarget>)dictionary["ElevationTarget2"].Expression).Value;
					}
					CrossSegmentType crossSegmentType3 = CrossSegmentType.None;
					if ((Literal<CrossSegmentType>)dictionary["Superelevation2"].Expression != null)
					{
						crossSegmentType3 = ((Literal<CrossSegmentType>)dictionary["Superelevation2"].Expression).Value;
					}
					if (runMode == Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.LayoutMode)
					{
						double num10 = value18;
						double num11 = value19;
						if (double.IsInfinity(num10))
						{
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, Resource.ErrorHandling_CreatePoint_InfiniteSlope);
						}
						double num12 = (double)num * num11;
						double num13 = Math.Abs(num12) * num10;
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point12 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point.X + num12, point.Y + num13);
						point21.X = point12.X;
						point21.Y = point12.Y;
						break;
					}
					double num14 = value18;
					double num15 = value19;
					OffsetTarget offsetTarget4 = offsetTarget3;
					ElevationTarget elevationTarget4 = elevationTarget3;
					CrossSegmentType crossSegmentType4 = crossSegmentType3;
					if (crossSegmentType4 != CrossSegmentType.None)
					{
						ICrossSlope slopeAt2 = Superelevation.Data.GetSlopeAt(crossSegmentType4);
						if (slopeAt2.IsDefined)
						{
							num14 = slopeAt2.Slope;
						}
					}
					if (double.IsInfinity(num14))
					{
						((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, Resource.ErrorHandling_CreatePoint_InfiniteSlope);
					}
					double num16 = (double)num * num15;
					double num17 = Math.Abs(num16) * num14;
					if (offsetTarget4 != null && offsetTarget4.IsValid)
					{
						try
						{
							num16 = offsetTarget4.Value - point.Offset;
							num17 = Math.Abs(num16) * num14;
						}
						catch
						{
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetOffsetFromOffsetTarget);
							num16 = (double)num * num15;
						}
					}
					if (elevationTarget4 != null && elevationTarget4.IsValid)
					{
						try
						{
							num17 = elevationTarget4.Value - point.Elevation;
						}
						catch
						{
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetElevationFromElevationTarget);
							num17 = Math.Abs(num16) * num14;
						}
					}
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point13 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point.X + num16, point.Y + num17);
					point21.X = point13.X;
					point21.Y = point13.Y;
					break;
				}
				case PointGeometryType.SlopeAndDeltaY:
				{
					double value7 = ((Literal<double>)dictionary["Slope5"].Expression).Value;
					double value8 = ((Literal<double>)dictionary["DeltaY5"].Expression).Value;
					OffsetTarget offsetTarget = null;
					if ((Literal<OffsetTarget>)dictionary["OffsetTarget5"].Expression != null)
					{
						offsetTarget = ((Literal<OffsetTarget>)dictionary["OffsetTarget5"].Expression).Value;
					}
					ElevationTarget elevationTarget = null;
					if ((Literal<ElevationTarget>)dictionary["ElevationTarget5"].Expression != null)
					{
						elevationTarget = ((Literal<ElevationTarget>)dictionary["ElevationTarget5"].Expression).Value;
					}
					CrossSegmentType crossSegmentType = CrossSegmentType.None;
					if ((Literal<CrossSegmentType>)dictionary["Superelevation5"].Expression != null)
					{
						crossSegmentType = ((Literal<CrossSegmentType>)dictionary["Superelevation5"].Expression).Value;
					}
					bool value9 = ((Literal<bool>)dictionary["ReverseSlopeDirection5"].Expression).Value;
					if (runMode == Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.LayoutMode)
					{
						double slope2 = value7;
						bool flag3 = value9;
						bool isRightSide = true;
						if ((num == -1 && !flag3) || (num == 1 && flag3))
						{
							isRightSide = false;
						}
						double num5 = value8;
						double distance = 0.0;
						if (!CalDistance(slope2, num5, isRightSide, out distance))
						{
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, Resource.ErrorHandling_CannotComputePointBySlopeAndElevation);
						}
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point5 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point.X + distance, point.Y + num5);
						point21.X = point5.X;
						point21.Y = point5.Y;
						break;
					}
					double slope3 = value7;
					double num6 = value8;
					OffsetTarget offsetTarget2 = offsetTarget;
					ElevationTarget elevationTarget2 = elevationTarget;
					CrossSegmentType crossSegmentType2 = crossSegmentType;
					bool flag4 = value9;
					bool isRightSide2 = true;
					if ((num == -1 && !flag4) || (num == 1 && flag4))
					{
						isRightSide2 = false;
					}
					if (crossSegmentType2 != CrossSegmentType.None)
					{
						ICrossSlope slopeAt = Superelevation.Data.GetSlopeAt(crossSegmentType2);
						if (slopeAt.IsDefined)
						{
							slope3 = slopeAt.Slope;
						}
					}
					double num7 = num6;
					double distance2 = 0.0;
					if (elevationTarget2 != null && elevationTarget2.IsValid)
					{
						try
						{
							num7 = elevationTarget2.Value - point.Elevation;
						}
						catch
						{
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetElevationFromElevationTarget);
							num7 = num6;
						}
					}
					bool flag5 = false;
					if (offsetTarget2 != null && offsetTarget2.IsValid)
					{
						try
						{
							distance2 = offsetTarget2.Value - point.Offset;
							flag5 = true;
						}
						catch
						{
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Warning, Resource.ErrorHandling_CannotGetOffsetFromOffsetTarget);
						}
					}
					if (!flag5 && !CalDistance(slope3, num7, isRightSide2, out distance2))
					{
						((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, Resource.ErrorHandling_CannotComputePointBySlopeAndElevation);
					}
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point6 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point.X + distance2, point.Y + num7);
					point21.X = point6.X;
					point21.Y = point6.Y;
					break;
				}
				case PointGeometryType.SlopeToSurface:
				{
					double value3 = ((Literal<double>)dictionary["Slope3"].Expression).Value;
					SurfaceTarget surfaceTarget = null;
					if ((Literal<SurfaceTarget>)dictionary["SurfaceTarget3"].Expression != null)
					{
						surfaceTarget = ((Literal<SurfaceTarget>)dictionary["SurfaceTarget3"].Expression).Value;
					}
					bool value4 = ((Literal<bool>)dictionary["ReverseSlopeDirection3"].Expression).Value;
					double value5 = ((Literal<double>)dictionary["DeltaXForLayout3"].Expression).Value;
					bool value6 = ((Literal<bool>)dictionary["ShowErrors"].Expression).Value;
					if (runMode == Autodesk.SubassemblyComposer.WorkflowEngine.SubassemblyRunMode.LayoutMode)
					{
						double num2 = value3;
						bool flag = value4;
						((IRecordError)this).ShowErrors = ShowErrors;
						double num3 = value5;
						double num4 = 1.0;
						if ((num == -1 && !flag) || (num == 1 && flag))
						{
							num4 = -1.0;
						}
						Autodesk.SubassemblyComposer.WorkflowEngine.Point point2 = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point.X + num3 * num4, point.Y + Math.Abs(num3) * num2);
						point21.X = point2.X;
						point21.Y = point2.Y;
						break;
					}
					double slope = value3;
					SurfaceTarget surfaceTarget2 = surfaceTarget;
					bool flag2 = value4;
					((IRecordError)this).ShowErrors = value6;
					bool isTowardRight = true;
					if ((num == -1 && !flag2) || (num == 1 && flag2))
					{
						isTowardRight = false;
					}
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point3 = null;
					if (surfaceTarget2 != null && surfaceTarget2.IsValid && point != null)
					{
						try
						{
							point3 = surfaceTarget2.getDaylightPoint(point, slope, isTowardRight);
						}
						catch
						{
							((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, Resource.ErrorHandling_NoIntersectionPtWithSurfaceTarget);
						}
					}
					if (point3 == null)
					{
						((IRecordError)this).RecordErrorInfo((ActivityContext)context, ErrorLevel.Error, Resource.ErrorHandling_NoIntersectionPtWithSurfaceTarget);
					}
					Autodesk.SubassemblyComposer.WorkflowEngine.Point point4 = point3;
					point21.X = point4.X;
					point21.Y = point4.Y;
					break;
				}
				}
				value.Points.Add(point21);
				if (!((Literal<bool>)((CreatePoint)activity).AutoLink.Expression).Value || point == null || point21 == null)
				{
					break;
				}
				Link link = new Link(point, point21);
				link.ActivityId = point21.ActivityId;
				link.GeometryId = ((Literal<string>)((CreatePoint)activity).AutoLinkGeometryName.Expression).Value;
				if (((CreatePoint)activity).AutoLinkCodes != null)
				{
					int count2 = ((CreatePoint)activity).AutoLinkCodes.Count;
					link.Codes = new string[count2];
					for (int j = 0; j < count2; j++)
					{
						link.Codes[j] = ((Literal<string>)((CreatePoint)activity).AutoLinkCodes[j].Expression).Value;
					}
				}
				link.IsAuxiliary = point.IsAuxiliary || point21.IsAuxiliary;
				if (ActivityDesigntime.Instance != null)
				{
					link.DisplayName = ((CreatePoint)activity).DisplayName;
				}
				value.Links.Add(link);
				break;
			}
		}
		foreach (Link link2 in offset.Links)
		{
			foreach (Activity activity2 in Activities)
			{
				if (!(activity2 is CreateLink) || link2.ActivityId != (((CreateLink)activity2).ActivityId.Expression as Literal<int>).Value)
				{
					continue;
				}
				link2.GeometryId = ((Literal<string>)((CreateLink)activity2).LinkNumber.Expression).Value;
				link2.ApplyAOR = ((Literal<bool>)((CreateLink)activity2).ApplyAOR.Expression).Value;
				string value30 = ((Literal<string>)((CreateLink)activity2).StartPoint.Expression).Value;
				string value31 = ((Literal<string>)((CreateLink)activity2).EndPoint.Expression).Value;
				link2.Points.Clear();
				link2.Points.Add(value.GetPoint(value30));
				link2.Points.Add(value.GetPoint(value31));
				if (((CreateLink)activity2).LinkCodes != null)
				{
					int count3 = ((CreateLink)activity2).LinkCodes.Count;
					link2.Codes = new string[count3];
					for (int k = 0; k < count3; k++)
					{
						link2.Codes[k] = ((Literal<string>)((CreateLink)activity2).LinkCodes[k].Expression).Value;
					}
				}
				break;
			}
			value.Links.Add(link2);
		}
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

	private bool SortLinks(List<Link> links, List<Link> sortedLinks, ref bool isPloyLine, ref bool isPloyGon)
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
				isPloyLine = false;
				isPloyGon = false;
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
				isPloyLine = false;
				isPloyGon = false;
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
						isPloyLine = false;
						break;
					}
					if (num6 == links.Count - 1)
					{
						if (next2 != -1)
						{
							isPloyLine = false;
						}
						else
						{
							isPloyLine = true;
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
								isPloyGon = false;
							}
							else
							{
								isPloyGon = true;
							}
						}
						continue;
					}
					isPloyGon = false;
					break;
				}
				continue;
			}
			}
			break;
		}
		if (isPloyLine | isPloyGon)
		{
			return true;
		}
		return false;
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
		BindCodeArguments(metadata, OffsetCodes, this.PropertyName((CreateOffset p) => p.OffsetCodes));
		ValidateCodeArguments(metadata, OffsetCodes);
	}
}
