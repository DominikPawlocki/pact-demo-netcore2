using Pact.Consumer.MVC.PactTests.With.Pact.Provider.Api;
using Xunit;

namespace Pact.Consumer.MVC.PactTests
{
    [CollectionDefinition(ConsumerContractsFixture.CollectionName)]
    public class PactCollection : ICollectionFixture<ConsumerContractsFixture>
    {
    }
}