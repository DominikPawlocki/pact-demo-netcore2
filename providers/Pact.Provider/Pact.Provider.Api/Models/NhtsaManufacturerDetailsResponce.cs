using System;

namespace Pact.Provider.Api.Models {
    //https://vpic.nhtsa.dot.gov/api/vehicles/getmanufacturerdetails/honda?format=json
    public class NhtsaManufacturerDetailsResponce : NhtsaBaseResponse 
    {
        public ManufacturerDetailsResult[] Results { get; set; }
    }

    public class ManufacturerDetailsResult {
        public string Address { get; set; }
        public string City { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Country { get; set; }
        public string DBAs { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Mfr_CommonName { get; set; }
        public int Mfr_ID { get; set; }
        public string Mfr_Name { get; set; }
        public string PostalCode { get; set; }
        public string PrincipalFirstName { get; set; }
        public object PrincipalLastName { get; set; }
        public string PrincipalPosition { get; set; }
        public string StateProvince { get; set; }
        public string SubmittedName { get; set; }
        public string SubmittedPosition { get; set; }
    }
}