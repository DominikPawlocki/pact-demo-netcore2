using PactNet;
using Xunit;
using Xunit.Abstractions;

namespace Pact.Provider.Api.ConsumerTests.Consumer.MVC
{
    [Collection(ConsumerMVCPactFixture.Name)]
    public class ProviderApiTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ConsumerMVCPactFixture _fixture;
        private readonly IPactVerifier _pactVerifier;

        public ProviderApiTests(ITestOutputHelper output, ConsumerMVCPactFixture fixture)
        {
            _output = output;
            _fixture = fixture;
            _pactVerifier = _fixture.SetPactVerifier(output);
        }

        [Fact(DisplayName = "Models of 'tesla' returns data")]
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
                // .Verify($"A GET request to retrieve provider/api/cars/manufacturers/fsoo/details");
                .Verify();
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
    }
}