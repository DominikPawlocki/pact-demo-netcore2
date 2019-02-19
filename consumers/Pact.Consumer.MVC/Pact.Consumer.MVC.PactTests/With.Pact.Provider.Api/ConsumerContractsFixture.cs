using PactNet;
using PactNet.Mocks.MockHttpService;
using System;
using System.Net;
using System.Net.Sockets;

namespace Pact.Consumer.MVC.PactTests.With.Pact.Provider.Api
{
    public class ConsumerContractsFixture : IDisposable
    {
        public IPactBuilder PactBuilder { get; private set; }
        public IMockProviderService MockProviderService { get; }
        public int MockServerPort { get; private set; }
        public string MockProviderServiceBaseUri => String.Format("http://localhost:{0}/", MockServerPort);
        public string SuccessMessage => "Results returned successfully";

        public const string CollectionName = "With.Pact.Provider.Api";

        public ConsumerContractsFixture()
        {
            // Using Spec version 2.0.0 more details at https://goo.gl/UrBSRc
            var pactConfig = new PactConfig
            {
                SpecificationVersion = "2.0.0",
                PactDir = @"..\..\pacts",
                LogDir = @"..\..\pact_logs"
            };

            PactBuilder = new PactBuilder(pactConfig);

            PactBuilder.ServiceConsumer("Pact.Consumer.MVC").HasPactWith("Pact.Provider.Api");

            MockServerPort = GetNextFreePort();
            MockProviderService = PactBuilder.MockService(MockServerPort, false, PactNet.Models.IPAddress.Loopback);
        }

        private static int GetNextFreePort()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(new IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 0));
            int port = ((IPEndPoint)sock.LocalEndPoint).Port;
            sock.Dispose();
            return port;
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // This will save the pact file once finished.
                    PactBuilder.Build();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}