using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Pact.Provider.PactProviderTests
{
    public class ProviderStateMiddleware
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };
        private const string ConsumerName = "Pact.Consumer.MVC";
        private readonly RequestDelegate _next;
        private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> _consumerMVCproviderStates;

        public ProviderStateMiddleware(RequestDelegate next)
        {
            _next = next;
            _consumerMVCproviderStates = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                // lack of handling provider state given in pact file goes to pact verification error also !
                ["an order with ID {id} exists"] = AddVehicleIdFirst_ThenDoPactTest,
                ["a vehicle with VIN 5UXWX7C5ABA"] = DoSomethingFirst_ThenDoPactTest,
                ["a vehicle with VIN some_wrong_vin"] = DoSomethingFirst_ThenDoPactTest,
                ["a new vehicle with VIN a_new_vin"] = DoSomethingFirst_ThenDoPactTest,
            };
        }

        private Task AddVehicleIdFirst_ThenDoPactTest(IDictionary<string, object> parameters)
        {
            // set some provider state here
            JsonElement id = (JsonElement)parameters["id"];
            var idn = id.GetInt32();
            return Task.FromResult(idn);
        }

        private Task DoSomethingFirst_ThenDoPactTest(IDictionary<string, object> parameters)
        {
            JsonElement id = (JsonElement)parameters["VIN"];
            var vin = id.GetString();
            return Task.FromResult(vin);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!(context.Request.Path.Value?.StartsWith("/provider-states") ?? false))
            {
                await this._next.Invoke(context);
                return;
            }

            // ------------- handling provider states, for POST consumer requests mostly ---------
            context.Response.StatusCode = StatusCodes.Status200OK;

            if (context.Request.Method == System.Net.Http.HttpMethod.Post.ToString())
            {
                string jsonRequestBody;

                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                try
                {
                    ProviderState providerState = System.Text.Json.JsonSerializer.Deserialize<ProviderState>(jsonRequestBody, Options);

                    if (!string.IsNullOrEmpty(providerState?.State))
                    {
                        this._consumerMVCproviderStates[providerState.State].Invoke(providerState.Params);
                    }
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("Failed to deserialise JSON provider state body:");
                    await context.Response.WriteAsync(jsonRequestBody);
                    await context.Response.WriteAsync(string.Empty);
                    await context.Response.WriteAsync(e.ToString());
                }
            }
        }
    }
}