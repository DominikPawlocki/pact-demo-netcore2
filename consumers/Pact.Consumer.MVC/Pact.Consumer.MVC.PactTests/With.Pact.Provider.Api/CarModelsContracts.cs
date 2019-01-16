using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pact.Consumer.MVC.Models;
using Pact.Consumer.MVC.Services;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace Pact.Consumer.MVC.PactTests.With.Pact.Provider.Api
{
    [Collection(ConsumerContractsFixture.CollectionName)]
    public class CarModelsContracts
    {
        private readonly IMockProviderService _mockProviderService;
        private readonly string _mockProviderServiceBaseUri;
        private readonly ConsumerContractsFixture _fixture;

        public CarModelsContracts(ConsumerContractsFixture fixture)
        {
            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
            _mockProviderService = fixture.MockProviderService;
            _fixture = fixture;
            _mockProviderService.ClearInteractions();
        }

        [Fact]
        public async Task Given_ManufacturerName_and_Year_When_Getting_Manufacturer_Models_Then_Returns_Data()
        {
            string manufacturer = "tesla";
            int year = 2018;

            _mockProviderService
                .UponReceiving($"A GET request to provider/api/cars/manufacturers/{manufacturer}/models/{year}")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/provider/api/cars/manufacturers/{manufacturer}/models/{year}",
                    Headers = new Dictionary<string, object> { { "Accept", "application/json" } }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.OK,
                    Headers = new Dictionary<string, object> { { "Content-Type", "application/json; charset=utf-8" } },
                    Body = new NhtsaCarModelResponce
                    {
                        Count = 3,
                        Message = _fixture.SuccessMessage,
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
                    }
                });

            // Act
            var consumer = new CarService(_mockProviderServiceBaseUri);
            var response = await consumer.GetModels(manufacturer, year);
            var result = JsonConvert.DeserializeObject<NhtsaCarModelResponce>
                (await response.Content.ReadAsStringAsync());

            Assert.DoesNotContain("No interaction found", result.Message);
            Assert.Equal(3, result.Results.Count());
            _mockProviderService.VerifyInteractions();
        }
    }
}