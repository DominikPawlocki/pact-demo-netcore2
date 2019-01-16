namespace Pact.Consumer.MVC.Models {
    public partial class NhtsaManufacturersResponce {
        public int Count { get; set; }
        public string Message { get; set; }
        public object SearchCriteria { get; set; }
        public ManufacturerResult[] Results { get; set; }
    }

    public partial class ManufacturerResult {
        public string Country { get; set; }
        public string Mfr_CommonName { get; set; }
        public int Mfr_ID { get; set; }
        public string Mfr_Name { get; set; }
        public VehicleType[] VehicleTypes { get; set; }
    }

    public partial class VehicleType {
        public bool IsPrimary { get; set; }
        public string Name { get; set; }
    }

    public enum Name { Bus, IncompleteVehicle, LowSpeedVehicleLsv, Motorcycle, MultipurposePassengerVehicleMpv, OffRoadVehicle, PassengerCar, Trailer, Truck };
}