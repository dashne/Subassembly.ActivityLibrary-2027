using System;
using System.Linq.Expressions;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal static class ObjectExt
{
	public static string PropertyName<T, TProperty>(this T argument, Expression<Func<T, TProperty>> expression)
	{
		if (expression.Body is MemberExpression memberExpression)
		{
			return memberExpression.Member.Name;
		}
		throw new NotImplementedException();
	}
}
