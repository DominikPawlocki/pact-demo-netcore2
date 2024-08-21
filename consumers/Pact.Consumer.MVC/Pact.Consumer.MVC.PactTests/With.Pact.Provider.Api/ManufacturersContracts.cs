using FluentAssertions;
using Moq;
using Pact.Consumer.MVC.Models;
using Pact.Consumer.MVC.Services;
using Pact.Provider.Api;
using PactNet;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Pact.Consumer.MVC.PactTests.With.Pact.Provider.Api
{
    [Collection(ConsumerContractsFixture.CollectionName)]
    public class ManufacturersContracts
    {
        private readonly ConsumerContractsFixture _fixture;
        Mock<IHttpClientFactory> _mockFactory = new();
        private readonly NhtsaManufacturersResponce _expected = new NhtsaManufacturersResponce
        {
            Count = 2,
            Message = ConsumerContractsFixture.SuccessMessage,
            SearchCriteria = null,
            Results = new[] {
                            new ManufacturerResult {
                                Country = "United States (USA)",
                                Mfr_CommonName = "Chrysler",
                                Mfr_ID = 994,
                                Mfr_Name = "FCA US LLC",
                                VehicleTypes = new [] {
                                    new VehicleType {
                                        IsPrimary = true,
                                        Name = "Multipurpose Passenger Vehicle (MPV)"
                                    }
}
                            },
                            new ManufacturerResult
                            {
                                Country = "Japan",
                                Mfr_CommonName = "Mazda",
                                Mfr_ID = 1041,
                                Mfr_Name = "Mazda Motor Corporation",
                                VehicleTypes = new[] {
                                    new VehicleType {
                                        IsPrimary = true,
                                        Name = "Multipurpose Passenger Vehicle (MPV)"
                                    }
                                }
                            }
                        }
        };

        public ManufacturersContracts(ConsumerContractsFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _fixture.GetOrCreatePactConfig();
            _fixture.PactBuilder =
                PactNet.Pact.V4(ConsumerContractsFixture.ConsumerName, ConsumerContractsFixture.ProviderName, _fixture.PactConf)
                .WithHttpInteractions();
        }

        [Fact]
        public async Task When_Getting_Random_20_Manufacturers_Returns_Data()
        {
            _fixture.PactBuilder
                        .UponReceiving("A GET request to retrieve provider/api/cars/manufacturers/random20")
                        .Given("an order with ID {id} exists", new Dictionary<string, string> { ["id"] = "1" })
                        .WithRequest(HttpMethod.Get, "/provider/api/cars/manufacturers/random20")
                        .WithHeader("Accept", "application/json")
                    .WillRespond()
                        .WithStatus(HttpStatusCode.OK)
                        .WithJsonBody(_expected);

            await _fixture.PactBuilder.VerifyAsync(async ctx =>
            {
                _mockFactory
                    .Setup(f => f.CreateClient(Program.NhtsaPublicApiHttpClientName))
                    .Returns(() => new HttpClient
                    {
                        BaseAddress = ctx.MockServerUri,
                        DefaultRequestHeaders =
                        {
                            Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") },
                        }
                    });

                var consumer = new CarService(_mockFactory.Object);
                var result = await consumer.GetManufacturers();

                result.Should().BeEquivalentTo(_expected); //--> NOT A PACT ! Unit Test !
            });
        }
    }
}