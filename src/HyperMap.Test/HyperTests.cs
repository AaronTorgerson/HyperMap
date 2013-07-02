using System.Collections.Generic;
using NUnit.Framework;

namespace HyperMap.Test
{
	[TestFixture]
	public class HyperTests
	{
		[TearDown]
		public void AfterEach()
		{
			TypeMaps.Clear();
		}

		[Test]
		public void MapWithIdUri()
		{
			HyperMap.Configure<SampleTypes.Thing>("sample")
			        .Id(c => c.Id);

			var mapped = HyperMap.Map(new SampleTypes.Thing {Id = 2});

			Assert.That(mapped.Id, Is.EqualTo("/sample/2"));
		}

		[Test]
		public void IdFieldIsAssumedByName()
		{
			HyperMap.Configure<SampleTypes.Thing>();

			var mapped = HyperMap.Map(new SampleTypes.Thing {Id = 2});

			Assert.That(mapped.Id, Is.EqualTo("/thing/2"));
		}

		[Test]
		public void UriSegmentCanBeDerivedFromType()
		{
			HyperMap.Configure<SampleTypes.Thing>()
			        .Id(c => c.Id);

			var mapped = HyperMap.Map(new SampleTypes.Thing {Id = 2});

			Assert.That(mapped.Id, Is.EqualTo("/thing/2"));
		}

		[Test]
		public void OtherPropertiesGetMappedNormally()
		{
			HyperMap.Configure<SampleTypes.Thing>()
			        .Id(c => c.Id);

			var mapped = HyperMap.Map(
				new SampleTypes.Thing
				{
					Id = 2,
					Name = "Thing 2"
				});

			Assert.That(mapped.Name, Is.EqualTo("Thing 2"));
		}

		[Test]
		public void ReferenceChildPropertyAsResourceUri()
		{
			HyperMap.Configure<SampleTypes.Widget>()
							.Id(t => t.Id)
			        .ChildResource(t => t.Features);

			var mapped = HyperMap.Map(new SampleTypes.Widget{Id = 3});

			Assert.That(mapped.Features, Is.EqualTo("/widget/3/features"));
		}

		[Test]
		public void ResourceMappingOfAMappedChildProperty()
		{
			HyperMap.Configure<SampleTypes.Feature>()
							.Configure(t => t.Hoozit);

			var feature = new SampleTypes.Feature
			{
				Id = 10,
				Hoozit = new SampleTypes.Hoozit {Id = 5}
			};

			var mapped = HyperMap.Map(feature);

			Assert.That(mapped.Hoozit.Id, Is.EqualTo("/feature/10/hoozit/5"));
		}

		[Test]
		public void MappingOfChildColleciton()
		{
			HyperMap.Configure<SampleTypes.Widget>()
			        .ConfigureCollection(w => w.Features);

			var widget = new SampleTypes.Widget
			{
				Id = 123,
				Features = new List<SampleTypes.Feature>
				{
					new SampleTypes.Feature
					{
						Hoozit = new SampleTypes.Hoozit(),
						Id = 555
					}
				}
			};

			var mapped = HyperMap.Map(widget);

			Assert.That(mapped.Features, Has.Count.EqualTo(1));
			Assert.That(mapped.Features[0].Id, Is.EqualTo("/widget/123/features/555"));
		}

		// ResourceMappingOfAMappedChildCollection
		// If ChildResource is called, Id must have been specified.
		// If Map is called without having Configured that type.
		// Attempt to map a type without 
	}
}
