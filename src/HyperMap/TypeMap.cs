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
		private readonly List<MemberInfo> propertiesToOmit;
		private readonly PropertyInfo[] properties;
		private readonly Dictionary<PropertyInfo, LinkTemplate> linkTemplates;
		private PropertyInfo idProperty;

		public Type Type { get; private set; }
		public string UriSegment { get; private set; }

		public TypeMap(Type type, string uriSegment)
		{
			Type = type;
			UriSegment = uriSegment;
			propertiesToOmit = new List<MemberInfo>();
			properties = Type.GetProperties();
			AssumeIdBasedOnName();
			linkTemplates = new Dictionary<PropertyInfo, LinkTemplate>();
		}

		private void AssumeIdBasedOnName()
		{
			idProperty = properties.FirstOrDefault(p =>
			{
				return p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase);
			});
		}

		public void SetId(PropertyInfo idProperty)
		{
			this.idProperty = idProperty;
		}

		public void AddLink(PropertyInfo member, string linkRelation)
		{
			linkTemplates.Add(member, new LinkTemplate(member.Name, linkRelation));
			propertiesToOmit.Add(member);
		}

		public dynamic MapInstance(object toMap, string parentIdUri = "")
		{
			IDictionary<string, object> result = new ExpandoObject();
			var links = new Links();
			string idUri = GetIdUri(toMap, parentIdUri);
			links.Add(new Link { Rel = "self", Href = idUri });

			foreach (var property in properties)
			{
				Type propertyType = property.PropertyType;
				object propertyValue = property.GetValue(toMap);

				if (linkTemplates.ContainsKey(property))
				{
					links.Add(linkTemplates[property].CreateLink(idUri));
				}
				else
				{
					result[property.Name] = MapProperty(propertyType, propertyValue, idUri);
				}
			}

			if (links.Any())
				result["Links"] = links;

			return result;
		}

		private static object MapProperty(Type propertyType, object propertyValue, string idUri)
		{
			return IsGenericListType(propertyType)
				       ? MapList(propertyType, propertyValue, idUri)
				       : MapSingleValue(propertyType, propertyValue, idUri);
		}

		private static object MapSingleValue(Type propertyType, object memberValue, string idUri)
		{
			TypeMap memberMap = TypeMaps.For(propertyType);

			return memberMap != null
				       ? memberMap.MapInstance(memberValue, idUri)
				       : memberValue;
		}

		private static object MapList(Type propertyType, object memberValue, string idUri)
		{
			object result;
			var enumerableType = propertyType.GetGenericArguments().First();
			var enumerableMap = TypeMaps.For(enumerableType);
			
			if (enumerableMap != null)
			{
				var list = new ArrayList();
				foreach (var item in (IEnumerable)memberValue)
				{
					list.Add(enumerableMap.MapInstance(item, idUri));
				}
				result = list;
			}
			else
			{
				result = memberValue;
			}
			return result;
		}

		private static bool IsGenericListType(Type propertyType)
		{
			return propertyType.IsGenericType && (typeof(IEnumerable)).IsAssignableFrom(propertyType);
		}

		private string GetIdUri(object toMap, string parentIdUri)
		{
			return parentIdUri + "/" + UriSegment + "/" + idProperty.GetValue(toMap);
		}
	}
}