using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pact.Consumer.MVC.PactTests
{
    public class PactServerErrorResponse
    {
        public string Message { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<object> interaction_diffs { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Backtrace { get; set; }
    }
}