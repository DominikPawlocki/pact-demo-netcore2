using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Pact.Consumer.MVC.Models;
using Pact.Consumer.MVC.Services;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace Pact.Consumer.MVC.PactTests.With.Pact.Provider.Api
{
    [Collection(ConsumerContractsFixture.CollectionName)]
    public class VinDecodingContracts
    {
        private readonly IMockProviderService _mockProviderService;
        private readonly string _mockProviderServiceBaseUri;
        private readonly ConsumerContractsFixture _fixture;

        public VinDecodingContracts(ConsumerContractsFixture fixture)
        {
            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
            _mockProviderService = fixture.MockProviderService;
            _fixture = fixture;
            _mockProviderService.ClearInteractions();
        }

        [Fact]
        public async Task Given_5UXWX7C5ABA_VIN_When_DecodeVIN_Then_Returns_Data()
        {
            string providerResource = "5UXWX7C5ABA";
            _mockProviderService
                .Given(providerResource)
                .UponReceiving($"A GET request to provider/api/cars/vin/{providerResource}")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/provider/api/cars/vin/{providerResource}",
                    Headers = new Dictionary<string, object> { { "Accept", "application/json" } }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.OK,
                    Headers = new Dictionary<string, object> { { "Content-Type", "application/json; charset=utf-8" } },
                    Body = new NhtsaVINdecoderResponce
                    {
                        Count = 1,
                        Message = _fixture.SuccessMessage,
                        SearchCriteria = $"VIN(s): {providerResource}",
                        Results = new[] {
                            new CarDetails {
                                AdditionalErrorText = "",
                                ErrorCode = "",
                                EngineCylinders = "6",
                                EngineKW = 223.7100,
                                FuelTypePrimary = "Gasoline",
                                FuelTypeSecondary = "",
                                Make = "BMW",
                                Manufacturer = "BMW MANUFACTURER CORPORATION / BMW NORTH AMERICA",
                                Model = "X3",
                                ModelYear = 2011,
                                PlantCity = "Munich",
                                PlantCountry = "Germany",
                                PlantState = "",
                                VehicleType = "MULTIPURPOSE PASSENGER VEHICLE (MPV)"
                                }
                            }
                    }
                });

            var consumer = new CarService(_mockProviderServiceBaseUri);
            var result = await consumer.DecodeVin(providerResource);

            // Assert
            Assert.DoesNotContain("No interaction found", result.Message);
            Assert.NotNull(result.Results);
            Assert.Single(result.Results);
            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public async Task Given_Wrong_VIN_When_DecodeVIN_Then_Returns_EmptyData()
        {
            string providerResource = "some_wrong_vin";
            _mockProviderService
                .Given($"{providerResource}")
                .UponReceiving($"A GET request to provider/api/cars/vin/{providerResource}")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/provider/api/cars/vin/{providerResource}",
                    Headers = new Dictionary<string, object> { { "Accept", "application/json" } }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.OK,
                    Headers = new Dictionary<string, object> { { "Content-Type", "application/json; charset=utf-8" } },
                    Body = new NhtsaVINdecoderResponce
                    {
                        Count = 1,
                        Message = _fixture.SuccessMessage,
                        SearchCriteria = $"VIN(s): {providerResource}",
                        Results = new[] {
                            new CarDetails {
                                AdditionalErrorText = "",
                                ErrorCode = "6 - Incomplete VIN; 7 - Manufacturer is not registered with NHTSA for sale or importation in the U.S. for use on U.S roads; Please contact the manufacturer directly for more information; 400 - Invalid Characters Present (I, O, Q",
                                EngineCylinders = "",
                                EngineKW = 0.0,
                                FuelTypePrimary = "",
                                FuelTypeSecondary = "",
                                Make = "",
                                Manufacturer = "",
                                Model = "",
                                ModelYear = 0,
                                PlantCity = "",
                                PlantCountry = "",
                                PlantState = "",
                                VehicleType = ""
                            }
                        }
                    }
                });

            var consumer = new CarService(_mockProviderServiceBaseUri);
            var result = await consumer.DecodeVin(providerResource);

            // Assert
            Assert.DoesNotContain("No interaction found", result.Message);
            Assert.Single(result.Results);
            _mockProviderService.VerifyInteractions();
        }
    }
}