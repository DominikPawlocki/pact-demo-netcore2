using System;
using System.Net.Http;

namespace Pact.Provider.Api.Services
{
    public interface INhtsaHttpClient
    {
        HttpClient Client { get; }
    }

    internal class NhtsaHttpClient : INhtsaHttpClient
    {
        public HttpClient Client { get; }
        private readonly string _uri = "https://vpic.nhtsa.dot.gov/api/vehicles/";

        public NhtsaHttpClient()
        {
            Client = new HttpClient
            {
                BaseAddress = new Uri(_uri)
            };
        }
    }
}
