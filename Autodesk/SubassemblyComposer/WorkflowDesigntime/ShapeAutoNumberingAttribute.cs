using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal sealed class ShapeAutoNumberingAttribute : GeometryAutoNumberingAttribute
{
	public override string ValidatePromptMessage => Resource.Message_Name_Shape;

	public override string IDPattern => "^[Ss]\\d+$";

	public override string IDPrefix => "S";
}
