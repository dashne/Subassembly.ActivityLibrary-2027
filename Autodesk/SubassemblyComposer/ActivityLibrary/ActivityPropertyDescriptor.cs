using System;
using System.ComponentModel;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class ActivityPropertyDescriptor : PropertyDescriptor
{
	private Type _componentType;

	private Type _propertyType;

	public override Type ComponentType => _componentType;

	public override bool IsReadOnly => false;

	public override Type PropertyType => _propertyType;

	public ActivityPropertyDescriptor(string propertyName, Type componentType, Type propertyType, params Attribute[] attributes)
		: base(propertyName, attributes)
	{
		_componentType = componentType;
		_propertyType = propertyType;
	}

	public override bool CanResetValue(object component)
	{
		return false;
	}

	public override object GetValue(object component)
	{
		if (component.GetType() == _componentType)
		{
			return ((ICustomAccessProperty)component).GetPropertyValue(Name);
		}
		return null;
	}

	public override void SetValue(object component, object value)
	{
		if (component.GetType() == _componentType)
		{
			((ICustomAccessProperty)component).SetPropertyValue(Name, value);
		}
	}

	public override void ResetValue(object component)
	{
	}

	public override bool ShouldSerializeValue(object component)
	{
		return true;
	}
}
