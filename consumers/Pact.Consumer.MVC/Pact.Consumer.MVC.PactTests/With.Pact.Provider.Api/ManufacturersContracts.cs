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
    public class ManufacturersContracts
    {
        private readonly IMockProviderService _mockProviderService;
        private readonly string _mockProviderServiceBaseUri;
        private readonly ConsumerContractsFixture _fixture;

        public ManufacturersContracts(ConsumerContractsFixture fixture)
        {
            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
            _mockProviderService = fixture.MockProviderService;
            _fixture = fixture;
            _mockProviderService.ClearInteractions();
        }

        [Fact]
        public async Task When_Getting_Random_20_Manufacturers_Returns_Data()
        {
            _mockProviderService
                .UponReceiving("A GET request to retrieve provider/api/cars/manufacturers/random20")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/provider/api/cars/manufacturers/random20",
                    Headers = new Dictionary<string, object> {
                        { "Accept", "application/json" }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse {
                    Status = (int)HttpStatusCode.OK,
                    Headers = new Dictionary<string, object> {
                        { "Content-Type", "application/json; charset=utf-8" },
                        { "Korean","Ssangyong in header" }
                    },
                    Body = new NhtsaManufacturersResponce {
                        Count = 2,
                        Message = _fixture.SuccessMessage,
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
                            new ManufacturerResult {
                                Country = "Japan",
                                Mfr_CommonName = "Mazda",
                                Mfr_ID = 1041,
                                Mfr_Name = "Mazda Motor Corporation",
                                VehicleTypes = new [] {
                                    new VehicleType {
                                        IsPrimary = true,
                                        Name = "Multipurpose Passenger Vehicle (MPV)"
                                    }
                                }
                            }
                        }
                    }
                });

            var consumer = new CarService(_mockProviderServiceBaseUri);
            var result = await consumer.GetManufacturers();

            Assert !
            _mockProviderService.VerifyInteractions();
        }
    }
}