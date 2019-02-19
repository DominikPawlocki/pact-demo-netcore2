using System;
using System.Collections.Generic;
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
    public class ManufacturerDetailsContracts
    {
        private readonly IMockProviderService _mockProviderService;
        private readonly string _mockProviderServiceBaseUri;
        private readonly ConsumerContractsFixture _fixture;

        public ManufacturerDetailsContracts(ConsumerContractsFixture fixture)
        {
            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
            _mockProviderService = fixture.MockProviderService;
            _fixture = fixture;
            _mockProviderService.ClearInteractions();
        }

        [Fact]
        public async Task Given_Tesla_When_Getting_Manufacturers_Details_Returns_Tesla_Details()
        {
            string providerResource = "tesla";

            _mockProviderService
                .UponReceiving($"A GET request to retrieve provider/api/cars/manufacturers/{providerResource}/details")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/provider/api/cars/manufacturers/{providerResource}/details",
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
                    Body = new NhtsaManufacturerDetailsResponce
                    {
                        Count = 1,
                        Message = _fixture.SuccessMessage,
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
                    }
                });

            // Act
            var consumer = new CarService(_mockProviderServiceBaseUri);
            var response = await consumer.GetManufacturerDetails(providerResource);

            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public async Task Given_Not_Existing_Manufacturer_When_Getting_Manufacturers_Details_Returns_404()
        {
            string providerResource = "fsoo";
            _mockProviderService
                .UponReceiving($"A GET request to retrieve provider/api/cars/manufacturers/{providerResource}/details")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = $"/provider/api/cars/manufacturers/{providerResource}/details",
                    Headers = new Dictionary<string, object> {
                        { "Accept", "application/json" }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.NotFound,
                });

            var consumer = new CarService(_mockProviderServiceBaseUri);
            var response = await consumer.GetManufacturerDetails(providerResource);

            _mockProviderService.VerifyInteractions();
        }
    }
}