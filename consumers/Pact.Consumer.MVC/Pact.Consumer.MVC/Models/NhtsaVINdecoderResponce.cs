namespace Pact.Consumer.MVC.Models {
    // https://vpic.nhtsa.dot.gov/api/vehicles/decodevinvalues/5UXWX7C5*BA?format=json&modelyear=2011
    // 1HGCM82633A004352
    // 19UYA42601A019296
    public class NhtsaVINdecoderResponce {
        public int Count { get; set; }
        public string Message { get; set; }
        public object SearchCriteria { get; set; }
        public CarDetails[] Results { get; set; }
    }
    public class CarDetails {
        public string AdditionalErrorText { get; set; }
        public string ErrorCode { get; set; }
        public string EngineCylinders { get; set; }
        public double EngineKW { get; set; }
        public string FuelTypePrimary { get; set; }
        public string FuelTypeSecondary { get; set; }
        public string Make { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int ModelYear { get; set; }
        public string PlantCity { get; set; }
        public string PlantCountry { get; set; }
        public string PlantState { get; set; }
        public string VehicleType { get; set; }
    }

}