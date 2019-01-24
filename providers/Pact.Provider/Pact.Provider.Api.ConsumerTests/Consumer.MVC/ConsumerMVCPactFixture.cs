using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Pact.Provider.Api.ConsumerTests.xUnit;

namespace Pact.Provider.Api.ConsumerTests.Consumer.MVC
{
    public class ConsumerMVCPactFixture : IDisposable
    {
        public const string Name = "Consumer MVC pacts";
        public string ProviderUri => "http://localhost:5000";
        public string PactVerifierStatesUri => "http://localhost:5001";
        public string PactBrokerUri => "http://23.97.153.18";
        private readonly IWebHost _providerWebHost;
        private readonly IWebHost _pactProviderStatesWebHost;
        public IPactVerifier PactVerifier { get; private set; }

        public ConsumerMVCPactFixture()
        {
            _providerWebHost = WebHost.CreateDefaultBuilder()
                .Configure(configureApp =>
                {
                    // adding a middleware to handle service provider state setting calls ("given")
                    // configureApp.UseMiddleware<ProviderStateMiddleware>();
                })
                .UseUrls(ProviderUri)
                // .UseContentRoot(aaa)
                .UseStartup<TestStartup>()
                .ConfigureServices(services =>
                {
                    // !! adding missing service with mocked responses
                    services.AddSingleton<INhtsaHttpClient, HttpClientMock>();
                })
                .UseIISIntegration()
                .Build();
            _providerWebHost.RunAsync();

            _pactProviderStatesWebHost = WebHost.CreateDefaultBuilder()
                .UseUrls(PactVerifierStatesUri)
                .UseStartup<PactStateStartup>()
                .Build();
            _pactProviderStatesWebHost.Start();
        }

        internal IPactVerifier SetPactVerifier(ITestOutputHelper output)
        {
            var config = new PactVerifierConfig
            {
                // default : ConsoleOutput,however xUnit 2 does not capture
                // the console output, so a custom outputter is required.
                Outputters = new[] { new XUnitOutput(output) },
                Verbose = true,
                ProviderVersion = "7",
                PublishVerificationResults = true
            };

            PactVerifier = new PactVerifier(config);
            PactVerifier
                .ServiceProvider("Pact.Provider.Api", ProviderUri)
                .HonoursPactWith("Pact.Consumer.MVC")
                //.PactUri(@"..\..\..\..\..\pacts\consumer-provider.json")
                .PactUri($"{PactBrokerUri}/pacts/provider/Pact.Provider.Api/consumer/Pact.Consumer.MVC/latest");
            return PactVerifier;
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _providerWebHost.StopAsync().GetAwaiter().GetResult();
                    _providerWebHost.Dispose();
                    _pactProviderStatesWebHost.StopAsync().GetAwaiter().GetResult();
                    _pactProviderStatesWebHost.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}