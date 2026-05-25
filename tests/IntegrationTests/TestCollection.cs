using Xunit;

namespace PersonalFinance.IntegrationTests;

[CollectionDefinition("Shared collection")]
public class TestCollection : ICollectionFixture<IntegrationTestFactory>
{
}
