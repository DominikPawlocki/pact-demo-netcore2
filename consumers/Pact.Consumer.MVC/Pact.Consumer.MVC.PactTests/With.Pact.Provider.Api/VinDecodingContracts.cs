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
    public class VinDecodingContracts
    {
        private readonly ConsumerContractsFixture _fixture;
        Mock<IHttpClientFactory> _mockFactory = new();

        public VinDecodingContracts(ConsumerContractsFixture fixture)
        {
            _fixture = fixture;
            _fixture.GetOrCreatePactConfig();
            _fixture.PactBuilder =
                PactNet.Pact.V4(ConsumerContractsFixture.ConsumerName, ConsumerContractsFixture.ProviderName, _fixture.PactConf)
                .WithHttpInteractions();
        }

        //[Fact(DisplayName = "Decoding '5UXWX7C5ABA' VIN's returns data")]
        [Fact]
        public async Task Given_5UXWX7C5ABA_VIN_When_DecodeVIN_Then_Returns_Data()
        {
            string providerResource = "5UXWX7C5ABA";

            var expectedProviderResponse = new NhtsaVINdecoderResponce
            {
                Count = 1,
                Message = ConsumerContractsFixture.SuccessMessage,
                SearchCriteria = $"VIN(s): {providerResource}",
                Results = [
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
                        ]
            };

            _fixture.PactBuilder
                      .UponReceiving($"A GET request to provider/api/cars/vin/{providerResource}")
                      .Given($"a vehicle with VIN {providerResource}", new Dictionary<string, string> { ["VIN"] = providerResource })
                      .WithRequest(HttpMethod.Get, $"/provider/api/cars/vin/{providerResource}")
                      .WithHeader("Accept", "application/json")
                  .WillRespond()
                      .WithStatus(HttpStatusCode.OK)
                      .WithHeader("Content-Type", "application/json; charset=utf-8")
                      .WithJsonBody(expectedProviderResponse);

            await _fixture.PactBuilder.VerifyAsync(async ctx =>
            {
                _fixture.SetupHttpClientMock(_mockFactory, ctx.MockServerUri);

                var consumer = new CarService(_mockFactory.Object);
                var result = await consumer.DecodeVin(providerResource);

                Assert.Equal(ConsumerContractsFixture.SuccessMessage, result.Message);
                result.Should().BeEquivalentTo(expectedProviderResponse); //--> NOT A PACT ! Unit Test !
            });
        }

        //[Fact(DisplayName = "Decoding 'some_wrong VIN' returns data")]
        [Fact]
        public async Task Given_Wrong_VIN_When_DecodeVIN_Then_Returns_EmptyData()
        {
            string providerResource = "some_wrong_vin";
            var expectedProviderResponse = new NhtsaVINdecoderResponce
            {
                Count = 1,
                Message = ConsumerContractsFixture.SuccessMessage,
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
            };

            _fixture.PactBuilder
                         .UponReceiving($"A GET request to provider/api/cars/vin/{providerResource}")
                         .Given($"a vehicle with VIN {providerResource}", new Dictionary<string, string> { ["VIN"] = providerResource })
                         .WithRequest(HttpMethod.Get, $"/provider/api/cars/vin/{providerResource}")
                         .WithHeader("Accept", "application/json")
                     .WillRespond()
                         .WithStatus(HttpStatusCode.OK)
                         .WithHeader("Content-Type", "application/json; charset=utf-8")
                         .WithJsonBody(expectedProviderResponse);

            await _fixture.PactBuilder.VerifyAsync(async ctx =>
            {
                _fixture.SetupHttpClientMock(_mockFactory, ctx.MockServerUri);

                var consumer = new CarService(_mockFactory.Object);
                var result = await consumer.DecodeVin(providerResource);

                Assert.Equal(ConsumerContractsFixture.SuccessMessage, result.Message);//--> NOT A PACT ! Unit Test !
                result.Should().BeEquivalentTo(expectedProviderResponse);
            });
        }

        //[Fact(DisplayName = "Adding a new car VIN to database returns 201 and Id")]
        [Fact]
        public async Task Given_New_VIN_When_Adding_it_Then_Returns_Created_http_status_and_its_Id()
        {
            string providerResource = "a_new_vin";

            var requestBody = new
            {
                Vin = providerResource,
                Message = "Add new VIN into database",
                CarDetail = new
                {
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
            };

            var expectedProviderResponse = new
            {
                Id = 15421,
                Vin = providerResource,
                Message = "Car added/modified correctly."
            };

            _fixture.PactBuilder
                      .UponReceiving($"A POST request to provider/api/cars/vin/")
                      .Given($"a new vehicle with VIN {providerResource}", new Dictionary<string, string> { ["VIN"] = providerResource })
                      .WithRequest(HttpMethod.Post, $"/provider/api/cars/vin")
                      .WithJsonBody(requestBody)
                      .WithHeader("Accept", "application/json")
                  .WillRespond()
                      .WithStatus(HttpStatusCode.Created)
                      .WithHeader("Content-Type", "application/json; charset=utf-8")
                      .WithHeader("Location", "/provider/api/cars")
                      .WithJsonBody(expectedProviderResponse);


            await _fixture.PactBuilder.VerifyAsync(async ctx =>
            {
                _fixture.SetupHttpClientMock(_mockFactory, ctx.MockServerUri);

                var consumer = new CarService(_mockFactory.Object);
                var response = await consumer.UpsertVin(providerResource);

                // Just notification when request doesnt match expected one and then no interaction is found on PACT server
                var result = JsonConvert.DeserializeObject<NhtsaVINResponce>(await response.Content.ReadAsStringAsync());
                result.Message.Should().Be("Car added/modified correctly.");
                result.Should().BeEquivalentTo(expectedProviderResponse);
            });
        }
    }
}