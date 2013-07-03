using System.Collections.Generic;
using NUnit.Framework;

namespace HyperMap.Test
{
	[TestFixture]
	public class ResourceMapperTests
	{
		[TearDown]
		public void AfterEach()
		{
			TypeMaps.Clear();
		}

		[Test]
		public void MapWithIdUri()
		{
			ResourceMapper.CreateMap<SampleTypes.Thing>("sample")
			              .Id(c => c.Id);

			var mapped = ResourceMapper.Map(new SampleTypes.Thing {Id = 2});

			Assert.That(mapped.Id, Is.EqualTo(2));
			Assert.That(mapped.Links, Has.Count.EqualTo(1));
			Assert.That(mapped.Links[0].Rel, Is.EqualTo("self"));
			Assert.That(mapped.Links[0].Href, Is.EqualTo("/sample/2"));
		}

		[Test]
		public void IdFieldDefaultsToPropertyNamedId()
		{
			ResourceMapper.CreateMap<SampleTypes.Thing>();

			var mapped = ResourceMapper.Map(new SampleTypes.Thing {Id = 2});

			Assert.That(mapped.Id, Is.EqualTo(2));
			Assert.That(mapped.Links, Has.Count.EqualTo(1));
			Assert.That(mapped.Links[0].Rel, Is.EqualTo("self"));
			Assert.That(mapped.Links[0].Href, Is.EqualTo("/thing/2"));
		}

		[Test]
		public void OtherPropertiesGetMappedNormally()
		{
			ResourceMapper.CreateMap<SampleTypes.Thing>();

			var mapped = ResourceMapper.Map(
				new SampleTypes.Thing
				{
					Id = 2,
					Name = "Thing 2"
				});

			Assert.That(mapped.Id, Is.EqualTo(2));
			Assert.That(mapped.Name, Is.EqualTo("Thing 2"));
		}

		//[Test]
		//public void ReferenceChildPropertyAsResourceUri()
		//{
		//	ResourceMapper.CreateMap<SampleTypes.Widget>()
		//								.Id(t => t.Id)
		//								.WithResourceLink(t => t.Features);

		//	var mapped = ResourceMapper.Map(new SampleTypes.Widget{Id = 3});

		//	Assert.That(mapped.Features, Is.EqualTo("/widget/3/features"));
		//}

		[Test]
		public void MappedChildPropertyGetsOwnSelfLink()
		{
			ResourceMapper.CreateMap<SampleTypes.Feature>()
			              .WithChildResource(t => t.Hoozit);

			var feature = new SampleTypes.Feature
			{
				Id = 10,
				Hoozit = new SampleTypes.Hoozit {Id = 5}
			};

			var mapped = ResourceMapper.Map(feature);

			Assert.That(mapped.Hoozit.Id, Is.EqualTo(5));
			Assert.That(mapped.Hoozit.Links, Has.Count.EqualTo(1));
			Assert.That(mapped.Hoozit.Links[0].Rel, Is.EqualTo("self"));
			Assert.That(mapped.Hoozit.Links[0].Href, Is.EqualTo("/feature/10/hoozit/5"));
		}

		[Test]
		public void ChildPropertyAsResourceLink()
		{
			ResourceMapper.CreateMap<SampleTypes.Feature>()
										.WithChildResource(t => t.Hoozit);

			var feature = new SampleTypes.Feature
			{
				Id = 10,
				Hoozit = new SampleTypes.Hoozit { Id = 5 }
			};

			var mapped = ResourceMapper.Map(feature);

			Assert.That(mapped.Hoozit.Id, Is.EqualTo(5));
			Assert.That(mapped.Hoozit.Links, Has.Count.EqualTo(1));
			Assert.That(mapped.Hoozit.Links[0].Rel, Is.EqualTo("self"));
			Assert.That(mapped.Hoozit.Links[0].Href, Is.EqualTo("/feature/10/hoozit/5"));
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
		public void EachItemOfMappedChildCollectionGetsSelfUri()
		{
			ResourceMapper.CreateMap<SampleTypes.Widget>()
			              .WithChildResourceList(w => w.Features);

			var mapped = ResourceMapper.Map(widget);

			Assert.That(mapped.Features, Has.Count.EqualTo(1));
			Assert.That(mapped.Features[0].Links, Has.Count.EqualTo(1));
			Assert.That(mapped.Features[0].Links[0].Rel, Is.EqualTo("self"));
			Assert.That(mapped.Features[0].Links[0].Href, Is.EqualTo("/widget/123/features/555"));
		}

		[Test]
		public void MappingOfChildCollecitonAsResourceUri()
		{
			ResourceMapper.CreateMap<SampleTypes.Widget>()
			              .WithResourceLink(w => w.Features, "/widget/feature");

			var mapped = ResourceMapper.Map(widget);

			Assert.That(mapped.Links, Has.Count.EqualTo(2));
			Assert.That(mapped.Links[1].Rel, Is.EqualTo("/widget/feature"));
			Assert.That(mapped.Links[1].Href, Is.EqualTo("/widget/123/features"));
		}

		// If Map is called without having Configured that type.
		// Sibling resource links
		// Type has no Id property - what do?
	}
}
