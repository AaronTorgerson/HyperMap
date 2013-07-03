namespace HyperMap
{
	public class LinkTemplate {
		private readonly string uriSegment;
		private readonly string linkRelation;

		public LinkTemplate(string uriSegment, string linkRelation)
		{
			this.uriSegment = uriSegment;
			this.linkRelation = linkRelation;
		}

		public Link CreateLink(string baseUri)
		{
			return new Link
			{
				Rel = linkRelation.ToLower(), 
				Href = (baseUri.TrimEnd('/') + "/" + uriSegment.TrimStart('/')).ToLower()
			};
		}
	}
}