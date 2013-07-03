using System;

namespace HyperMap
{
	public class ResourceMapper
	{
		public static MapBuilder<T> CreateMap<T>(string uriSegment)
		{
			return new MapBuilder<T>(uriSegment);
		}

		public static MapBuilder<T> CreateMap<T>()
		{
			return new MapBuilder<T>();
		}

		public static dynamic Map(object toMap)
		{
			Type toMapType = toMap.GetType();
			var map = TypeMaps.For(toMapType);
			return map.MapInstance(toMap);
		}
	}
}