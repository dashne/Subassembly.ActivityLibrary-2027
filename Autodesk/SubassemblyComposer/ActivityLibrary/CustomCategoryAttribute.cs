using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Resources;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
internal class CustomCategoryAttribute : CategoryAttribute
{
	private static Dictionary<string, string> _cacheResources = new Dictionary<string, string>();

	public int Index { get; private set; }

	private Type ResourceManagerProvider { get; set; }

	public CustomCategoryAttribute(Type resourceManagerProvider, string resourceKey, int index)
		: base(resourceKey)
	{
		ResourceManagerProvider = resourceManagerProvider;
		Index = index;
	}

	protected override string GetLocalizedString(string value)
	{
		string text = ResourceManagerProvider.FullName + value;
		if (_cacheResources.ContainsKey(text))
		{
			return _cacheResources[text];
		}
		PropertyInfo[] properties = ResourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.PropertyType == typeof(ResourceManager))
			{
				string text2 = ((ResourceManager)propertyInfo.GetValue(null, null)).GetString(value);
				_cacheResources.Add(text, text2);
				return text2;
			}
		}
		return string.Empty;
	}
}
