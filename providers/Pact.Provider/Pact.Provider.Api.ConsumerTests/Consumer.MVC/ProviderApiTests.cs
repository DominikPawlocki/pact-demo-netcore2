using Pact.Provider.Api.ConsumerTests.xUnit;
using PactNet.Infrastructure.Outputters;
using PactNet;
using PactNet.Verifier;
using System.Collections.Generic;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Pact.Provider.Api.ConsumerTests.Consumer.MVC
{
    //[Collection(ConsumerMVCPactFixture.Name)]

    public class ProviderApiTests : IClassFixture<ProviderFixture>
    {
        private readonly ProviderFixture fixture;
        private readonly ITestOutputHelper output;

        public ProviderApiTests(ProviderFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
        }

        [Fact]
        public void VerifyLatestPacts()
        {
            string version = Environment.GetEnvironmentVariable("VERSION");
            string branch = Environment.GetEnvironmentVariable("BRANCH");
            string buildUri = Environment.GetEnvironmentVariable("BUILD_URL");

            var verifier = new PactVerifier("My Provider", new PactVerifierConfig
            {
                LogLevel = PactLogLevel.Information,
                Outputters = new List<IOutput>
            {
                new XUnitOutput(this.output)
            }
            });

            verifier.WithHttpEndpoint(this.fixture.ServerUri)
                    .WithPactBrokerSource(new Uri("https://broker.example.org"), options =>
                    {
                        options.ConsumerVersionSelectors(new ConsumerVersionSelector { MainBranch = true, Latest = true })
                               .PublishResults(version, results =>
                               {
                                   results.ProviderBranch(branch)
                                          .BuildUri(new Uri(buildUri));
                               });
                    })
                    .WithProviderStateUrl(new Uri(this.fixture.ServerUri, "/provider-states"))
                    .Verify();
        }
    }
}








/*[Fact(DisplayName = "Models of 'tesla' returns data")]
public void Given_Tesla_When_Getting_Manufacturer_Models_Returns_Data()
{
    _pactVerifier
        .Verify("A GET request to provider/api/cars/manufacturers/tesla/models/2018");
}

[Fact(DisplayName = "Details of 'tesla' manufacturer returns data")]
public void Given_CompanyName_When_Getting_Manufacturer_Details_Returns_Data()
{
    _pactVerifier
        .Verify($"A GET request to retrieve provider/api/cars/manufacturers/tesla/details");
}

[Fact(DisplayName = "Details of 'fsoo' manufacturer returns 404")]
public void Given_NotExisting_When_Getting_Manufacturer_Details_Returns_404()
{
    _pactVerifier
        .Verify($"A GET request to retrieve provider/api/cars/manufacturers/fsoo/details");
}

[Fact(DisplayName = "Random manufacturers returns data with random header")]
public void When_Getting_Random20_Then_Returns_Data_With_Random_Header()
{
    _pactVerifier
        .Verify($"A GET request to retrieve provider/api/cars/manufacturers/random20");
}

[Fact(DisplayName = "Decoding '5UXWX7C5ABA' VIN's returns data")]
public void Given_5UXWX7C5ABA_When_DecodingVIN_Then_Returns_Data()
{
    _pactVerifier
        .ProviderState($"{_fixture.PactVerifierStatesUri}/provider-states")
        .Verify("A GET request to provider/api/cars/vin/5UXWX7C5ABA", "5UXWX7C5ABA");
}

[Fact(DisplayName = "Decoding 'some_wrong VIN' returns data")]
public void Given_SomeWrongVIN_When_DecodingVIN_Then_Returns_Data()
{
    _pactVerifier
        .ProviderState($"{_fixture.PactVerifierStatesUri}/provider-states")
        .Verify("A GET request to provider/api/cars/vin/some_wrong_vin", "some_wrong_vin");
}

[Fact(DisplayName = "Creating new car with 'a_new_vin' returns data")]
public void Given_A_new_vin_When_Upserting_Car_Then_Returns_Data()
{
    _pactVerifier
        .ProviderState($"{_fixture.PactVerifierStatesUri}/provider-states")
        .Verify("A POST request to provider/api/cars/vin", "some_wrong_vin");
}*/
