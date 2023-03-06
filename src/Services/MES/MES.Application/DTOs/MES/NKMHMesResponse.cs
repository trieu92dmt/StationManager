namespace MES.Application.DTOs.MES
{
    public class NKMHMesResponse
    {
        public List<PuchaseOrderNKMHResponse> PuchaseOrderNKMHs { get; set; } = new List<PuchaseOrderNKMHResponse>();
        public List<ListNKMHResponse> ListNKMHs { get; set; } = new List<ListNKMHResponse>();

    }
    public class PuchaseOrderNKMHResponse
    {
        //Id
        public Guid Id { get; set; }
        //PO
        public Guid PoDetailId { get; set; }
        public string Plant { get; set; }
        public string PurchaseOrderCode { get; set; }
        public string POItem { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string Material { get; set; }
        public string MaterialName { get; set; }
        public string Batch { get; set; }
        //Số lần cân
        public int QuantityWeight { get; set; }
        public string VehicleCode { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? OpenQuantity { get; set; }
        public decimal? DeliveredQuantity { get; set; }
        public string StorageLocation { get; set; }
        public string Unit { get; set; }
        public string WeightHeadCode { get; set; }
    }

    public class ListNKMHResponse
    {
        //PO
        public Guid NkmhId { get; set; }
        public string Plant { get; set; }
        public string PurchaseOrderCode { get; set; }
        //Số phiếu cân
        public string WeightVote { get; set; }
        public string WeightId { get; set; }
        public string POType { get; set; }
        public string PurchasingOrg { get; set; }
        public string PurchasingGroup { get; set; }
        public string VendorCode { get; set; }
        public string Material { get; set; }
        public string MaterialName { get; set; }
        public DateTime? DocumentDate { get; set; }
        //POitem
        public string POItem { get; set; }
        public string SlocCode { get; set; }
        public string StorageLocation { get; set; }
        public string VehicleCode { get; set; }
        public decimal? OpenQuantity { get; set; }
        public string Unit { get; set; }
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
        public Guid? TruckInfoId { get; set; }
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
        public Guid? CreateById { get; set; }
        public DateTime? LastEditTime { get; set; }
        public string LastEditBy { get; set; }
        public Guid? LastEditById { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
        public string Batch { get; set; }
        public string VendorName { get; set; }
        public bool isDelete { get; set; }
    }
}
