using System.Activities.Validation;
using System.Collections.Generic;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal interface IDesigntimeValidate
{
	List<ValidationError> SubValidateInDesigntime(int activityId);
}
