using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class LinkAutoNumberingAttribute : GeometryAutoNumberingAttribute
{
	public override string ValidatePromptMessage => Resource.Message_Name_Link;

	public override string IDPattern => "^[Ll]\\d+$";

	public override string IDPrefix => "L";

	public string StartPointPropertyName { get; protected set; }

	public string EndPointPropertyName { get; protected set; }

	public LinkAutoNumberingAttribute(string startPointPropertyName, string endPointPropertyName)
	{
		StartPointPropertyName = startPointPropertyName;
		EndPointPropertyName = endPointPropertyName;
	}

	public LinkAutoNumberingAttribute(string checkPropertyName, string startPointPropertyName, string endPointPropertyName, bool allowNull)
		: this(startPointPropertyName, endPointPropertyName)
	{
		CheckPropertyName = checkPropertyName;
		AllowNull = allowNull;
	}
}
