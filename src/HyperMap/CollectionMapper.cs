using System.Collections;

namespace HyperMap
{
	public class CollectionMapper : ITypeMapper
	{
		public dynamic MapInstance(object toMap, string parentIdUri = "")
		{
			var source = toMap as ICollection;
			var result = new ArrayList();
			
			foreach (var item in source)
			{
				var itemType = item.GetType();
				var itemTypeMap = TypeMappers.For(itemType);

				if (itemTypeMap != null)
				{
					result.Add(itemTypeMap.MapInstance(item, parentIdUri));
				}
				else
				{
					result.Add(item);
				}
			}
		
			return result;
		}
	}
}