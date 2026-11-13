using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal sealed class AuxLinkAutoNumberingAttribute : LinkAutoNumberingAttribute
{
	public override string ValidatePromptMessage => Resource.Message_Name_AuxLink;

	public override string IDPattern => "^A[Ll]\\d+$";

	public override string IDPrefix => "AL";

	public AuxLinkAutoNumberingAttribute(string startPointPropertyName, string endPointPropertyName)
		: base(startPointPropertyName, endPointPropertyName)
	{
	}

	public AuxLinkAutoNumberingAttribute(string checkPropertyName, string startPointPropertyName, string endPointPropertyName, bool allowNull)
		: base(checkPropertyName, startPointPropertyName, endPointPropertyName, allowNull)
	{
	}
}
