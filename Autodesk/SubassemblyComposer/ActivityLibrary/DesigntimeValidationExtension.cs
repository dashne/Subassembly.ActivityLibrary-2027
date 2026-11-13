using System.Activities;
using System.Activities.Expressions;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Autodesk.SubassemblyComposer.ActivityLibrary.Resources;
using Autodesk.SubassemblyComposer.WorkflowDesigntime;
using Autodesk.SubassemblyComposer.WorkflowEngine;
using Microsoft.VisualBasic.Activities;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal static class DesigntimeValidationExtension
{
	public static List<ValidationError> ValidateInDesigntime(this IDesigntimeValidate myInterface, int activityId)
	{
		List<ValidationError> list = new List<ValidationError>();
		ValidationError validationError = myInterface.IsInLoop(activityId);
		if (validationError != null)
		{
			list.Add(validationError);
		}
		else
		{
			list.AddRange(myInterface.SubValidateInDesigntime(activityId));
		}
		return list;
	}

	public static ValidationError IsInLoop(this IDesigntimeValidate myInterface, int activityId)
	{
		ValidationError result = null;
		if (ActivityDesigntime.Instance.CodePathTree == null && ActivityDesigntime.Instance.ActivitiesInLoop.Contains(activityId))
		{
			result = new ValidationError(Resource.Message_Validate_Loop, isWarning: false, "");
		}
		return result;
	}

	private static string ExtractStringFromExpression(Activity<string> expression)
	{
		if (expression == null)
		{
			return null;
		}
		if (expression is VisualBasicValue<string> visualBasicValue)
		{
			return visualBasicValue.ExpressionText;
		}
		if (expression is Literal<string> literal)
		{
			return literal.Value;
		}
		return expression.ToString();
	}

	public static bool PointArgumentValidate(this IDesigntimeValidate myInterface, InArgument<string> pointArgument, ObservableCollection<string> validPointNames)
	{
		string text = ExtractStringFromExpression(pointArgument?.Expression);
		if (text != null && !string.IsNullOrEmpty(text) && ((!GeometryNamePatterns.IsRegularPointName(text) && !GeometryNamePatterns.IsLoopEndPointName(text)) || !validPointNames.Contains(text)))
		{
			return false;
		}
		return true;
	}

	public static bool LinkArgumentValidate(this IDesigntimeValidate myInterface, InArgument<string> linkArgument, ObservableCollection<string> validNames)
	{
		string text = ExtractStringFromExpression(linkArgument?.Expression);
		if (text != null && !string.IsNullOrEmpty(text))
		{
			if (!GeometryNamePatterns.IsLinkName(text) || !validNames.Contains(text))
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public static Collection<string> ValidateGeometryNameInExpression<T>(this IDesigntimeValidate myInterface, InArgument<T> argument, Collection<string> validPointNames, Collection<string> validLinkNames)
	{
		Collection<string> collection = new Collection<string>();
		if (argument != null && argument.Expression != null && argument.Expression is VisualBasicValue<T> visualBasicValue)
		{
			Match match = Regex.Match(visualBasicValue.ExpressionText, GeometryNamePatterns.GeometryNameInExpressionPattern);
			while (match.Success)
			{
				string text = match.Value.TrimEnd('.');
				text = text.TrimStart('(');
				text = text.TrimStart('+');
				text = text.TrimStart('-');
				text = text.TrimStart('*');
				text = text.TrimStart('/');
				text = text.ToUpper();
				if (validPointNames != null && !validPointNames.Contains(text) && validLinkNames != null && !validLinkNames.Contains(text))
				{
					collection.Add(text);
				}
				match = match.NextMatch();
			}
		}
		return collection;
	}
}
