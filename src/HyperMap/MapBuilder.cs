using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace HyperMap
{
	public class MapBuilder<T>
	{
		private readonly TypeMap typeMap;

		public MapBuilder(string uriSegment)
		{
			typeMap = new TypeMap(typeof(T), uriSegment);
			TypeMaps.Register(typeMap);
		}

		public MapBuilder() 
			: this(typeof(T).Name.ToLower())
		{
		}

		private MapBuilder(PropertyInfo referringParentProperty)
			: this(referringParentProperty.Name.ToLower())
		{
		}

		public MapBuilder<T> Id<TMember>(Expression<Func<T, TMember>> sourceMember)
		{
			var property = ((MemberExpression)sourceMember.Body).Member as PropertyInfo;
			typeMap.SetId(property);
			return this;
		}

		public MapBuilder<T> ResourceUriFor<TMember>(Expression<Func<T, TMember>> sourceMember)
		{
			var property = ((MemberExpression)sourceMember.Body).Member as PropertyInfo;
			typeMap.AddChildResource(property);
			return this;
		}

		public MapBuilder<TMember> ChildResource<TMember>(Expression<Func<T, TMember>> sourceMember)
		{
			var referringProperty = ((MemberExpression)sourceMember.Body).Member as PropertyInfo;
			return new MapBuilder<TMember>(referringProperty);
		}

		public MapBuilder<TEnumerable> ChildResource<TEnumerable>(Expression<Func<T, IEnumerable<TEnumerable>>> sourceMember)
		{
			var referringProperty = ((MemberExpression)sourceMember.Body).Member as PropertyInfo;
			return new MapBuilder<TEnumerable>(referringProperty);
		}

		public MapBuilder<TEnumerable> ChildResource<TEnumerable>(Expression<Func<T, List<TEnumerable>>> sourceMember)
		{
			var referringProperty = ((MemberExpression)sourceMember.Body).Member as PropertyInfo;
			return new MapBuilder<TEnumerable>(referringProperty);
		}
	}
}