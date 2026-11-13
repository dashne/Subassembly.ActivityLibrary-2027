using System;
using Autodesk.SubassemblyComposer.WorkflowEngine;

namespace Autodesk.SubassemblyComposer.ActivityLibrary;

internal class TypeHelper
{
	public static readonly Type ObjectType;

	public static readonly Type NullableType;

	public static bool AreTypesCompatible(Type sourceType, Type destinationType)
	{
		if (sourceType == typeof(Slope) || sourceType == typeof(Grade))
		{
			sourceType = typeof(double);
		}
		if (destinationType == typeof(Slope) || destinationType == typeof(Grade))
		{
			destinationType = typeof(double);
		}
		if ((object)sourceType != destinationType && !IsImplicitNumericConversion(sourceType, destinationType) && !IsImplicitReferenceConversion(sourceType, destinationType) && !IsImplicitBoxingConversion(sourceType, destinationType))
		{
			return IsImplicitNullableConversion(sourceType, destinationType);
		}
		return true;
	}

	private static bool IsImplicitNumericConversion(Type source, Type destination)
	{
		TypeCode typeCode = Type.GetTypeCode(source);
		TypeCode typeCode2 = Type.GetTypeCode(destination);
		switch (typeCode)
		{
		case TypeCode.Char:
			if ((uint)(typeCode2 - 8) <= 7u)
			{
				return true;
			}
			return false;
		case TypeCode.SByte:
			switch (typeCode2)
			{
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return true;
			default:
				return false;
			}
		case TypeCode.Byte:
			if ((uint)(typeCode2 - 7) <= 8u)
			{
				return true;
			}
			return false;
		case TypeCode.Int16:
			switch (typeCode2)
			{
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return true;
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				return false;
			default:
				return false;
			}
		case TypeCode.UInt16:
			if ((uint)(typeCode2 - 9) <= 6u)
			{
				return true;
			}
			return false;
		case TypeCode.Int32:
			switch (typeCode2)
			{
			case TypeCode.Int64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return true;
			case TypeCode.UInt64:
				return false;
			default:
				return false;
			}
		case TypeCode.UInt32:
			if ((uint)(typeCode2 - 10) <= 5u)
			{
				return true;
			}
			return false;
		case TypeCode.Int64:
		case TypeCode.UInt64:
			if ((uint)(typeCode2 - 13) <= 2u)
			{
				return true;
			}
			return false;
		case TypeCode.Single:
			return typeCode2 == TypeCode.Double;
		default:
			return false;
		}
	}

	private static bool IsImplicitReferenceConversion(Type sourceType, Type destinationType)
	{
		return destinationType.IsAssignableFrom(sourceType);
	}

	private static bool IsImplicitBoxingConversion(Type sourceType, Type destinationType)
	{
		if (!sourceType.IsValueType || (!(destinationType == ObjectType) && !(destinationType == typeof(ValueType))))
		{
			if (sourceType.IsEnum)
			{
				return destinationType == typeof(Enum);
			}
			return false;
		}
		return true;
	}

	private static bool IsImplicitNullableConversion(Type sourceType, Type destinationType)
	{
		if (!IsNullableType(destinationType))
		{
			return false;
		}
		destinationType = destinationType.GetGenericArguments()[0];
		if (IsNullableType(sourceType))
		{
			sourceType = sourceType.GetGenericArguments()[0];
		}
		return AreTypesCompatible(sourceType, destinationType);
	}

	private static bool IsNullableType(Type type)
	{
		if (type.IsGenericType)
		{
			return type.GetGenericTypeDefinition() == NullableType;
		}
		return false;
	}
}
