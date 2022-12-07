namespace ISD.API.Applications.DTOs.WorkOrder
{
    public class GetInfoSaveBarcodeResponse
    {
        public string WorkOderCode { get; set; }
        public string ProductName { get; set; }
        public int? QuantityPrinted { get; set; }
        public decimal? Quantity { get; set; }
    }
}
