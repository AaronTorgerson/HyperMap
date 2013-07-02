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
			HyperMap.CreateResourceMap<SampleTypes.Thing>("sample")
			        .Id(c => c.Id);

			var mapped = HyperMap.Map(new SampleTypes.Thing {Id = 2});

			Assert.That(mapped.Id, Is.EqualTo("/sample/2"));
		}

		[Test]
		public void IdFieldIsAssumedByName()
		{
			HyperMap.CreateResourceMap<SampleTypes.Thing>();

			var mapped = HyperMap.Map(new SampleTypes.Thing {Id = 2});

			Assert.That(mapped.Id, Is.EqualTo("/thing/2"));
		}

		[Test]
		public void UriSegmentCanBeDerivedFromType()
		{
			HyperMap.CreateResourceMap<SampleTypes.Thing>()
			        .Id(c => c.Id);

			var mapped = HyperMap.Map(new SampleTypes.Thing {Id = 2});

			Assert.That(mapped.Id, Is.EqualTo("/thing/2"));
		}

		[Test]
		public void OtherPropertiesGetMappedNormally()
		{
			HyperMap.CreateResourceMap<SampleTypes.Thing>()
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
			HyperMap.CreateResourceMap<SampleTypes.Widget>()
			        .Id(t => t.Id)
			        .ResourceUriFor(t => t.Features);

			var mapped = HyperMap.Map(new SampleTypes.Widget{Id = 3});

			Assert.That(mapped.Features, Is.EqualTo("/widget/3/features"));
		}

		[Test]
		public void ResourceMappingOfAMappedChildProperty()
		{
			HyperMap.CreateResourceMap<SampleTypes.Feature>()
			        .ChildResource(t => t.Hoozit);

			var feature = new SampleTypes.Feature
			{
				Id = 10,
				Hoozit = new SampleTypes.Hoozit {Id = 5}
			};

			var mapped = HyperMap.Map(feature);

			Assert.That(mapped.Hoozit.Id, Is.EqualTo("/feature/10/hoozit/5"));
		}

		SampleTypes.Widget widget = new SampleTypes.Widget
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

		[Test]
		public void MappingOfChildCollecitonAsListWithHref()
		{
			HyperMap.CreateResourceMap<SampleTypes.Widget>()
			        .ChildResource(w => w.Features);

			var mapped = HyperMap.Map(widget);

			Assert.That(mapped.Features.Href, Is.EqualTo("/widget/123/features"));
			Assert.That(mapped.Features.Items, Has.Count.EqualTo(1));
			Assert.That(mapped.Features.Items[0].Id, Is.EqualTo("/widget/123/features/555"));
		}

		[Test]
		public void MappingOfChildCollecitonAsResourceUri()
		{
			HyperMap.CreateResourceMap<SampleTypes.Widget>()
					  .ResourceUriFor(w => w.Features);

			var mapped = HyperMap.Map(widget);

			Assert.That(mapped.Features, Is.EqualTo("/widget/123/features"));
		}

		// If Map is called without having Configured that type.
	}
}
