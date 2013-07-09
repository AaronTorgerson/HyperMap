namespace HyperMap
{
	public interface ITypeMapper {
		dynamic MapInstance(object toMap, string parentIdUri = "");
	}
}