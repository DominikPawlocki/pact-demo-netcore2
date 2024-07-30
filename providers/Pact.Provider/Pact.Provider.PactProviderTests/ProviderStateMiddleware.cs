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
        private readonly IDictionary<string, Action> _consumerMVCproviderStates;

        public ProviderStateMiddleware(RequestDelegate next)
        {
            _next = next;
            _consumerMVCproviderStates = new Dictionary<string, Action>
            {
                // lack of handling provider state given in pact file goes to pact verification error also !
                {
                    "an order with ID {id} exists",
                    () => { }
                }
            };
        }

        private void AddData()
        {
            // set some provider state here
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!(context.Request.Path.Value?.StartsWith("/provider-states") ?? false))
            {
                await this._next.Invoke(context);
                return;
            }

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
                        this._consumerMVCproviderStates[providerState.State].Invoke();
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