namespace MES.Application.DTOs.MES.XKLXH
{
    public class GetDataByODODItemResponse
    {
        //Ship to party name
        public string ShipToPartyName { get; set; }
        //Material
        public string Material { get; set; }
        //Material desc
        public string MaterialName { get; set; }
        //Batch
        public string Batch { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Total Qty
        public decimal? TotalQty { get; set; }
        //Delivery Qty
        public decimal? DeliveryQty { get; set; }
        //Open Qty
        public decimal? OpenQty => TotalQty - DeliveryQty;
        //Document Date
        public DateTime? DocumentDate { get; set; }
    }
}
