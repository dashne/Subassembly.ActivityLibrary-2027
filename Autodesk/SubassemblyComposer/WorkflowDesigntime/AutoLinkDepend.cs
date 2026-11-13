using System.Activities;
using System.Activities.Presentation.Model;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.PropertyEditor;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class AutoLinkDepend : ArgumentDependenceAttribute
{
	public override void Execute(ActivityDesigntime designTime, ModelProperty modelProperty)
	{
		ModelItem parent = modelProperty.Parent;
		bool flag = bool.Parse((modelProperty.Value.GetCurrentValue() as InArgument<bool>).Expression.ToString());
		string stringPropertyValue = ActivityHelpers.GetStringPropertyValue(parent, "DisplayName");
		string newComment = CommentsConverter.ExtractComment(stringPropertyValue);
		stringPropertyValue = ActivityHelpers.GetStringPropertyValue(parent, "PointNumber");
		ModelProperty modelProperty2 = parent.Properties["AutoLinkGeometryName"];
		if (flag)
		{
			GeometryAutoNumberingAttribute obj = TypeDescriptor.GetProperties(parent.GetCurrentValue())["AutoLinkGeometryName"].Attributes[typeof(GeometryAutoNumberingAttribute)] as GeometryAutoNumberingAttribute;
			string nextGeometryName = obj.GetNextGeometryName(obj.GetType());
			modelProperty2.SetValue(new InArgument<string>(nextGeometryName));
			if (!string.IsNullOrEmpty(stringPropertyValue))
			{
				stringPropertyValue += SubassemblyActivityAutoNumberingAttribute.DisplayNameSeparator;
			}
			stringPropertyValue += nextGeometryName;
		}
		else
		{
			modelProperty2.SetValue(null);
		}
		stringPropertyValue = CommentsConverter.InjectComment(stringPropertyValue, newComment);
		parent.Properties["DisplayName"].SetValue(stringPropertyValue);
		ActivityDesigntime.Instance.RefreshPropertyGrid();
	}
}
