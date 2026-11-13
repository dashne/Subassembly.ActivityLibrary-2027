using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Metadata;
using System.Activities.Validation;
using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

public abstract class ConditionBase<T> : NativeActivity<T>, IDesigntimeValidate, IRecordError
{
	[Browsable(false)]
	[RequiredArgument]
	public InArgument<int> ActivityId { get; set; }

	[RequiredArgument]
	[Browsable(false)]
	public InOutArgument<SubassemblyErrorCenter> SubassemblyErrorCenter { get; set; }

	[RequiredArgument]
	[Browsable(false)]
	public InOutArgument<SubassemblyRunMode> SubassemblyRunMode { get; set; }

	[Browsable(false)]
	public bool ShowErrors { get; set; }

	static ConditionBase()
	{
		AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
		attributeTableBuilder.AddCustomAttributes(typeof(ConditionBase<T>), typeof(ConditionBase<T>).GetProperty("Result", typeof(OutArgument<bool>)), BrowsableAttribute.No);
		attributeTableBuilder.AddCustomAttributes(typeof(ConditionBase<T>), typeof(ConditionBase<T>).GetProperty("ResultType"), BrowsableAttribute.No);
		MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
	}

	protected ConditionBase()
	{
		ShowErrors = true;
	}

	public void RecordErrorInfo(ActivityContext context, ErrorLevel level, string msg)
	{
		SubassemblyErrorCenter subassemblyErrorCenter = context.GetValue(SubassemblyErrorCenter) ?? throw new NullReferenceException("SubassemblyErrorCenter is not available");
		string mErrorHost = DisplayName;
		subassemblyErrorCenter.mErrorInfoList.Add(new SubassemblyErrorInfo(level, mErrorHost, msg, ShowErrors));
	}

	protected override void CacheMetadata(NativeActivityMetadata metadata)
	{
		base.CacheMetadata(metadata);
		if (ActivityDesigntime.Instance == null)
		{
			return;
		}
		int value = (ActivityId.Expression as Literal<int>).Value;
		foreach (ValidationError item in this.ValidateInDesigntime(value))
		{
			metadata.AddValidationError(item);
		}
	}

	public virtual List<ValidationError> SubValidateInDesigntime(int activityId)
	{
		return new List<ValidationError>();
	}

	private bool BeforeExecute(NativeActivityContext context)
	{
		if (ApiErrorCollection.IsEmpty)
		{
			return true;
		}
		foreach (string allError in ApiErrorCollection.GetAllErrors())
		{
			RecordErrorInfo(context, ErrorLevel.Error, allError);
		}
		ApiErrorCollection.Clear();
		return false;
	}

	protected override void Execute(NativeActivityContext context)
	{
		if (BeforeExecute(context))
		{
			SubExecute(context);
		}
	}

	protected abstract void SubExecute(NativeActivityContext context);
}
