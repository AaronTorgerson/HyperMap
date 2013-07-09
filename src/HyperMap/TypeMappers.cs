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
			else if (typeof (ICollection).IsAssignableFrom(type)) return new CollectionMapper();
			else return new UnknownTypeMapper();
		}
	}
}