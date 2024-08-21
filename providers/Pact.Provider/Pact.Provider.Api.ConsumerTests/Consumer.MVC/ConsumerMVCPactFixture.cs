using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Pact.Provider.Api.ConsumerTests.xUnit;
using Pact.Provider.Api.Services;
using PactNet.Verifier;
using Microsoft.Extensions.Hosting;

namespace Pact.Provider.Api.ConsumerTests.Consumer.MVC
{
    // A test fixture ensures the API is started once and stopped at the end of the test run
    public class ProviderFixture : IDisposable
    {
        private readonly IHost server;
        public Uri ServerUri { get; }
        public string PactBrokerUri => "http://13.80.68.171";

        public ProviderFixture()
        {
            this.ServerUri = new Uri("http://localhost:9222");

            this.server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(this.ServerUri.ToString());
                                  //webBuilder.UseStartup<TestStartup>();
                                  webBuilder.ConfigureServices(services =>
                                    {
                                        // !! adding mocked responses
                                        services.AddSingleton<INhtsaHttpClient, HttpClientMock>();
                                    });
                              })
                              .Build();

            this.server.Start();
        }

        public void Dispose()
        {
            this.server.Dispose();
        }
    }


    //public class ConsumerMVCPactFixture : IDisposable
    //{
    //    private readonly IWebHost _providerWebHost;
    //    private readonly IWebHost _pactVerifierWebHost;

    //    public const string Name = "Consumer MVC pacts";
    //    public string ProviderUri => "http://localhost:5000";
    //    public string PactVerifierUri => "http://localhost:5001";
    //    public IPactVerifier PactVerifier { get; private set; }

    //    public ConsumerMVCPactFixture()
    //    {
    //        _providerWebHost = WebHost.CreateDefaultBuilder()
    //            .UseUrls(ProviderUri)
    //            .UseStartup<TestStartup>()
    //            .ConfigureServices(services =>
    //            {
    //                // !! adding mocked responses
    //                services.AddSingleton<INhtsaHttpClient, HttpClientMock>();
    //            })
    //            .UseIISIntegration()
    //            .Build();
    //        _providerWebHost.RunAsync();

    //        _pactVerifierWebHost = WebHost.CreateDefaultBuilder()
    //            .UseUrls(PactVerifierUri)
    //            .UseStartup<PactStateStartup>()
    //            .Build();
    //        _pactVerifierWebHost.Start();
    //    }

    //    internal IPactVerifier SetPactVerifier(ITestOutputHelper output)
    //    {
    //        var config = new PactVerifierConfig
    //        {
    //            // default : ConsoleOutput,however xUnit 2 does not capture
    //            // the console output, so a custom outputter is required.
    //            Outputters = new[] { new XUnitOutput(output) },
    //            Verbose = true,
    //            ProviderVersion = "8",
    //            PublishVerificationResults = true
    //        };

    //        PactVerifier = new PactVerifier(config);
    //        PactVerifier
    //            .ServiceProvider("Pact.Provider.Api", ProviderUri)
    //            .HonoursPactWith("Pact.Consumer.MVC")
    //            //.PactUri(@"..\..\..\..\..\pacts\consumer-provider.json")
    //            .PactUri($"{PactBrokerUri}/pacts/provider/Pact.Provider.Api/consumer/Pact.Consumer.MVC/latest");
    //        return PactVerifier;
    //    }

    //    #region IDisposable Support

    //    private bool disposedValue = false;

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (!disposedValue)
    //        {
    //            if (disposing)
    //            {
    //                _providerWebHost.StopAsync().GetAwaiter().GetResult();
    //                _providerWebHost.Dispose();
    //                _pactVerifierWebHost.StopAsync().GetAwaiter().GetResult();
    //                _pactVerifierWebHost.Dispose();
    //            }
    //            disposedValue = true;
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        Dispose(true);
    //    }
    //    #endregion
    //}
}