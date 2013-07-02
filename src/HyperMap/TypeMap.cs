using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace HyperMap
{
	public class TypeMap
	{
		private readonly List<MemberInfo> overriddenProperties;
		private readonly PropertyInfo[] properties;
		private PropertyInfo idProperty;

		public Type Type { get; private set; }
		public string UriSegment { get; private set; }

		public TypeMap(Type type, string uriSegment)
		{
			Type = type;
			UriSegment = uriSegment;
			overriddenProperties = new List<MemberInfo>();
			properties = Type.GetProperties();
			AssumeIdBasedOnName();
		}

		private void AssumeIdBasedOnName()
		{
			idProperty = properties.FirstOrDefault(p =>
			{
				return p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase);
			});
		}

		public bool IsChildResource(MemberInfo member)
		{
			return overriddenProperties.Contains(member);
		}

		public void SetId(PropertyInfo idProperty)
		{
			this.idProperty = idProperty;
		}

		public void AddChildResource(MemberInfo member)
		{
			overriddenProperties.Add(member);
		}

		public dynamic MapInstance(object toMap, string parentIdUri = "")
		{
			IDictionary<string, object> result = new ExpandoObject();
			string idUri = IdUri(toMap, parentIdUri);

			foreach (var getter in properties)
			{
				Type propertyType = getter.PropertyType;

				if (IsChildResource(getter))
				{
					result[getter.Name] = idUri + "/" + getter.Name.ToLower();
				}
				else if (getter == idProperty)
				{
					result[getter.Name] = idUri;
				}
				else if (propertyType.IsGenericType && (typeof(IEnumerable)).IsAssignableFrom(propertyType))
				{
					var enumerableType = propertyType.GetGenericArguments().First();
					var memberMap = TypeMaps.For(enumerableType);
					if (memberMap != null)
					{
						var list = new ChildCollectionResource
							{
								Href = idUri + "/" + getter.Name.ToLower()
							};

						foreach (var item in (IEnumerable)getter.GetValue(toMap))
						{
							list.Add(memberMap.MapInstance(item, idUri));
						}
						result[getter.Name] = list;
					}
					else
					{
						result[getter.Name] = getter.GetValue(toMap);
					}
				}
				else
				{
					TypeMap memberMap = TypeMaps.For(propertyType);
					object memberValue = getter.GetValue(toMap);

					if (memberMap != null)
					{
						result[getter.Name] = memberMap.MapInstance(memberValue, idUri);
					}
					else
					{
						result[getter.Name] = memberValue;
					}
				}
			}

			return result;
		}

		private string IdUri(object toMap, string parentIdUri)
		{
			return parentIdUri + "/" + UriSegment + "/" + idProperty.GetValue(toMap);
		}

		public class ChildCollectionResource
		{
			public ChildCollectionResource()
			{
				Items = new ArrayList();
			}

			public string Href { get; set; }
			public ArrayList Items { get; set; }

			public void Add(object item)
			{
				Items.Add(item);
			}
		}
	}
}