using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

public abstract class CustomCondition : ConditionBase<bool>
{
	[Browsable(false)]
	public virtual string TrueLabel => Resource.CustomCondition_TrueLable;

	[Browsable(false)]
	public virtual string FalseLabel => Resource.CustomCondition_FalseLable;
}
