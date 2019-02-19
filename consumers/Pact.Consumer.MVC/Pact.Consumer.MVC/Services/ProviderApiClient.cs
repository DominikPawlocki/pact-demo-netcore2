using System.Net.Http;
using System.Net.Http.Headers;

namespace Pact.Consumer.MVC.Services
{
    public static class ProviderApiClient
    {
        private static readonly HttpClient _client = new HttpClient();

        public static HttpClient WithDefaultHeader()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return _client;
        }

        public static HttpClient WithCustomHeader()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer Ssangyong");
            return _client;
        }
    }
}