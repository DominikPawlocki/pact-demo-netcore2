using Xunit;

namespace Pact.Consumer.MVC.PactTests
{
    [CollectionDefinition(ConsumerContractsFixture.CollectionName)]
    public class PactCollection : ICollectionFixture<ConsumerContractsFixture>
    {
    }
}