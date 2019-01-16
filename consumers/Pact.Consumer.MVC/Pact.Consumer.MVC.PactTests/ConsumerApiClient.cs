using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pact.Consumer.MVC.PactTests
{
    public class ConsumerApiClient
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public ConsumerApiClient(HttpClient client, string baseUrl)
        {
            _client = client;
            _baseUrl = baseUrl;
        }

        public async Task<T> GetDataFromPactMock<T>(string endpoint) where T : class, new()
        {
            T result;
            var streamTask = await _client.GetStringAsync(_baseUrl + endpoint);

            result = JsonConvert.DeserializeObject<T>(streamTask);
            return result;
        }
    }
}