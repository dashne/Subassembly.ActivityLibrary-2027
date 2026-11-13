using System;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
internal class EnumItemProcessorAttribute : Attribute
{
	public Type ProcessorType { get; private set; }

	public EnumItemProcessorAttribute(Type processorType)
	{
		ProcessorType = processorType;
	}
}
