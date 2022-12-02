using System;

namespace ISD.ViewModels
{
    public class AuthenticateResultViewModel
    {
        public AuthenticateDataViewModel Data { get; set; }
    }

    public class SaveBarcodeManualDataViewModel
    {
        public Guid rawMaterialCardManualId { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public string batch { get; set; }
        public string barcodePath { get; set; }
    }
    public class AuthenticateDataViewModel
    {
        public string token { get; set; }
    }
}
