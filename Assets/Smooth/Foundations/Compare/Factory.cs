using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Smooth.Algebraics;
using Smooth.Collections;
using Smooth.Comparisons;
using Smooth.Compare.Comparers;

namespace Smooth.Compare {
	public static class Factory {
		private const int hashCodeSeed = 17;
		private const int hashCodeStepMultiplier = 29;

		#region Comparer

		/// <summary>
		/// Creates a sort order comparer for type T, or returns None if no comparer can be created.
		/// 
		/// This method will create comparers for the following types:
		/// 
		/// System.Collections.KeyValuePair<,>
		/// </summary>
		public static Option<IComparer<T>> Comparer<T>() {
			var type = typeof(T);

			if (!type.IsValueType) {
				return Option<IComparer<T>>.None;
			} else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)) {
				return new Option<IComparer<T>>(KeyValuePairComparer<T>(type));
			} else {
				return Option<IComparer<T>>.None;
			}
		}
		
		#endregion

		#region EqualityComparer

		/// <summary>
		/// Creates an equality comparer for type T, or returns None if no comparer can be created.
		/// 
		/// This method will create comparers for the following types:
		/// 
		/// Enumerations
		/// System.Collections.KeyValuePair<,>
		/// Value types T with a public T.Equals(T) method or ==(T,T) operator
		/// </summary>
		public static Option<IEqualityComparer<T>> EqualityComparer<T>() {
			var type = typeof(T);

			if (!type.IsValueType) {
				return Option<IEqualityComparer<T>>.None;
			} else if (type.IsEnum) {
				return new Option<IEqualityComparer<T>>(EnumEqualityComparer<T>(type));
			} else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)) {
				return new Option<IEqualityComparer<T>>(KeyValuePairEqualityComparer<T>(type));
			} else {
				var l = Expression.Parameter(type, "l");
				var r = Expression.Parameter(type, "r");

				var expression = EqualsExpression(l, r);

				return expression.isSome ?
					new Option<IEqualityComparer<T>>(new FuncEqualityComparer<T>(Expression.Lambda<Func<T, T, bool>>(expression.value, l, r).Compile())) :
						Option<IEqualityComparer<T>>.None; 
			}
		}

		#endregion

		#region Expressions

		private static Expression HashCodeSeed() {
			return Expression.Constant(hashCodeSeed, typeof(int));
		}
		
		private static Expression HashCodeStepMultiplier() {
			return Expression.Constant(hashCodeStepMultiplier, typeof(int));
		}

		/// <summary>
		/// Returns an option containing an expression that compares l and r for equality without casting, or None if no such comparison can be found.
		/// </summary>
		public static Option<Expression> EqualsExpression(Expression l, Expression r) {
			try {
				var expression = Expression.Equal(l, r);
				var method = expression.Method;
				if (method != null) {
					var ps = method.GetParameters();
					if (ps[0].ParameterType == l.Type && ps[1].ParameterType == r.Type) {
						return new Option<Expression>(expression);
					}
				}
			} catch (InvalidOperationException) {
			} catch (Exception e) {
				Debug.LogError(e);
			}
			
			try {
				var mi = l.Type.GetMethod(
					"Equals",
					BindingFlags.Public | BindingFlags.Instance,
					null,
					new Type[] { r.Type },
					null
				);
				
				if (mi != null && mi.GetParameters()[0].ParameterType == r.Type) {
					return new Option<Expression>(Expression.Call(l, mi, r));
				}
			} catch (Exception e) {
				Debug.LogError(e);
			}
			
			return Option<Expression>.None;
		}

		/// <summary>
		/// Returns an expression that evaluates to Smooth.Collections.Comparer<T>.Default for the runtime type of Type.
		/// The out compare parameter will contain a MethodInfo for the returned expression's Compare(T, T) method.
		/// </summary>
		public static Expression ExistingComparer(Type type, out MethodInfo compare) {
			var pi = typeof(Smooth.Collections.Comparer<>).MakeGenericType(type).GetProperty(
				"Default",
				BindingFlags.Public | BindingFlags.Static,
				null,
				typeof(IComparer<>).MakeGenericType(type),
				Type.EmptyTypes,
				null);
			
			var c = Expression.Property(null, pi);
			
			compare = c.Type.GetMethod("Compare", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type, type }, null);
			
			return c;
		}
		
		/// <summary>
		/// Returns an expression that evaluates to Smooth.Collections.EqualityComparer<T>.Default for the runtime type of Type.
		/// The out equals parameter will contain a MethodInfo for the returned expression's Equals(T, T) method.
		/// The out hashCode parameter will contain a MethodInfo for the returned expression's GetHashCode(T) method.
		/// </summary>
		public static Expression ExistingEqualityComparer(Type type, out MethodInfo equals, out MethodInfo hashCode) {
			var pi = typeof(Smooth.Collections.EqualityComparer<>).MakeGenericType(type).GetProperty(
				"Default",
				BindingFlags.Public | BindingFlags.Static,
				null,
				typeof(IEqualityComparer<>).MakeGenericType(type),
				Type.EmptyTypes,
				null);
			
			var ec = Expression.Property(null, pi);
			
			equals = ec.Type.GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type, type }, null);
			hashCode = ec.Type.GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type }, null);
			
			return ec;
		}
		
		#endregion

		#region Comparers for specific types
		
		private static IComparer<T> KeyValuePairComparer<T>(Type type) {
			var l = Expression.Parameter(type, "l");
			var r = Expression.Parameter(type, "r");
			
			var keyL = Expression.Property(l, "Key");
			var keyR = Expression.Property(r, "Key");
			
			var valueL = Expression.Property(l, "Value");
			var valueR = Expression.Property(r, "Value");
			
			MethodInfo miKeysCompare;
			var keyComparer = ExistingComparer(keyL.Type, out miKeysCompare);
			
			MethodInfo miValuesCompare;
			var valueComparer = ExistingComparer(valueL.Type, out miValuesCompare);

			var keysCompared = Expression.Lambda<Comparison<T>>(Expression.Call(keyComparer, miKeysCompare, keyL, keyR), l, r).Compile();
			var valuesCompared = Expression.Lambda<Comparison<T>>(Expression.Call(valueComparer, miValuesCompare, valueL, valueR), l, r).Compile();

			return new FuncComparer<T>((lhs, rhs) => { var c = keysCompared(lhs, rhs); return c == 0 ? valuesCompared(lhs, rhs) : c; });
		}
		
		#endregion

		#region EqualityComparers for specific types

		private static IEqualityComparer<T> EnumEqualityComparer<T>(Type type) {
			switch(Type.GetTypeCode(type)) {
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return new Blittable64EqualityComparer<T>();
			default:
				return new Blittable32EqualityComparer<T>();
			}
		}
		
		private static IEqualityComparer<T> KeyValuePairEqualityComparer<T>(Type type) {
			var l = Expression.Parameter(type, "l");
			var r = Expression.Parameter(type, "r");

			var keyL = Expression.Property(l, "Key");
			var keyR = Expression.Property(r, "Key");
			
			var valueL = Expression.Property(l, "Value");
			var valueR = Expression.Property(r, "Value");

			MethodInfo miKeysEqual;
			MethodInfo miKeyHashCode;
			var keyComparer = ExistingEqualityComparer(keyL.Type, out miKeysEqual, out miKeyHashCode);

			MethodInfo miValuesEqual;
			MethodInfo miValueHashCode;
			var valueComparer = ExistingEqualityComparer(valueL.Type, out miValuesEqual, out miValueHashCode);

			var keysEqual = Expression.Call(keyComparer, miKeysEqual, keyL, keyR);
			var valuesEqual = Expression.Call(valueComparer, miValuesEqual, valueL, valueR);

			var equals = Expression.And(keysEqual, valuesEqual);

			var hashCodeKey = Expression.Call(keyComparer, miKeyHashCode, keyL);
			var hashCodeValue = Expression.Call(valueComparer, miValueHashCode, valueL);

			var hashCode = HashCodeSeed();
			var hashCodeStepMultiplier = HashCodeStepMultiplier();

			hashCode = BinaryExpression.Add(hashCodeKey, BinaryExpression.Multiply(hashCode, hashCodeStepMultiplier));
			hashCode = BinaryExpression.Add(hashCodeValue, BinaryExpression.Multiply(hashCode, hashCodeStepMultiplier));

			return new FuncEqualityComparer<T>(
				Expression.Lambda<Func<T, T, bool>>(equals, l, r).Compile(),
				Expression.Lambda<Func<T, int>>(hashCode, l).Compile());
		}

		#endregion
	}
}
