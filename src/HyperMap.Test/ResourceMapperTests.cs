using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace HyperMap.Test
{
	[TestFixture]
	public class ResourceMapperTests
	{
		[TearDown]
		public void AfterEach()
		{
			TypeMappers.Clear();
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
		public void PropertiesMappedNormally()
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

		[Test]
		public void SingleValuedPropertyMappedAsResource()
		{
			ResourceMapper.CreateMap<SampleTypes.Feature>()
			              .WithResource(t => t.Hoozit);

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
		public void SinlgeValuedPropertyMappedAsLink()
		{
			ResourceMapper.CreateMap<SampleTypes.Feature>()
										.WithResourceLink(t => t.Hoozit, "feature/hoozit");

			var feature = new SampleTypes.Feature
			{
				Id = 10,
				Hoozit = new SampleTypes.Hoozit {Id = 5}
			};

			var mapped = ResourceMapper.Map(feature);

			Assert.That(mapped.Links, Has.Count.EqualTo(2));
			Assert.That(mapped.Links[1].Rel, Is.EqualTo("feature/hoozit"));
			Assert.That(mapped.Links[1].Href, Is.EqualTo("/feature/10/hoozit"));
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
				}.Take(1) // to make it an IEnumerable<> but not List<>
		};

		[Test]
		public void ChildCollectionAsResourceList()
		{
			ResourceMapper.CreateMap<SampleTypes.Widget>()
			              .WithResourceList(w => w.Features);

			var mapped = ResourceMapper.Map(widget);

			Assert.That(mapped.Features, Has.Count.EqualTo(1));
			Assert.That(mapped.Features[0].Links, Has.Count.EqualTo(1));
			Assert.That(mapped.Features[0].Links[0].Rel, Is.EqualTo("self"));
			Assert.That(mapped.Features[0].Links[0].Href, Is.EqualTo("/widget/123/features/555"));
		}

		[Test]
		public void ChildCollecitonAsLink()
		{
			ResourceMapper.CreateMap<SampleTypes.Widget>()
			              .WithResourceLink(w => w.Features, "/widget/features");

			var mapped = ResourceMapper.Map(widget);

			Assert.That(mapped.Links, Has.Count.EqualTo(2));
			Assert.That(mapped.Links[1].Rel, Is.EqualTo("/widget/features"));
			Assert.That(mapped.Links[1].Href, Is.EqualTo("/widget/123/features"));
		}

		[Test]
		public void UnmappedChildCollection()
		{
			ResourceMapper.CreateMap<SampleTypes.Widget>();

			var mapped = ResourceMapper.Map(widget);

			Assert.That(mapped.Features, Has.Count.EqualTo(1));
			Assert.That(mapped.Features[0].Id, Is.EqualTo(555));
			Assert.That(mapped.Features[0].Hoozit, Is.Not.Null);
		}

		[Test]
		public void MapAListOfResources()
		{
			ResourceMapper.CreateMap<SampleTypes.Thing>();

			var mapped = ResourceMapper.Map(new List<SampleTypes.Thing>
			{
				new SampleTypes.Thing{Id = 1},
				new SampleTypes.Thing{Id = 2},
				new SampleTypes.Thing{Id = 3},
			});

			Assert.That(mapped, Has.Count.EqualTo(3));
			Assert.That(mapped[0].Id, Is.EqualTo(1));
			Assert.That(mapped[1].Id, Is.EqualTo(2));
			Assert.That(mapped[2].Id, Is.EqualTo(3));
		}

		[Test]
		public void MapAnUnknownType()
		{
			TypeMappers.Clear();

			var mapped = ResourceMapper.Map(
				new SampleTypes.Thing
				{
					Id = 1, 
					Name = "Foo"
				});

			Assert.That(mapped.Id, Is.EqualTo(1));
			Assert.That(mapped.Name, Is.EqualTo("Foo"));
		}

		public class NoIdProperty
		{
			public string Name { get; set; }
		}

		[Test]
		public void TypeHasNoIdProperty()
		{
			ResourceMapper.CreateMap<NoIdProperty>();

			Assert.Throws<UnknownIdPropertyException>(() =>
			{
				ResourceMapper.Map(new NoIdProperty {Name = "foo"});
			});
		}

		// Sibling resource links (map hierarchies)
		// Property of unmapped type is mapped
		// Map a dynamic?
	}
}
