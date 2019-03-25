using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Pact.Provider.Api.ConsumerTests.xUnit;
using Pact.Provider.Api.Services;

namespace Pact.Provider.Api.ConsumerTests.Consumer.MVC
{
    public class ConsumerMVCPactFixture : IDisposable
    {
        private readonly IWebHost _providerWebHost;
        private readonly IWebHost _pactVerifierWebHost;

        public const string Name = "Consumer MVC pacts";
        public string ProviderUri => "http://localhost:5000";
        public string PactVerifierUri => "http://localhost:5001";
        public string PactBrokerUri => "http://13.80.68.171";
        public IPactVerifier PactVerifier { get; private set; }

        public ConsumerMVCPactFixture()
        {
            _providerWebHost = WebHost.CreateDefaultBuilder()
                .UseUrls(ProviderUri)
                .UseStartup<TestStartup>()
                .ConfigureServices(services =>
                {
                    // !! adding mocked responses
                    services.AddSingleton<INhtsaHttpClient, HttpClientMock>();
                })
                .UseIISIntegration()
                .Build();
            _providerWebHost.RunAsync();

            _pactVerifierWebHost = WebHost.CreateDefaultBuilder()
                .UseUrls(PactVerifierUri)
                .UseStartup<PactStateStartup>()
                .Build();
            _pactVerifierWebHost.Start();
        }

        internal IPactVerifier SetPactVerifier(ITestOutputHelper output)
        {
            var config = new PactVerifierConfig
            {
                // default : ConsoleOutput,however xUnit 2 does not capture
                // the console output, so a custom outputter is required.
                Outputters = new[] { new XUnitOutput(output) },
                Verbose = true,
                ProviderVersion = "8",
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
                    _pactVerifierWebHost.StopAsync().GetAwaiter().GetResult();
                    _pactVerifierWebHost.Dispose();
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