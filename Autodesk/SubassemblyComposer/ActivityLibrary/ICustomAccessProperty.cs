namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal interface ICustomAccessProperty
{
	object GetPropertyValue(string propertyName);

	void SetPropertyValue(string propertyName, object newValue);
}
