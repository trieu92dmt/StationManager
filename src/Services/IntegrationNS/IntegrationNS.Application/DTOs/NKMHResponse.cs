namespace IntegrationNS.Application.DTOs
{
    public class NKMHResponse
    {
        //PO
        public Guid NkmhId { get; set; }
        public string Plant { get; set; }
        public string PurchaseOrderCode { get; set; }
        public string WeightVote { get; set; }
        public string WeightId { get; set; }
        public string POType { get; set; }
        public string PurchasingOrg { get; set; }
        public string PurchasingGroup { get; set; }
        public string VendorCode { get; set; }
        public string Material { get; set; }
        public DateTime? DocumentDate { get; set; }
        //POitem
        public string POItem { get; set; }
        public string StorageLocation { get; set; }
        public string Batch { get; set; }
        public string VehicleCode { get; set; }
        //Đơn vị vận tải
        public string TransportUnit { get; set; }
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
        public string TruckQuantity { get; set; }
        public decimal? InputWeight { get; set; }
        public decimal? OutputWeight { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastEditTime { get; set; }
        public string LastEditBy { get; set; }
        public DateTime? WeightDate { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }
}
