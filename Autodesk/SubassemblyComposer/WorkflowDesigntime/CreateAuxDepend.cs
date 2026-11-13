using System.Activities;
using System.Activities.Presentation.Model;
using System.ComponentModel;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class CreateAuxDepend : ArgumentDependenceAttribute
{
	public override void Execute(ActivityDesigntime designTime, ModelProperty modelProperty)
	{
		ModelItem parent = modelProperty.Parent;
		GeometryAutoNumberingAttribute obj = TypeDescriptor.GetProperties(parent.GetCurrentValue())["PointNumber"].Attributes[typeof(GeometryAutoNumberingAttribute)] as GeometryAutoNumberingAttribute;
		string nextGeometryName = obj.GetNextGeometryName(obj.GetType());
		parent.Properties["PointNumber"].SetValue(new InArgument<string>(nextGeometryName));
		parent.Properties["DisplayName"].SetValue(nextGeometryName);
		ActivityDesigntime.Instance.RefreshPropertyGrid();
	}
}
