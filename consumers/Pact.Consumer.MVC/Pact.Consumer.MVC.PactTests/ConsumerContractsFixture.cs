using Moq;
using Pact.Provider.Api;
using PactNet;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pact.Consumer.MVC.PactTests
{
    public class ConsumerContractsFixture
    {
        public IPactBuilderV4 PactBuilder;
        public PactConfig PactConf { get; private set; }
        public const string ConsumerName = "Pact.Consumer.Mvc";
        public const string ProviderName = "Pact.Provider.Api";

        public const string SuccessMessage = "Results returned successfully";
        public const string CollectionName = "With.Pact.Provider.Api";

        public PactConfig GetOrCreatePactConfig()
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

        internal void SetupHttpClientMock(Mock<IHttpClientFactory> mock, System.Uri someUri)
        {
            mock
                .Setup(f => f.CreateClient(Program.NhtsaPublicApiHttpClientName))
                .Returns(() => new HttpClient
                {
                    BaseAddress = someUri,
                    DefaultRequestHeaders =
                    {
                        Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") },
                    }
                });
        }
    }
}