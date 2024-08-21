using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Pact.Consumer.MVC.Models;
using Pact.Consumer.MVC.Services;
using PactNet;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Pact.Consumer.MVC.PactTests.With.Pact.Provider.Api
{
    [Collection(ConsumerContractsFixture.CollectionName)]
    public class CarModelsContracts
    {
        Mock<IHttpClientFactory> _mockFactory = new();
        private readonly ConsumerContractsFixture _fixture;

        public CarModelsContracts(ConsumerContractsFixture fixture)
        {
            _fixture = fixture;
            _fixture.GetOrCreatePactConfig();
            _fixture.PactBuilder =
                PactNet.Pact.V4(ConsumerContractsFixture.ConsumerName, ConsumerContractsFixture.ProviderName, _fixture.PactConf)
                .WithHttpInteractions();
        }

        [Fact]
        public async Task Given_ManufacturerName_and_Year_When_Getting_Manufacturer_Models_Then_Returns_Data()
        {
            string manufacturer = "tesla";
            int year = 2018;

            var expectedResponse = new NhtsaCarModelResponce
            {
                Count = 3,
                Message = ConsumerContractsFixture.SuccessMessage,
                SearchCriteria = $"Make:Tesla | ModelYear:2018",
                Results = new[] {
                            new ModelResult {
                                Make_ID = 441,
                                Make_Name = $"{manufacturer}",
                                Model_ID = 1685,
                                Model_Name = "Model S"
                            },
                            new ModelResult {
                                Make_ID = 441,
                                Make_Name = $"{manufacturer}",
                                Model_ID = 10199,
                                Model_Name = "Model X"
                            },
                            new ModelResult {
                                Make_ID = 441,
                                Make_Name = $"{manufacturer}",
                                Model_ID = 17834,
                                Model_Name = "Model 3"
                            }
                        }
            };

            _fixture.PactBuilder
                    .UponReceiving($"A GET request to provider/api/cars/manufacturers/{manufacturer}/models/{year}")
                    .Given("an order with ID {id} exists", new Dictionary<string, string> { ["id"] = "1" })
                .WithRequest(HttpMethod.Get, $"/provider/api/cars/manufacturers/{manufacturer}/models/{year}")
                .WithHeader("Accept", "application/json")
                .WithHeader("Authorization", "Bearer Ssangyong")
                    .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(expectedResponse);

            await _fixture.PactBuilder.VerifyAsync(async ctx =>
            {
                _fixture.SetupHttpClientMock(_mockFactory, ctx.MockServerUri);

                var consumer = new CarService(_mockFactory.Object);
                var response = await consumer.GetModels(manufacturer, year);

                var result = JsonConvert.DeserializeObject<NhtsaCarModelResponce>(await response.Content.ReadAsStringAsync());
                result.Should().BeEquivalentTo(expectedResponse); //--> NOT A PACT ! Unit Test !
            });
        }
    }
}