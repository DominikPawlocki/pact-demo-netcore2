using Pact.Provider.Api;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Pact.Consumer.MVC.Services
{
    public class ProviderApiClient
    {
        private readonly HttpClient _client;

        public ProviderApiClient(IHttpClientFactory factory)
        {
             _client = factory.CreateClient(Program.NhtsaPublicApiHttpClientName);
        }
        public HttpClient WithDefaultHeader()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return _client;
        }

        public HttpClient WithCustomHeader()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer Ssangyong");
            return _client;
        }
    }
}