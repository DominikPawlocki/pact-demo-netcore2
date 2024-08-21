using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Pact.Consumer.MVC.Models;
using Pact.Consumer.MVC.Services;
using PactNet;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Pact.Consumer.MVC.PactTests.With.Pact.Provider.Api
{
    [Collection(ConsumerContractsFixture.CollectionName)]
    public class ManufacturerDetailsContracts
    {
        Mock<IHttpClientFactory> _mockFactory = new();
        private readonly ConsumerContractsFixture _fixture;

        public ManufacturerDetailsContracts(ConsumerContractsFixture fixture)
        {
            _fixture = fixture;
            _fixture.GetOrCreatePactConfig();
            _fixture.PactBuilder =
                PactNet.Pact.V4(ConsumerContractsFixture.ConsumerName, ConsumerContractsFixture.ProviderName, _fixture.PactConf)
                .WithHttpInteractions();
        }

        [Fact]
        public async Task Given_Tesla_When_Getting_Manufacturers_Details_Returns_Tesla_Details()
        {
            string providerResource = "tesla";

            var expectedResponse = new NhtsaManufacturerDetailsResponce
            {
                Count = 1,
                Message = ConsumerContractsFixture.SuccessMessage,
                SearchCriteria = null,
                Results = new[] {
                    new ManufacturerDetailsResult {
                                Address = "3500 Deer Creek Road",
                                City = "Palo Alto",
                                ContactEmail = "callen@tesla.com",
                                ContactPhone = "(267)808-8976",
                                Country = "United States (USA)",
                                DBAs = "Tesla,Inc",
                                LastUpdated = DateTime.Parse("2017-03-27T20:25:00"),
                                Mfr_CommonName = "Tesla",
                                Mfr_ID = 955,
                                Mfr_Name = "TESLA, INC.",
                                PostalCode = "94304",
                                PrincipalFirstName = "Elon Musk",
                                PrincipalLastName = null,
                                PrincipalPosition = "CEO",
                                StateProvince = "California",
                                SubmittedName = "Charity Allen",
                                SubmittedPosition = "Managing Counsel, Regulatory"
                            }
                        }
            };

            _fixture.PactBuilder
                .UponReceiving($"A GET request to retrieve provider/api/cars/manufacturers/{providerResource}/details")
                .Given("an order with ID {id} exists", new Dictionary<string, string> { ["id"] = "1" })
            .WithRequest(HttpMethod.Get, $"/provider/api/cars/manufacturers/{providerResource}/details")
            .WithHeader("Accept", "application/json")
                .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(expectedResponse);

            await _fixture.PactBuilder.VerifyAsync(async ctx =>
            {
                _fixture.SetupHttpClientMock(_mockFactory, ctx.MockServerUri);

                var consumer = new CarService(_mockFactory.Object);
                var response = await consumer.GetManufacturerDetails(providerResource);

                var result = JsonConvert.DeserializeObject<NhtsaManufacturerDetailsResponce>(await response.Content.ReadAsStringAsync());
                result.Should().BeEquivalentTo(expectedResponse); //--> NOT A PACT ! Unit Test !
            });
        }

        [Fact]
        public async Task Given_Not_Existing_Manufacturer_When_Getting_Manufacturers_Details_Returns_404()
        {
            string providerResource = "fsoo";

            _fixture.PactBuilder
               .UponReceiving($"A GET request to retrieve provider/api/cars/manufacturers/{providerResource}/details")
           .WithRequest(HttpMethod.Get, $"/provider/api/cars/manufacturers/{providerResource}/details")
           .WithHeader("Accept", "application/json")
               .WillRespond()
           .WithStatus(HttpStatusCode.NotFound);

            await _fixture.PactBuilder.VerifyAsync(async ctx =>
            {
                _fixture.SetupHttpClientMock(_mockFactory, ctx.MockServerUri);

                var consumer = new CarService(_mockFactory.Object);
                var response = await consumer.GetManufacturerDetails(providerResource);
            });
        }
    }
}