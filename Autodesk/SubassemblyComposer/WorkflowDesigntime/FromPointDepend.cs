using System.Activities;
using System.Activities.Presentation.Model;
using System.Text.RegularExpressions;
using Autodesk.SubassemblyComposer.ActivityLibrary;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.WorkflowDesigntime;

internal class FromPointDepend : ArgumentDependenceAttribute
{
	public override void Execute(ActivityDesigntime designTime, ModelProperty modelProperty)
	{
		if (modelProperty.Parent != null && modelProperty.Parent.Equals(designTime.Selection.SelectedModelItem))
		{
			ModelProperty modelProperty2 = modelProperty.Parent.Properties["AutoLink"];
			if (modelProperty.Value == null)
			{
				modelProperty2.SetValue(new InArgument<bool>
				{
					Expression = false
				});
			}
			else if (!(modelProperty.Value.GetCurrentValue() is InArgument<string> { Expression: not null } inArgument) || inArgument.Expression.ToString().Equals(GlobleString.String_Origin) || (modelProperty.Parent.ItemType == typeof(CreatePoint) && !Regex.Match(inArgument.Expression.ToString(), new PointAutoNumberingAttribute().IDPattern).Success))
			{
				modelProperty2.SetValue(new InArgument<bool>
				{
					Expression = false
				});
			}
		}
	}
}
