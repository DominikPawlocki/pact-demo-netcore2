namespace Pact.Provider.Api.Models {
    //  https://vpic.nhtsa.dot.gov/api/vehicles/getmodelsformakeyear/make/honda/modelyear/2018?format=json
    //  https://vpic.nhtsa.dot.gov/api//vehicles/GetModelsForMake/honda?format=json
    public partial class NhtsaCarModelResponce : NhtsaBaseResponse 
    {
        public ModelResult[] Results { get; set; }
    }

    public class ModelResult {
        public int Make_ID { get; set; }
        public string Make_Name { get; set; }
        public int Model_ID { get; set; }
        public string Model_Name { get; set; }
    }
}