using System.Collections.Generic;
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

        [Fact(DisplayName = "Decoding '5UXWX7C5ABA' VIN's returns data")]
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
                    Headers = new Dictionary<string, object> {
                        { "Accept", "application/json" }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.OK,
                    Headers = new Dictionary<string, object> {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new NhtsaVINdecoderResponce {
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

            _mockProviderService.VerifyInteractions();
        }

        [Fact(DisplayName = "Decoding 'some_wrong VIN' returns data")]
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
                    Headers = new Dictionary<string, object> {
                        { "Accept", "application/json" }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse {
                    Status = (int)HttpStatusCode.OK,
                    Headers = new Dictionary<string, object> {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new NhtsaVINdecoderResponce {
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
            Assert !

            _mockProviderService.VerifyInteractions();
        }

        [Fact(DisplayName = "Adding a new car VIN to database returns 201 and Id")]
        public async Task Given_New_VIN_When_Posting_it_Then_Returns_Created()
        {
            string providerResource = "a_new_vin";
            _mockProviderService
                .Given($"{providerResource}")
                .UponReceiving("A POST request to provider/api/cars/vin")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Post,
                    Path = "/provider/api/cars/vin",
                    Headers = new Dictionary<string, object> {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new {
                        Vin = providerResource,
                        Message = "Add new VIN into database",
                        CarDetail = new {
                            AdditionalErrorText = "",
                            EngineCylinders = "4",
                            ErrorCode = "",
                            FuelTypePrimary = "Oil",
                            FuelTypeSecondary = "",
                            EngineKW = 344.3,
                            Make = "Audi",
                            Manufacturer = "Audi",
                            Model = "A7",
                            ModelYear = 2018,
                            PlantCountry = "Germany",
                            PlantState = "",
                            PlantCity = "Ingolstadt",
                            VehicleType = ""
                        }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.Created,
                    Headers = new Dictionary<string, object> {
                        { "Content-Type", "application/json; charset=utf-8" },
                        { "Location", "/provider/api/cars" },
                    },
                    Body = new
                    {
                        Id = 15421,
                        Vin = providerResource,
                        Message = "Car added/modified correctly."
                    }
                });

            var consumer = new CarService(_mockProviderServiceBaseUri);
            var response = await consumer.UpsertVin(providerResource);

            // This is not neccessary. 
            // Just faster path notification when request doesnt match expected one and then no interaction is found on PACT server
            /*  var result = JsonConvert.DeserializeObject<PactServerErrorResponse>
                 (await response.Content.ReadAsStringAsync ());

             Assert.Null (result);
             Assert.DoesNotContain ("No interaction found", result.Message);*/
            _mockProviderService.VerifyInteractions();
        }
    }
}