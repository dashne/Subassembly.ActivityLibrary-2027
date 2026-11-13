using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Resources;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
internal class CustomDisplayNameAttribute : DisplayNameAttribute
{
	private static Dictionary<string, string> _cacheResources = new Dictionary<string, string>();

	public int Index { get; private set; }

	public CustomDisplayNameAttribute(Type resourceManagerProvider, string resourceKey, int index)
	{
		Index = index;
		string key = resourceManagerProvider.FullName + resourceKey;
		if (_cacheResources.ContainsKey(key))
		{
			DisplayNameValue = _cacheResources[key];
			return;
		}
		PropertyInfo[] properties = resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.PropertyType == typeof(ResourceManager))
			{
				ResourceManager resourceManager = (ResourceManager)propertyInfo.GetValue(null, null);
				DisplayNameValue = resourceManager.GetString(resourceKey);
				_cacheResources.Add(key, DisplayNameValue);
				break;
			}
		}
	}

	public CustomDisplayNameAttribute(string resourceKey, int index)
	{
		Index = index;
		DisplayNameValue = resourceKey;
	}
}
