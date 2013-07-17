using System.Collections.Generic;

namespace HyperMap.Test
{
	public class SampleTypes
	{
		public class Widget
		{
			public int Id { get; set; }

			public Widget()
			{
				Features = new List<Feature>();
			}

			public IEnumerable<Feature> Features { get; set; }
		}

		public class Feature
		{
			public int Id { get; set; }
			public Hoozit Hoozit { get; set; }
		}

		public class Hoozit
		{
			public int Id { get; set; }
		}

		public class Thing
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		public class Portfolio
		{
			public int PortfolioId { get; set; }
			public decimal Cash { get; set; }
			public List<Position> Positions { get; set; }
		}

		public class Position
		{
			public int PositionId { get; set; }
			public decimal Units { get; set; }
			public decimal UnitPrice { get; set; }
			public string Ticker { get; set; }
		}
	}
}