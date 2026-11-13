using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.Validation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Autodesk.SubassemblyComposer.ActivityLibrary.ActivityDesigner;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[Designer(typeof(CreateShapeDesigner))]
[CreateShapeAutoNumbering]
[ToolboxBitmap(typeof(CreateShape), "Resources.Icons.shape_create_toolbox.ico")]
public class CreateShape : SubassemblyActivity
{
	public const string c_Links = "Links";

	public const char linksSeparator = ',';

	private InArgument<string> _shapeNumber;

	[CustomCategory(typeof(Resource), "Category_Shape", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ShapeNumber", 1)]
	[Editor(typeof(StringTextBox), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[ShapeAutoNumbering]
	public InArgument<string> ShapeNumber
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateShape p) => p.ShapeNumber)) as InArgument<string>;
		}
		set
		{
			_shapeNumber = ShapeNumber;
			PropertyCollection_Set(this.PropertyName((CreateShape p) => p.ShapeNumber), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Shape", 1)]
	[CustomDisplayName(typeof(Resource), "DisplayName_ShapeCodes", 2)]
	[Editor(typeof(ShapeCodeComboBox), typeof(PropertyValueEditor))]
	[CanReferParameterName]
	[CanReferVariableName]
	[CanReferShapeCodeName]
	public List<InArgument<string>> ShapeCodes
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateShape p) => p.ShapeCodes)) as List<InArgument<string>>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateShape p) => p.ShapeCodes), value);
		}
	}

	[CustomCategory(typeof(Resource), "Category_Component", 2)]
	[CustomDisplayName(typeof(Resource), "DisplayName_Links", 1)]
	[Editor(typeof(InlineLinksUserCtrl), typeof(PropertyValueEditor))]
	[RequiredArgument]
	[CanReferGeometryName]
	public InArgument<string> Links
	{
		get
		{
			return PropertyCollection_Get(this.PropertyName((CreateShape p) => p.Links)) as InArgument<string>;
		}
		set
		{
			PropertyCollection_Set(this.PropertyName((CreateShape p) => p.Links), value);
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

	private ValidationError ConstructTempShape(Shape shape, List<string> linkNames, List<CodePathTreeNode> path)
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
				shape.Links.Add(link);
				continue;
			}
			return new ValidationError(string.Format(Resource.Message_Validate_Shape_WrongLink, linkName), isWarning: true, "Links");
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

	public CreateShape()
	{
		PropertyCollection_Set(this.PropertyName((CreateShape p) => p.ShapeNumber), _shapeNumber, ShapeNumberOnChanged);
	}

	protected void ShapeNumberOnChanged(object value)
	{
		if (value != null && (InArgument<string>)value != _shapeNumber)
		{
			InArgument<string> shapeNumber = _shapeNumber;
			_shapeNumber = (InArgument<string>)value;
			OnGeometryNameChanged(this.PropertyName((CreateShape p) => p.ShapeNumber), shapeNumber, _shapeNumber);
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
		Shape shape = new Shape();
		string[] array = context.GetValue(Links).Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i].Trim();
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			if (GeometryNamePatterns.IsLoopName(text))
			{
				string value2 = text + "L";
				bool flag = false;
				foreach (Link link2 in value.Links)
				{
					if (link2.GeometryId.StartsWith(value2, StringComparison.OrdinalIgnoreCase))
					{
						shape.Links.Add(link2);
						flag = true;
					}
				}
				if (!flag)
				{
					RecordErrorInfo(context, ErrorLevel.Warning, string.Format(Resource.ErrorHandling_CreateShape_LinkNotFound, text));
				}
			}
			else
			{
				Link link = value.GetLink(text);
				if (link != null)
				{
					shape.Links.Add(link);
				}
				else
				{
					RecordErrorInfo(context, ErrorLevel.Warning, string.Format(Resource.ErrorHandling_CreateShape_LinkNotFound, text));
				}
			}
		}
		shape.ActivityId = context.GetValue(ActivityId);
		shape.GeometryId = context.GetValue(ShapeNumber);
		shape.Codes = GetCodes(context, ShapeCodes);
		if (ActivityDesigntime.Instance != null)
		{
			shape.DisplayName = DisplayName;
		}
		value.Shapes.Add(shape);
		if (shape.Points == null)
		{
			RecordErrorInfo(context, ErrorLevel.Error, Resource.Message_Validate_Shape_Closure);
		}
	}

	protected override void CacheMetadata(CodeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
		BindCodeArguments(metadata, ShapeCodes, this.PropertyName((CreateShape p) => p.ShapeCodes));
		ValidateCodeArguments(metadata, ShapeCodes);
	}

	public override List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		return new List<ValidationError>();
	}
}
