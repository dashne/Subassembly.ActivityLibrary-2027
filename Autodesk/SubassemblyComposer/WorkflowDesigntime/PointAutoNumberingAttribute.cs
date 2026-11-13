using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal sealed class PointAutoNumberingAttribute : GeometryAutoNumberingAttribute
{
	public override string IDPattern => "^[Pp]\\d+$|^[Ll][Oo]\\d+(\\.EndPoint)?$";

	public override string IDPrefix => "P";

	public override string ValidatePromptMessage => Resource.Message_Name_Point;
}
