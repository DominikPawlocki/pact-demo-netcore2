using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Newtonsoft.Json;

namespace Pact.Provider.Api.ConsumerTests
{
    public class ProviderStateMiddleware
    {
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
                    "some state",
                    () => { /* set some state in db, file or something */}
                },
                {
                    "5UXWX7C5ABA",
                     AddData
                },
                {
                    "some_wrong_vin",
                     () => { }
                }
            };
        }

        private void AddData()
        {
            // set some provider state here
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value == "/provider-states")
            {
                this.HandleProviderStatesRequest(context);
                await context.Response.WriteAsync(String.Empty);
            }
            else
            {
                await this._next(context);
            }
        }

        private void HandleProviderStatesRequest(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method.ToUpper() == HttpMethod.Post.ToString().ToUpper() &&
                context.Request.Body != null)
            {
                string jsonRequestBody = String.Empty;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = reader.ReadToEnd();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                //A null or empty provider state key must be handled
                if (providerState != null && !String.IsNullOrEmpty(providerState.State) &&
                    providerState.Consumer == ConsumerName)
                {
                    _consumerMVCproviderStates[providerState.State].Invoke();
                }
            }
        }
    }
}