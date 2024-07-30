using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace Pact.Provider.PactProviderTests.Setup
{
    // A test fixture ensures the API is started once and stopped at the end of the test run
    public class ProviderFixture : IDisposable
    {
        private readonly IHost server;
        public Uri ProviderServerUri { get; }

        public ProviderFixture()
        {
            this.ProviderServerUri = new Uri("https://localhost:5000");

            this.server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(this.ProviderServerUri.ToString());
                                  webBuilder.UseStartup<TestStartup>();
                              })
                              .Build();
            this.server.Start();
        }


        public void Dispose()
        {
            this.server.Dispose();
        }
    }
}
