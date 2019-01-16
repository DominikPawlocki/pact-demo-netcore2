namespace Pact.Consumer.MVC.Models {
    public partial class NhtsaCarModelResponce 
    {
        public int Count { get; set; }
        public string Message { get; set; }
        public object SearchCriteria { get; set; }
        public ModelResult[] Results { get; set; }
    }

    public class ModelResult {
        public int Make_ID { get; set; }
        public string Make_Name { get; set; }
        public int Model_ID { get; set; }
        public string Model_Name { get; set; }
    }
}