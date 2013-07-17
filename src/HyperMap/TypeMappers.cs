using System;
using System.Collections;
using System.Collections.Generic;

namespace HyperMap
{
	public class TypeMappers
	{
		private static readonly Dictionary<Type, TypeMap> Maps
			= new Dictionary<Type, TypeMap>();

		public static void Register(TypeMap typeMap)
		{
			Maps.Add(typeMap.Type, typeMap);
		}

		public static void Clear()
		{
			Maps.Clear();
		}

		public static ITypeMapper For(Type type)
		{
			if (Maps.ContainsKey(type)) return Maps[type];
			else if (IsListType(type)) return new CollectionMapper();
			else return new UnknownTypeMapper();
		}

		private static bool IsListType(Type type)
		{
			return (typeof (IEnumerable).IsAssignableFrom(type)
			        && type.IsGenericType)
			       || typeof (ICollection).IsAssignableFrom(type);
		}
	}
}