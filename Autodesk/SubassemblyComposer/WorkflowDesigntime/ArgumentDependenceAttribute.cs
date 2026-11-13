using System;
using System.Activities.Presentation.Model;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

[AttributeUsage(AttributeTargets.Property, Inherited = true)]
internal class ArgumentDependenceAttribute : Attribute
{
	public virtual void Execute(ActivityDesigntime designTime, ModelProperty modelProperty)
	{
	}
}
