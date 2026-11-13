using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal sealed class AuxPointAutoNumberingAttribute : GeometryAutoNumberingAttribute
{
	public override string ValidatePromptMessage => Resource.Message_Name_AuxPoint;

	public override string IDPattern => "^A[Pp]\\d+$";

	public override string IDPrefix => "AP";
}
