﻿using System.Collections.Generic;
using NUnit.Framework;

namespace HyperMap.Test.Acceptance
{
	[TestFixture]
	public class RealisticScenarioTests
	{
		private static readonly SampleTypes.Portfolio Portfolio = new SampleTypes.Portfolio
		{
			PortfolioId = 123,
			Cash = 10000,
			Positions = new List<SampleTypes.Position>
			{
				new SampleTypes.Position
				{
					PositionId = 1,
					Ticker = "AAPL",
					UnitPrice = 430,
					Units = 50
				}
			}
		};

		[TearDown]
		public void AfterEach()
		{
			TypeMaps.Clear();
		}

		[Test]
		public void PortfolioScenario1()
		{
			HyperMap.CreateResourceMap<SampleTypes.Portfolio>()
			        .Id(p => p.PortfolioId)
			        .ResourceUriFor(p => p.Positions);

			var mapped = HyperMap.Map(Portfolio);

			Assert.That(mapped.PortfolioId, Is.EqualTo("/portfolio/123"));
			Assert.That(mapped.Cash, Is.EqualTo(10000));
			Assert.That(mapped.Positions, Is.EqualTo("/portfolio/123/positions"));
		}

		[Test]
		public void PortfolioScenario2()
		{
			HyperMap.CreateResourceMap<SampleTypes.Portfolio>()
							.Id(p => p.PortfolioId)
							.ChildResource(p => p.Positions)
							.Id(p => p.PositionId);

			var mapped = HyperMap.Map(Portfolio);

			Assert.That(mapped.PortfolioId, Is.EqualTo("/portfolio/123"));
			Assert.That(mapped.Cash, Is.EqualTo(10000));
			Assert.That(mapped.Positions.Items, Has.Count.EqualTo(1));
			Assert.That(mapped.Positions.Items[0].PositionId, Is.EqualTo("/portfolio/123/positions/1"));
			Assert.That(mapped.Positions.Items[0].Ticker, Is.EqualTo("AAPL"));
			Assert.That(mapped.Positions.Items[0].Units, Is.EqualTo(50));
			Assert.That(mapped.Positions.Items[0].UnitPrice, Is.EqualTo(430));
		}
	}
}