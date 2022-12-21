namespace ISD.API.Applications.DTOs.IntegrationNS
{
    public class NKMHResponse
    {
        //PO
        public string Plant { get; set; }
        public string PurchaseOrderCode { get; set; }
        public string POType { get; set; }
        public string PurchasingOrg { get; set; }
        public string PurchasingGroup { get; set; }
        public string VendorCode { get; set; }
        public string ProductCode { get; set; }
        public DateTime? DocumentDate { get; set; }
        //POitem
        public string POLine { get; set; }
        public string StorageLocation { get; set; }
        public string Batch { get; set; }
        public string VehicleCode { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? OpenQuantity { get; set; }
        //NKMH
        public decimal? BagQuantity { get; set; }
        public decimal? SingleWeight { get; set; }
        public string WeightHeadCode { get; set; }
        public decimal? Weight { get; set; }
        public decimal? ConfirmQty { get; set; }
        public decimal? QuantityWithPackaging { get; set; }
        public decimal? QuantityWeitght { get; set; }
        public decimal? TotalQuantity { get; set; }
        public decimal? DeliveredQuantity { get; set; }
        public int? TruckQuantity { get; set; }
        public decimal? InputWeight { get; set; }
        public decimal? OutputWeight { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public string QuantityWeitghtVote { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastEditTime { get; set; }
        public string LastEditBy { get; set; }
    }
}
