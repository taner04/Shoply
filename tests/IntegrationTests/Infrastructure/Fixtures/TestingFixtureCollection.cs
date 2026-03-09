namespace IntegrationTests.Infrastructure.Fixtures;

[CollectionDefinition(nameof(TestingFixtureCollection))]
public class TestingFixtureCollection : ICollectionFixture<TestingFixture>;