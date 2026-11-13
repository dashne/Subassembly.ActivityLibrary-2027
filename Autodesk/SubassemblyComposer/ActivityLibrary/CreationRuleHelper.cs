using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal static class CreationRuleHelper
{
	private class ActivityPropertyInfo
	{
		public string PropertyName { get; set; }

		public Type PropertyType { get; set; }

		public List<Attribute> Attributes { get; set; }

		public ActivityPropertyInfo()
		{
			Attributes = new List<Attribute>();
		}
	}

	private static Dictionary<string, Collection<ActivityPropertyInfo>> _cachedPropertyInfo = new Dictionary<string, Collection<ActivityPropertyInfo>>();

	public static Dictionary<TEnum, TCreationRule> GetRuleProcessors<TEnum, TCreationRule>()
	{
		Type typeFromHandle = typeof(TEnum);
		if (typeof(TEnum).BaseType != typeof(Enum))
		{
			throw new InvalidOperationException();
		}
		Dictionary<TEnum, TCreationRule> dictionary = new Dictionary<TEnum, TCreationRule>();
		FieldInfo[] fields = typeFromHandle.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(EnumItemProcessorAttribute), inherit: false);
			if (customAttributes != null && customAttributes.Length == 1)
			{
				EnumItemProcessorAttribute enumItemProcessorAttribute = customAttributes[0] as EnumItemProcessorAttribute;
				if (!typeof(TCreationRule).IsAssignableFrom(enumItemProcessorAttribute.ProcessorType))
				{
					throw new InvalidOperationException();
				}
				object obj = Activator.CreateInstance(enumItemProcessorAttribute.ProcessorType);
				TEnum key = (TEnum)Enum.Parse(typeFromHandle, fieldInfo.Name);
				dictionary.Add(key, (TCreationRule)obj);
			}
		}
		return dictionary;
	}

	public static bool SetPropertyValue(object instance, string propertyName, object newValue)
	{
		if (instance is CustomActivity customActivity)
		{
			customActivity.PropertyCollection_Set(propertyName, newValue);
			return true;
		}
		PropertyInfo property = instance.GetType().GetProperty(propertyName);
		if (property != null)
		{
			property.SetValue(instance, newValue, null);
			return true;
		}
		return false;
	}

	public static object GetPropertyValue(object instance, string propertyName)
	{
		if (instance is CustomActivity customActivity)
		{
			return customActivity.PropertyCollection_Get(propertyName);
		}
		PropertyInfo property = instance.GetType().GetProperty(propertyName);
		if (property != null)
		{
			return property.GetValue(instance, null);
		}
		return null;
	}

	private static void CreateNameProperty(PropertyDescriptorCollection properties, ActivityPropertyInfo item, Type ownerInstanceType)
	{
		ActivityPropertyDescriptor activityPropertyDescriptor = new ActivityPropertyDescriptor(item.PropertyName, ownerInstanceType, item.PropertyType, item.Attributes.ToArray());
		if (activityPropertyDescriptor != null)
		{
			properties.Add(activityPropertyDescriptor);
		}
	}

	public static PropertyDescriptorCollection GetProperties(object ownerInstance, object componentInstance)
	{
		PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
		string key = componentInstance.GetType().ToString();
		Type type = ownerInstance.GetType();
		if (_cachedPropertyInfo.ContainsKey(key))
		{
			foreach (ActivityPropertyInfo item in _cachedPropertyInfo[key])
			{
				CreateNameProperty(propertyDescriptorCollection, item, type);
			}
		}
		else
		{
			Collection<ActivityPropertyInfo> collection = new Collection<ActivityPropertyInfo>();
			PropertyInfo[] properties = componentInstance.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				object[] customAttributes = propertyInfo.GetCustomAttributes(inherit: false);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					ActivityPropertyInfo activityPropertyInfo = new ActivityPropertyInfo();
					object[] array = customAttributes;
					foreach (object obj in array)
					{
						activityPropertyInfo.Attributes.Add(obj as Attribute);
					}
					activityPropertyInfo.PropertyName = propertyInfo.Name;
					activityPropertyInfo.PropertyType = propertyInfo.PropertyType;
					collection.Add(activityPropertyInfo);
					CreateNameProperty(propertyDescriptorCollection, activityPropertyInfo, type);
				}
			}
			_cachedPropertyInfo.Add(key, collection);
		}
		return propertyDescriptorCollection;
	}
}
