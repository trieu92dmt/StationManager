namespace IntegrationNS.Application.DTOs
{
    public class PuchaseOrderNKMHResponse
    {
        public string PurchaseOrderCode { get; set; }
        public string POType { get; set; }
        public string Plant { get; set; }
        public string PurchasingOrg { get; set; }
        public string PurchasingGroup { get; set; }
        public string Vendor { get; set; }
        public string Material { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? CreateOn { get; set; }
        public DateTime? ChangeOn { get; set; }
        public List<DetailPuchaseOrderNKMHResponse> PODetails { get; set; } = new List<DetailPuchaseOrderNKMHResponse>();
    }

    public class DetailPuchaseOrderNKMHResponse
    {
        public string POLine { get; set; }
        public string StorageLocation { get; set; }
        public string Batch { get; set; }
        public string VehicleCode { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? OpenQuantity { get; set; }
        public string UoM { get; set; }
        public DateTime? CreateOn { get; set; }
        public DateTime? ChangeOn { get; set; }
    }
}
