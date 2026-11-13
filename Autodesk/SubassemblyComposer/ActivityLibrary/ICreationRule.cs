using System.Activities;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal interface ICreationRule : ICustomAccessProperty
{
	void CacheMetadata(CodeActivityMetadata metadata, Dictionary<string, Argument> arguments);

	PropertyDescriptorCollection GetProperties(object ownerInstance);

	void RefreshArguments(Dictionary<string, Argument> arguments);

	void SyncProperty(Dictionary<string, Argument> arguments);

	List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames);
}
