using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace HyperMap
{
	public class TypeMap : ITypeMapper
	{
		private PropertyInfo idProperty;
		private readonly PropertyInfo[] properties;
		private readonly Dictionary<PropertyInfo, LinkTemplate> linkTemplates;

		public Type Type { get; private set; }
		public string UriSegment { get; private set; }

		public TypeMap(Type type, string uriSegment)
		{
			Type = type;
			UriSegment = uriSegment;
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

		public void SetId(PropertyInfo idPropertyInfo)
		{
			idProperty = idPropertyInfo;
		}

		public void AddLink(PropertyInfo member, string linkRelation)
		{
			linkTemplates.Add(member, new LinkTemplate(member.Name, linkRelation));
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
			ITypeMapper memberMap = TypeMappers.For(propertyType);

			return memberMap.MapInstance(propertyValue, idUri);
		}

		private string GetIdUri(object toMap, string parentIdUri)
		{
			return parentIdUri + "/" + UriSegment + "/" + idProperty.GetValue(toMap);
		}
	}

	public class UnknownTypeMapper : ITypeMapper
	{
		public dynamic MapInstance(object toMap, string parentIdUri = "")
		{
			return toMap;
		}
	}
}