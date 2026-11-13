using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal sealed class OffsetAutoNumberingAttribute : GeometryAutoNumberingAttribute
{
	public override string ValidatePromptMessage => Resource.Message_Name_Offset;

	public override string IDPattern => "^[Oo]\\d+$";

	public override string IDPrefix => "O";
}
