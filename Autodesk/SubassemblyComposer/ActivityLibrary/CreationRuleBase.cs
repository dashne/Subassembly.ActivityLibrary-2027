using System;
using System.Activities;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal abstract class CreationRuleBase : ICreationRule, ICustomAccessProperty
{
	private PropertyDescriptorCollection _ruleProperties;

	private Dictionary<string, RuntimeArgument> _runtimeArguments = new Dictionary<string, RuntimeArgument>();

	private Dictionary<string, object> _propertyCollection = new Dictionary<string, object>();

	public CreationRuleBase()
	{
	}

	public void Init()
	{
		_ruleProperties = CreationRuleHelper.GetProperties(this, this);
		_runtimeArguments = CreateRuntimeArgument(_ruleProperties);
	}

	private Dictionary<string, RuntimeArgument> CreateRuntimeArgument(PropertyDescriptorCollection ruleProperties)
	{
		Dictionary<string, RuntimeArgument> dictionary = new Dictionary<string, RuntimeArgument>();
		foreach (PropertyDescriptor ruleProperty in ruleProperties)
		{
			if (!dictionary.ContainsKey(ruleProperty.Name))
			{
				if (!(ruleProperty.GetValue(this) is Argument argument))
				{
					throw new ArgumentNullException(ruleProperty.Name, "Must be initialized in constructor.");
				}
				bool isRequired = ruleProperty.Attributes.Contains(new RequiredArgumentAttribute());
				if (argument != null)
				{
					RuntimeArgument value = new RuntimeArgument(ruleProperty.Name, argument.ArgumentType, argument.Direction, isRequired);
					dictionary.Add(ruleProperty.Name, value);
				}
			}
		}
		return dictionary;
	}

	public PropertyDescriptorCollection GetProperties(object ownerInstance)
	{
		return CreationRuleHelper.GetProperties(ownerInstance, this);
	}

	public object GetPropertyValue(string propertyName)
	{
		if (!_propertyCollection.ContainsKey(propertyName))
		{
			return null;
		}
		return _propertyCollection[propertyName];
	}

	public void SetPropertyValue(string propertyName, object propertyValue)
	{
		if (!_propertyCollection.ContainsKey(propertyName))
		{
			_propertyCollection.Add(propertyName, propertyValue);
		}
		else
		{
			_propertyCollection[propertyName] = propertyValue;
		}
	}

	public void CacheMetadata(CodeActivityMetadata metadata, Dictionary<string, Argument> arguments)
	{
		if (arguments == null)
		{
			throw new ArgumentNullException();
		}
		if (_ruleProperties == null)
		{
			throw new ArgumentNullException("_ruleProperties", "The function of Init() hasn't invoke in your derived class constructor.");
		}
		foreach (PropertyDescriptor ruleProperty in _ruleProperties)
		{
			if (ruleProperty.GetValue(this) is Argument value)
			{
				if (!arguments.ContainsKey(ruleProperty.Name))
				{
					arguments.Add(ruleProperty.Name, value);
				}
				else
				{
					ruleProperty.SetValue(this, arguments[ruleProperty.Name]);
				}
			}
			if (arguments.ContainsKey(ruleProperty.Name) && _runtimeArguments.ContainsKey(ruleProperty.Name))
			{
				metadata.Bind(arguments[ruleProperty.Name], _runtimeArguments[ruleProperty.Name]);
				metadata.AddArgument(_runtimeArguments[ruleProperty.Name]);
			}
		}
	}

	public void SyncProperty(Dictionary<string, Argument> arguments)
	{
		if (arguments == null)
		{
			throw new ArgumentNullException();
		}
		foreach (KeyValuePair<string, Argument> argument in arguments)
		{
			_ruleProperties[argument.Key]?.SetValue(this, argument.Value);
		}
	}

	public void RefreshArguments(Dictionary<string, Argument> arguments)
	{
		if (arguments == null)
		{
			throw new ArgumentNullException();
		}
		arguments.Clear();
		foreach (PropertyDescriptor ruleProperty in _ruleProperties)
		{
			if (ruleProperty.GetValue(this) is Argument value && !arguments.ContainsKey(ruleProperty.Name))
			{
				arguments.Add(ruleProperty.Name, value);
			}
		}
	}

	public virtual List<ValidationError> DesigntimeValidation(IDesigntimeValidate validate, ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames)
	{
		return new List<ValidationError>();
	}

	protected void geometryNamesUppCase(ObservableCollection<string> validPointNames, ObservableCollection<string> validLinkNames, ref Collection<string> outputPointNames, ref Collection<string> outputLinkNames)
	{
		outputPointNames = new Collection<string>();
		foreach (string validPointName in validPointNames)
		{
			outputPointNames.Add(validPointName.ToUpper());
		}
		outputLinkNames = new Collection<string>();
		foreach (string validLinkName in validLinkNames)
		{
			outputLinkNames.Add(validLinkName.ToUpper());
		}
	}

	protected void validateGeometryNameInExpression(IDesigntimeValidate validate, Collection<string> validPointNames, Collection<string> validLinkNames, InArgument<double> argumentValue, string argumentDisplayName, List<ValidationError> result)
	{
		Collection<string> collection = validate.ValidateGeometryNameInExpression(argumentValue, validPointNames, validLinkNames);
		string text = string.Empty;
		foreach (string item in collection)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += ",";
			}
			text += item;
		}
		if (!string.IsNullOrEmpty(text))
		{
			string message = string.Format(Resource.Message_Validate_GeometryName, argumentDisplayName, text);
			result.Add(new ValidationError(message, isWarning: false, argumentDisplayName));
		}
	}
}
