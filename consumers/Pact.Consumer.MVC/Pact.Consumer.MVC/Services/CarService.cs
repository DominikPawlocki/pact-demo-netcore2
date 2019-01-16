using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pact.Consumer.MVC.Models;

namespace Pact.Consumer.MVC.Services {
    public interface ICarService {
        Task<NhtsaManufacturersResponce> GetManufacturers ();
        Task<HttpResponseMessage> GetManufacturerDetails (string manufacturer);
        Task<HttpResponseMessage> GetModels (string manufacturer, int year);
        Task<NhtsaVINdecoderResponce> DecodeVin (string vin);
    }

    public class CarService : ICarService {
        private readonly string _providerUri;

        public CarService (string providerUri) {
            if (string.IsNullOrWhiteSpace (providerUri)) {
                providerUri = "http://localhost:5000/";
            }
            _providerUri = providerUri;
        }

        public async Task<NhtsaManufacturersResponce> GetManufacturers () {
            string endpoint = "provider/api/cars/manufacturers/random20";
            try{
            var responce = await ProviderApiClient.Get ().GetAsync (_providerUri + endpoint);
            return await responce.Content.ReadAsAsync<NhtsaManufacturersResponce> ();
            }
            catch{              
                return new NhtsaManufacturersResponce {
                    Count = 0,
                        Message = "Error",
                        Results = new [] {
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

        public async Task<HttpResponseMessage> GetManufacturerDetails (string manufacturer) {
            string endpoint = $"/provider/api/cars/manufacturers/{manufacturer}/details";
            return await ProviderApiClient.Get ().GetAsync (_providerUri + endpoint);
        }

        public async Task<HttpResponseMessage> GetModels (string manufacturer, int year) {
            string endpoint = $"/provider/api/cars/manufacturers/{manufacturer}/models/{year}";
            return await ProviderApiClient.Get ().GetAsync (_providerUri + endpoint);
        }

        public async Task<NhtsaVINdecoderResponce> DecodeVin (string vin) {
            string endpoint = $"provider/api/cars/vin/{vin}";
            try {
                return await GetData<NhtsaVINdecoderResponce> (endpoint);
            } catch (HttpRequestException e) {
                return new NhtsaVINdecoderResponce {
                    Count = 0,
                        Message = e.Message
                };
            }
        }

        private async Task<T> GetData<T> (string endpoint) where T : class, new () {
            T result;
            var streamTask = await ProviderApiClient.Get ().GetStringAsync (_providerUri + endpoint);

            result = JsonConvert.DeserializeObject<T> (streamTask);
            //return await responce.Content.ReadAsAsync<NhtsaManufacturersResponce>();
            return result;
        }

    }
}