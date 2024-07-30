using Pact.Provider.PactProviderTests.Setup;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;
using System.Text.Json;
using Xunit.Abstractions;

namespace Pact.Provider.PactProviderTests
{
    public class ProviderApiPactConsumerDrivenTests : IClassFixture<ProviderFixture>
    {
        private readonly ProviderFixture _fixture;
        private readonly ITestOutputHelper _output;
        public string PactBrokerUri = "http://localhost:9292";

        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };


        public ProviderApiPactConsumerDrivenTests(ProviderFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }


        [Fact]
        public void Verify()
        {
            var verifier = new PactVerifier("Pact.Provider.Api", new PactVerifierConfig
            {
                LogLevel = PactLogLevel.Information,
                Outputters = new List<IOutput>
            {
                new XunitOutput(_output)
            }
            });

            // This code will do verification of pact file from broker
            verifier
                .WithHttpEndpoint(_fixture.ProviderServerUri)
                .WithPactBrokerSource(new Uri(PactBrokerUri), options =>
                {
                    options.ConsumerVersionSelectors(new ConsumerVersionSelector { MainBranch = false, Latest = true })
                           .PublishResults("42", results =>
                           {
                               results.ProviderBranch("feature_dmnk_branch")
                                      .BuildUri(new Uri("https://azuredevops.microsoft.com/DominikBuild/123"));
                           });
                })
                .WithProviderStateUrl(new Uri(_fixture.ProviderServerUri, "/provider-states"))
                .Verify();
        }
    }
}