namespace Pact.Provider.Api.Models
{
    public partial class NhtsaManufacturersResponce : NhtsaBaseResponse 
    {
        public ManufacturerResult[] Results { get; set; }
    }

    public partial class ManufacturerResult
    {
        public string Country { get; set; }
        public string Mfr_CommonName { get; set; }
        public int Mfr_ID { get; set; }
        public string Mfr_Name { get; set; }
        public VehicleType[] VehicleTypes { get; set; }
    }

    public partial class VehicleType
    {
        public bool IsPrimary { get; set; }
        public string Name { get; set; }
    }

    public enum Name { Bus, IncompleteVehicle, LowSpeedVehicleLsv, Motorcycle, MultipurposePassengerVehicleMpv, OffRoadVehicle, PassengerCar, Trailer, Truck };
}
