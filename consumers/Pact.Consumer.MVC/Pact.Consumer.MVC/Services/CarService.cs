using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pact.Consumer.MVC.Models;

namespace Pact.Consumer.MVC.Services
{
    public interface ICarService
    {
        // GET - expects a custom Http Header to be returned from provider
        Task<NhtsaManufacturersResponce> GetManufacturers();
        // GET - 2 tests, expects a 404 when car brand doesnt exists
        Task<HttpResponseMessage> GetManufacturerDetails(string manufacturer);
        // GET - expects a custom Http Header in sent to provider
        Task<HttpResponseMessage> GetModels(string manufacturer, int year);
        // GET - 2 tests, expects empty response when VIN doesnt exist, with provider resource
        Task<NhtsaVINdecoderResponce> DecodeVin(string vin);
        // POST - expects correct body and header is sent to provider
        Task<HttpResponseMessage> UpsertVin(string vin);
    }

    public class CarService : ICarService
    {
        private readonly IHttpClientFactory _factory;

        public CarService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<NhtsaManufacturersResponce> GetManufacturers()
        {
            string endpoint = "provider/api/cars/manufacturers/random20";
            try
            {
                return await GetData<NhtsaManufacturersResponce>(endpoint);
            }
            catch (Exception ex)
            {
                return new NhtsaManufacturersResponce
                {
                    Count = 0,
                    Message = "Error",
                    Results = new[] {
                        new ManufacturerResult {
                            Country = "Error",
                            Mfr_CommonName = "Error",
                            Mfr_ID = 0,
                            Mfr_Name = "Error",
                            VehicleTypes = new [] {
                                new VehicleType {
                                    IsPrimary = false,
                                    Name = "Error"
                                }
                            }
                        }
                    }
                };
            }
        }

        public async Task<HttpResponseMessage> GetManufacturerDetails(string manufacturer)
        {
            string endpoint = $"/provider/api/cars/manufacturers/{manufacturer}/details";
            return await new ProviderApiClient(_factory).WithDefaultHeader().GetAsync(endpoint);
        }

        public async Task<HttpResponseMessage> GetModels(string manufacturer, int year)
        {
            string endpoint = $"/provider/api/cars/manufacturers/{manufacturer}/models/{year}";
            return await new ProviderApiClient(_factory).WithCustomHeader().GetAsync(endpoint);
        }

        public async Task<NhtsaVINdecoderResponce> DecodeVin(string vin)
        {
            string endpoint = $"provider/api/cars/vin/{vin}";
            try
            {
                return await GetData<NhtsaVINdecoderResponce>(endpoint);
            }
            catch (HttpRequestException e)
            {
                return new NhtsaVINdecoderResponce
                {
                    Count = 0,
                    Message = e.Message
                };
            }
        }

        public async Task<HttpResponseMessage> UpsertVin(string vin)
        {
            var newVinRequestBody = new NhtsaVINRequest()
            {
                Vin = vin,
                Message = "Add new VIN into database",
                CarDetail = new CarDetails()
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

            string endpoint = "provider/api/cars/vin";
            return await new ProviderApiClient(_factory).WithDefaultHeader().PostAsJsonAsync(endpoint, newVinRequestBody);
        }

        private async Task<T> GetData<T>(string endpoint) where T : class, new()
        {
            T result;
            var streamTask = await new ProviderApiClient(_factory)
                .WithDefaultHeader().GetStringAsync(endpoint);

            result = JsonConvert.DeserializeObject<T>(streamTask);
            return result;
        }

    }
}