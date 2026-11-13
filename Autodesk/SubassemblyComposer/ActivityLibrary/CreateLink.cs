using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.GeomMath;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(CreateLinkDesigner))]
[CreateLinkAutoNumbering]
[ToolboxBitmap(typeof(CreatePoint), "Resources.Icons.link_create_toolbox.ico")]
public class CreateLink : SubassemblyActivity
{
	public const string c_StartPoint = "StartPoint";

	public const string c_EndPoint = "EndPoint";

	public const string c_LinkNumber = "LinkNumber";

	private InArgument<string> _linkNumber;

	[CustomCategory(typeof(Resource), "Category_Link", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_LinkNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[LinkAutoNumbering("StartPoint", "EndPoint")]
	public InArgument<string> LinkNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateLink p) => p.LinkNumber)) as InArgument<string>;
		}
		set
		{
			_linkNumber = PropertyCollection_Get(this.PropertyName((CreateLink p) => p.LinkNumber)) as InArgument<string>;
			PropertyCollection_Set(this.PropertyName((CreateLink p) => p.LinkNumber), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Link", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_LinkCodes", 2)]
	[Editor(typeof(LinkCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferLinkCodeName]
	public List<InArgument<string>> LinkCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateLink p) => p.LinkCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateLink p) => p.LinkCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Link", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ApplyAOR", 3)]
	[Editor(typeof(InlineCheckBox), typeof(PropertyValueEditor))]
	public InArgument<bool> ApplyAOR
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateLink p) => p.ApplyAOR)) as InArgument<bool>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateLink p) => p.ApplyAOR), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Position", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_StartPoint", 1)]
	[Editor(typeof(IsEnabledPointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public virtual InArgument<string> StartPoint
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateLink p) => p.StartPoint)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateLink p) => p.StartPoint), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Position", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_EndPoint", 2)]
	[Editor(typeof(IsEnabledPointComboBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public virtual InArgument<string> EndPoint
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateLink p) => p.EndPoint)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateLink p) => p.EndPoint), value);
		}
	}

	[Browsable(false)]
	[CustomCategory(typeof(Resource), "Category_Position", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_IsEnabled", 3)]
	public InArgument<bool> IsEnabled
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateLink p) => p.IsEnabled)) as InArgument<bool>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateLink p) => p.IsEnabled), value);
		}
	}

	private object GetActivity(int id, Dictionary<int, CustomActivity> dictActivity)
	{
		if (dictActivity.ContainsKey(id))
		{
			return dictActivity[id];
		}
		try
		{
			object currentValue = FindActivityOnIdHelper.Execute(id).GetCurrentValue();
			if (currentValue is CustomActivity)
			{
				dictActivity.Add(id, (CustomActivity)currentValue);
			}
			return currentValue;
		}
		catch
		{
			return null;
		}
	}

	private List<CustomActivity> GetActivityPath(IEnumerable<int> lstIds, Dictionary<int, CustomActivity> dictActivity)
	{
		try
		{
			return (from i in lstIds
				select GetActivity(i, dictActivity) into i
				where i is CustomActivity
				select i).Cast<CustomActivity>().Reverse().ToList();
		}
		catch
		{
			return null;
		}
	}

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		List<ValidationError> list = new List<ValidationError>();
		if (LinkNumber != null && LinkNumber.Expression != null)
		{
			string value = (LinkNumber.Expression as Literal<string>).Value;
			if (!GeomertyNameCheckHelper.Execute(ActivityDesigntime.Instance.CodePathTree, value, activityId))
			{
				list.Add(new ValidationError(Resource.Message_Validate_Duplicate_Name, isWarning: false, "LinkNumber"));
			}
		}
		if (!string.IsNullOrEmpty(LayoutCantData.LeftRail) && !string.IsNullOrEmpty(LayoutCantData.RightRail))
		{
			if (StartPoint != null && EndPoint != null && ((Literal<bool>)ApplyAOR.Expression).Value)
			{
				string sp = ((Literal<string>)StartPoint.Expression).Value;
				string ep = ((Literal<string>)EndPoint.Expression).Value;
				List<CodePathTreeNode> path = new List<CodePathTreeNode>();
				Dictionary<int, CustomActivity> dictActivity = new Dictionary<int, CustomActivity>();
				foreach (List<CodePathTreeNode> item3 in ActivityDesigntime.Instance.CodePathTree.Paths(path, sp, ep))
				{
					try
					{
						List<CustomActivity> activityPath = GetActivityPath(item3.Select((CodePathTreeNode i) => i.ActivityId).ToList(), dictActivity);
						CustomActivity item = activityPath.Single((CustomActivity i) => i.GeneratesPoint(sp));
						int num = activityPath.IndexOf(item);
						CustomActivity item2 = activityPath.Single((CustomActivity i) => i.GeneratesPoint(ep));
						int num2 = activityPath.IndexOf(item2);
						using IEnumerator<CustomActivity> enumerator2 = (from i in activityPath.GetRange(num + 1, num2 - num - 1)
							where i.DependsOnPoint(sp)
							select i).GetEnumerator();
						if (enumerator2.MoveNext())
						{
							CustomActivity current2 = enumerator2.Current;
							string message = string.Format(Resource.Message_Validate_ApplyAOR_Error, current2.DisplayName, ((Literal<string>)LinkNumber.Expression).Value);
							list.Add(new ValidationError(message, isWarning: false));
						}
					}
					catch
					{
					}
				}
			}
		}
		ObservableCollection<string> allValidPointNames = ValidGeometryNamesHelper.GetAllValidPointNames(ActivityDesigntime.Instance.CodePathTree, activityId, includeOrigin: false);
		if (!this.PointArgumentValidate(StartPoint, allValidPointNames))
		{
			string message2 = string.Format(Resource.Message_Validate_Invalid_Value, Resource.DisplayName_StartPoint);
			list.Add(new ValidationError(message2, isWarning: false, "StartPoint"));
		}
		if (!this.PointArgumentValidate(EndPoint, allValidPointNames))
		{
			string message3 = string.Format(Resource.Message_Validate_Invalid_Value, Resource.DisplayName_EndPoint);
			list.Add(new ValidationError(message3, isWarning: false, "EndPoint"));
		}
		return list;
	}

	public CreateLink()
	{
		PropertyCollection_Set(this.PropertyName((CreateLink p) => p.LinkNumber), null, LinkNumberOnChanged);
		IsEnabled = true;
		ApplyAOR = false;
	}

	protected void LinkNumberOnChanged(object value)
	{
		if (value != null && value != _linkNumber)
		{
			InArgument<string> linkNumber = _linkNumber;
			_linkNumber = value as InArgument<string>;
			OnGeometryNameChanged(this.PropertyName((CreateLink p) => p.LinkNumber), linkNumber, _linkNumber);
		}
	}

	protected override void LayoutExecute(CodeActivityContext context)
	{
		InternalExecute(context);
	}

	protected override void RoadwayExecute(CodeActivityContext context)
	{
		InternalExecute(context);
	}

	private void InternalExecute(CodeActivityContext context)
	{
		Geometry value = context.GetValue(Geometry);
		Autodesk.SubassemblyComposer.WorkflowEngine.Point startPoint = value.GetPoint(context.GetValue(StartPoint));
		if (startPoint == null)
		{
			string msg = string.Format(Resource.ErrorHandling_CreateLink_StartPointNotFound, context.GetValue(StartPoint));
			RecordErrorInfo(context, ErrorLevel.Error, msg);
			return;
		}
		Autodesk.SubassemblyComposer.WorkflowEngine.Point endPoint = value.GetPoint(context.GetValue(EndPoint));
		if (endPoint == null)
		{
			string msg2 = string.Format(Resource.ErrorHandling_CreateLink_EndPointNotFound, context.GetValue(EndPoint));
			RecordErrorInfo(context, ErrorLevel.Error, msg2);
			return;
		}
		if (context.GetValue(ApplyAOR))
		{
			Link link = new Link(startPoint, endPoint);
			link.ActivityId = context.GetValue(ActivityId);
			link.GeometryId = context.GetValue(LinkNumber);
			link.ApplyAOR = context.GetValue(ApplyAOR);
			link.Codes = GetCodes(context, LinkCodes);
			link.IsAuxiliary = IsCreatingAux;
			if (ActivityDesigntime.Instance != null)
			{
				link.DisplayName = DisplayName;
			}
			if (RuntimeCantData.IsFromCorridor)
			{
				string leftRail = LayoutCantData.LeftRail;
				string rightRail = LayoutCantData.RightRail;
				string cantPivotType = RuntimeCantData.PivotType.ToString();
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point = null;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point2 = null;
				Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point> dictionary = new Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point>();
				foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point5 in value.Points)
				{
					if (point5.GeometryId != "")
					{
						dictionary[point5.GeometryId] = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point5.X, point5.Y);
					}
				}
				if (leftRail != null && dictionary.ContainsKey(leftRail))
				{
					point = dictionary[leftRail];
				}
				if (rightRail != null && dictionary.ContainsKey(rightRail))
				{
					point2 = dictionary[rightRail];
				}
				if (point != null && point2 != null)
				{
					double leftRailDeltaElevation = RuntimeCantData.LeftRailDeltaElevation;
					double rightRailDeltaElevation = RuntimeCantData.RightRailDeltaElevation;
					bool invalidElevation = false;
					Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point> cantPoints = GeometryMath.getCantPoints(new List<Link> { link }, dictionary, cantPivotType, point, leftRailDeltaElevation, point2, rightRailDeltaElevation, ref invalidElevation);
					foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point6 in value.Points)
					{
						if (cantPoints.ContainsKey(point6.GeometryId))
						{
							point6.X = cantPoints[point6.GeometryId].X;
							point6.Y = cantPoints[point6.GeometryId].Y;
						}
					}
				}
			}
			else
			{
				string leftRail2 = LayoutCantData.LeftRail;
				string rightRail2 = LayoutCantData.RightRail;
				string cantPivotType2 = LayoutCantData.CantPivotType;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point3 = null;
				Autodesk.SubassemblyComposer.WorkflowEngine.Point point4 = null;
				Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point> dictionary2 = new Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point>();
				foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point7 in value.Points)
				{
					if (point7.GeometryId != "")
					{
						dictionary2[point7.GeometryId] = new Autodesk.SubassemblyComposer.WorkflowEngine.Point(point7.X, point7.Y);
					}
				}
				if (leftRail2 != null && dictionary2.ContainsKey(leftRail2))
				{
					point3 = dictionary2[leftRail2];
				}
				if (rightRail2 != null && dictionary2.ContainsKey(rightRail2))
				{
					point4 = dictionary2[rightRail2];
				}
				if (point3 != null && point4 != null)
				{
					double result = 0.0;
					double.TryParse(LayoutCantData.LeftRailDeltaElevation, out result);
					double result2 = 0.0;
					double.TryParse(LayoutCantData.RightRailDeltaElevation, out result2);
					bool invalidElevation2 = false;
					Dictionary<string, Autodesk.SubassemblyComposer.WorkflowEngine.Point> cantPoints2 = GeometryMath.getCantPoints(new List<Link> { link }, dictionary2, cantPivotType2, point3, result, point4, result2, ref invalidElevation2);
					foreach (Autodesk.SubassemblyComposer.WorkflowEngine.Point point8 in value.Points)
					{
						if (cantPoints2.ContainsKey(point8.GeometryId))
						{
							point8.X = cantPoints2[point8.GeometryId].X;
							point8.Y = cantPoints2[point8.GeometryId].Y;
						}
					}
				}
			}
		}
		Link link2 = new Link(value.Points.Single((Autodesk.SubassemblyComposer.WorkflowEngine.Point i) => i.ActivityId == startPoint.ActivityId && i.GeometryId == startPoint.GeometryId), value.Points.Single((Autodesk.SubassemblyComposer.WorkflowEngine.Point i) => i.ActivityId == endPoint.ActivityId && i.GeometryId == endPoint.GeometryId));
		link2.ActivityId = context.GetValue(ActivityId);
		link2.GeometryId = context.GetValue(LinkNumber);
		link2.ApplyAOR = context.GetValue(ApplyAOR);
		link2.Codes = GetCodes(context, LinkCodes);
		link2.IsAuxiliary = IsCreatingAux;
		if (ActivityDesigntime.Instance != null)
		{
			link2.DisplayName = DisplayName;
		}
		value.Links.Add(link2);
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
		BindCodeArguments(metadata, LinkCodes, this.PropertyName((CreateLink p) => p.LinkCodes));
		ValidateCodeArguments(metadata, LinkCodes);
	}

	public override bool DependsOnPoint(string pointNumber)
	{
		try
		{
			return ((Literal<string>)StartPoint.Expression).Value == pointNumber || ((Literal<string>)EndPoint.Expression).Value == pointNumber;
		}
		catch
		{
			return false;
		}
	}
}
