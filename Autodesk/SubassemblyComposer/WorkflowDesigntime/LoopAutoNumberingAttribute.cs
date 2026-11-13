using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal sealed class LoopAutoNumberingAttribute : GeometryAutoNumberingAttribute
{
	public override string ValidatePromptMessage => Resource.Message_Name_Loop;

	public override string IDPattern => "^(LO|lo)\\d+$";

	public override string IDPrefix => "LO";
}
