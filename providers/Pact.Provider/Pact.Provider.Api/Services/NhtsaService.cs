using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pact.Provider.Api.Models;

namespace Pact.Provider.Api.Services
{
    public interface INhtsaService
    {
        Task<NhtsaManufacturersResponce> GetRandom20Manufacturers();
        Task<NhtsaManufacturersResponce> GetFirst100Manufacturers();
        Task<NhtsaVINdecoderResponce> DecodeVin(string vin);
        Task<NhtsaCarModelResponce> GetModels(string manufacturer, int year);
        Task<NhtsaManufacturerDetailsResponce> GetManufacturerDetails(string manufacturer);
    }

    public class NhtsaService : INhtsaService
    {
        private readonly HttpClient _client;

        public NhtsaService(INhtsaHttpClient nhtsaHttpClient)
        {
            _client = nhtsaHttpClient.Client;
        }
               
        public async Task<NhtsaManufacturersResponce> GetFirst100Manufacturers()
        {
            return await GetData<NhtsaManufacturersResponce>($"getallmanufacturers?format=json");
        }

        public async Task<NhtsaVINdecoderResponce> DecodeVin(string vin)
        {
            return await GetData<NhtsaVINdecoderResponce>($"decodevinvalues/{vin}?format=json");
        }

        public async Task<NhtsaCarModelResponce> GetModels(string manufacturer, int year)
        {
            return await GetData<NhtsaCarModelResponce>($"getmodelsformakeyear/make/{manufacturer}/modelyear/{year}?format=json");
        }

        public async Task<NhtsaManufacturerDetailsResponce> GetManufacturerDetails(string manufacturer)
        {
            return await GetData<NhtsaManufacturerDetailsResponce>($"getmanufacturerdetails/{manufacturer}?format=json");
        }

        public async Task<NhtsaManufacturersResponce> GetRandom20Manufacturers()
        {
            var result = await GetData<NhtsaManufacturersResponce>($"getallmanufacturers?format=json");
            if (result.Results.Count() < 21)
            {
                return result;
            }

            result.Count = 20;
            result.Results = result.Results
                .Where(_ => !string.IsNullOrWhiteSpace(_.Mfr_CommonName))
                .GroupBy(_ => _.Mfr_ID).Select(y => y.FirstOrDefault())
                .ToArray();

            var random20 = new List<ManufacturerResult>(20);
            foreach (int el in Generate20Randoms(result.Results.Count()))
            {
                random20.Add(result.Results.ElementAt(el));
            }
            result.Results = random20.ToList().ToArray();

            return result;
        }

        private int[] Generate20Randoms(int max)
        {
            var randoms = new List<int>(20);
            Random r = new Random();
            for (int i = 0; i < 20; i++)
            {
                var randomNr = r.Next(0, max - 1);
                if (randoms.Contains(randomNr))
                {
                    i--;
                    continue;
                }
                randoms.Add(randomNr);
            }
            randoms.Sort();
            return randoms.ToArray();
        }

        private async Task<T> GetData<T>(string endpoint) where T : NhtsaBaseResponse, new()
        {
            T result;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await _client.GetAsync(endpoint);
                result = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException e)
            {
                return new T
                {
                    Count = 0,
                    Message = e.Message,
                };
            }
            return result;
        }
    }
}