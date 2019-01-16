namespace Pact.Provider.Api.Models
{
    public class NhtsaBaseResponse
    {
        public long Count { get; set; }
        public string Message { get; set; }
        public object SearchCriteria { get; set; }
    }
}