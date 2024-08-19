using PactNet;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace Pact.Consumer.MVC.PactTests.With.Pact.Provider.Api
{
    public class ConsumerContractsFixture
    {
        public IPactBuilderV4 PactBuilder;
        public PactConfig PactConf { get; private set; }
        public const string ConsumerName = "Pact.Consumer.Mvc";
        public const string ProviderName = "Pact.Provider.Api";

        public const string SuccessMessage = "Results returned successfully";
        public const string CollectionName = "With.Pact.Provider.Api";

        public PactConfig GetOrCreatePactConfig(ITestOutputHelper output)
        {
            if (PactConf == null)
            {
                PactConf = new PactConfig
                {
                    PactDir = "../../../pacts/",
                    DefaultJsonSettings = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    },
                    LogLevel = PactLogLevel.Debug
                };
            }
            return PactConf;
        }
    }
}