using System;
using System.Net.Http;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;

namespace Pact.Provider.Api.ConsumerTests.Consumer.MVC
{
    public class HttpClientMock : INhtsaHttpClient
    {
        public HttpClient Client { get; }
        private readonly string _successMessage = "Results returned successfully";
        private readonly string _uri = "https://vpic.nhtsa.dot.gov/api/vehicles/";
        private readonly MockHttpMessageHandler _mockHttpMessageHandler;

        public HttpClientMock()
        {
            _mockHttpMessageHandler = new MockHttpMessageHandler();
            Client = _mockHttpMessageHandler.ToHttpClient();
            Client.BaseAddress = new Uri(_uri);
            SetUpManufacturerDetails();
            SetUpVINDecoding();
            SetUpModelsGetting();
            SetUpManufacturerGetting();
        }

        private void SetUpManufacturerDetails()
        {
            var response = new
            {
                Count = 1,
                Message = _successMessage,
                SearchCriteria = (string)null,
                Results = new[] {
                new {
                Address = "3500 Deer Creek Road",
                City = "Palo Alto",
                ContactEmail = "callen@tesla.com",
                ContactPhone = "(267)808-8976",
                Country = "United States (USA)",
                DBAs = "Tesla,Inc",
                LastUpdated = DateTime.Parse ("2017-03-27T20:25:00"),
                Mfr_CommonName = "Tesla",
                Mfr_ID = 955,
                Mfr_Name = "TESLA, INC.",
                PostalCode = "94304",
                PrincipalFirstName = "Elon Musk",
                PrincipalLastName = (string) null,
                PrincipalPosition = "CEO",
                StateProvince = "California",
                SubmittedName = "Charity Allen",
                SubmittedPosition = "Managing Counsel, Regulatory"
                }
                }
            };
            _mockHttpMessageHandler.When(_uri + "getmanufacturerdetails/tesla?format=json")
                .Respond("application/json",
                    JsonConvert.SerializeObject(response)
                );

            _mockHttpMessageHandler.When(_uri + "getmanufacturerdetails/fsoo?format=json")
                .Respond("application/json",
                    JsonConvert.SerializeObject(
                        new
                        {
                            Count = 0,
                            Message = _successMessage,
                            SearchCriteria = (string)null,
                            Results = (string)null
                        }
                    )
                );
        }

        private void SetUpVINDecoding()
        {
            var vin = "5UXWX7C5ABA";

            var response = new
            {
                Count = 1,
                Message = _successMessage,
                SearchCriteria = $"VIN(s): {vin}",
                Results = new[] {
                new {
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
            };
            _mockHttpMessageHandler.When(_uri + $"decodevinvalues/{vin}?format=json")
                .Respond("application/json",
                    JsonConvert.SerializeObject(response)
                );

            var vinNotFoundResponse = new
            {
                Count = 1,
                Message = _successMessage,
                SearchCriteria = "VIN(s): some_wrong_vin",
                Results = new[] {
                        new {
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
            _mockHttpMessageHandler.When(_uri + $"decodevinvalues/some_wrong_vin?format=json")
                .Respond("application/json",
                    JsonConvert.SerializeObject(vinNotFoundResponse)
                );
        }

        private void SetUpManufacturerGetting()
        {
            var response = new
            {
                Count = 2,
                Message = _successMessage,
                SearchCriteria = (string)null,
                Results = new[] {
                new {
                Country = "United States (USA)",
                Mfr_CommonName = "Chrysler",
                Mfr_ID = 994,
                Mfr_Name = "FCA US LLC",
                VehicleTypes = new [] {
                new {
                IsPrimary = true,
                Name = "Multipurpose Passenger Vehicle (MPV)"
                }
                }
                },
                new {
                Country = "Japan",
                Mfr_CommonName = "Mazda",
                Mfr_ID = 1041,
                Mfr_Name = "Mazda Motor Corporation",
                VehicleTypes = new [] {
                new {
                IsPrimary = true,
                Name = "Multipurpose Passenger Vehicle (MPV)"
                }
                }
                }
                }
            };

            _mockHttpMessageHandler.When(_uri + "getallmanufacturers?format=json")
                .Respond("application/json",
                    JsonConvert.SerializeObject(response)
                );
        }

        private void SetUpModelsGetting()
        {
            var manufacturer = "tesla";
            int year = 2018;

            var fake = new
            {
                Count = 3,
                Message = _successMessage,
                SearchCriteria = $"Make:Tesla | ModelYear:2018",
                Results = new[] {
                new {
                Make_ID = 441,
                Make_Name = $"{manufacturer}",
                Model_ID = 1685,
                Model_Name = "Model S"
                },
                new {
                Make_ID = 441,
                Make_Name = $"{manufacturer}",
                Model_ID = 10199,
                Model_Name = "Model X"
                },
                new {
                Make_ID = 441,
                Make_Name = $"{manufacturer}",
                Model_ID = 17834,
                Model_Name = "Model 3"
                }
                }
            };
            _mockHttpMessageHandler.When(_uri + $"getmodelsformakeyear/make/{manufacturer}/modelyear/{year}?format=json")
                .Respond("application/json",
                    JsonConvert.SerializeObject(fake)
                );
        }
    }
}