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
			TypeMappers.Register(typeMap);
		}

		public MapBuilder() 
			: this(typeof(T).Name.ToLower())
		{
		}

		private MapBuilder(PropertyInfo referringParentProperty)
			: this(referringParentProperty.Name.ToLower())
		{
		}

		public MapBuilder<T> Id<TMember>(Expression<Func<T, TMember>> property)
		{
			var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
			typeMap.SetId(propertyInfo);
			return this;
		}

		public MapBuilder<T> WithResourceLink<TMember>(Expression<Func<T, TMember>> sourceMember, string linkRelation)
		{
			var property = ((MemberExpression)sourceMember.Body).Member as PropertyInfo;
			typeMap.AddLink(property, linkRelation);
			return this;
		}

		public MapBuilder<TMember> WithResource<TMember>(Expression<Func<T, TMember>> property)
		{
			var referringProperty = ((MemberExpression)property.Body).Member as PropertyInfo;
			return new MapBuilder<TMember>(referringProperty);
		}

		public MapBuilder<TEnumerable> WithResourceList<TEnumerable>(Expression<Func<T, IEnumerable<TEnumerable>>> listProperty)
		{
			var referringProperty = ((MemberExpression)listProperty.Body).Member as PropertyInfo;
			return new MapBuilder<TEnumerable>(referringProperty);
		}
	}
}