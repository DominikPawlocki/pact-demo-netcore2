using PactNet.Infrastructure.Outputters;
using Xunit.Abstractions;

namespace Pact.Provider.PactProviderTests.Setup
{
    public class XunitOutput : IOutput
    {
        private readonly ITestOutputHelper output;

        /// <summary>
        /// Initialises a new instance of the <see cref="XunitOutput"/> class.
        /// </summary>
        /// <param name="output">xUnit test output helper</param>
        public XunitOutput(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// Write a line to the output
        /// </summary>
        /// <param name="line">Line to write</param>
        public void WriteLine(string line) => this.output.WriteLine(line);
    }
}
