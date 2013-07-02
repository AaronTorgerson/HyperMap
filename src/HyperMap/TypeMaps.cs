using System;
using System.Collections.Generic;

namespace HyperMap
{
	public class TypeMaps
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

		public static TypeMap For(Type type)
		{
			return Maps.ContainsKey(type) ? Maps[type] : null;
		}
	}
}