using System;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class GeometryAutoNumberingAttribute : Attribute
{
	public string CheckPropertyName { get; protected set; }

	public bool AllowNull { get; set; }

	public virtual string IDPattern => string.Empty;

	public virtual string IDPrefix => string.Empty;

	public virtual string ValidatePromptMessage => string.Empty;

	public GeometryAutoNumberingAttribute()
	{
		CheckPropertyName = string.Empty;
		AllowNull = false;
	}

	public string GetNextGeometryName(Type type)
	{
		return IDPrefix + AutoNumberEngine.NextGeometryNumber(type);
	}
}
