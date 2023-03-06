using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.XKLXH
{
    public class GetInputDataResponse
    {
        //Id
        public Guid Id { get; set; }
        //Index key
        public int IndexKey { get; set; }
        //Plant
        public string Plant { get; set; }
        public string PlantName { get; set; }
        //Ship to party name
        public string ShipToPartyName { get; set; }
        //Outbound delivery
        public string OutboundDelivery { get; set; }
        //Outbound delivery item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //Material desc
        public string MaterialDesc { get; set; }
        //Storage location
        public string Sloc { get; set; }
        //Storage location desc
        public string SlocName { get; set; }
        public string SlocFmt => !string.IsNullOrEmpty(Sloc) && !string.IsNullOrEmpty(SlocName) ? $"{Sloc} | {SlocName}" : "";
        //Batch
        public string Batch { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Total quantity
        public decimal? TotalQty { get; set; }
        //Delivered quantity
        public decimal? DeliveredQty { get; set; }
        //Open quantity
        public decimal? OpenQty => TotalQty - DeliveredQty;
        //UOM
        public string Unit { get; set; }
        //Document Date
        public DateTime? DocumentDate { get; set; }
        //ShipToParty
        public string ShipToParty { get; set; }
    }
    public class SearchXKLXHResponse
    {
        //XKLXH ID
        public Guid XKLXHId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Ship to party name
        public string ShipToPartyName { get; set; }
        //Outbound delivery
        public string OutboundDelivery { get; set; }
        //Outbound delivery item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //Material desc
        public string MaterialDesc { get; set; }
        //Sloc
        public string Sloc { get; set; }
        public string SlocName { get; set; }
        public string SlocFmt => !string.IsNullOrEmpty(Sloc) && !string.IsNullOrEmpty(SlocName) ? $"{Sloc} | {SlocName}" : "";
        //Batch
        public string Batch { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm Qty
        public decimal? ConfirmQty { get; set; }
        //Sl kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Total quantity
        public decimal? TotalQty { get; set; }
        //Delivered Quantity
        public decimal? DeliveredQty { get; set; }
        //Open quantity
        public decimal? OpenQty => TotalQty - DeliveredQty;
        //UOM
        public string Unit { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Status
        public string Status { get; set; }
        //Số phiếu cân
        public string WeightVote { get; set; }
        //Thời gian bắt đầu
        public DateTime? StartTime { get; set; }
        //Thời gian kết thúc
        public DateTime? EndTime { get; set; }
        //Document Date
        public DateTime? DocumentDate { get; set; }
        //Số xe tải
        public Guid? TruckInfoId { get; set; }
        public string TruckNumber { get; set; }
        //Số cân đầu vào
        public decimal? InputWeight { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Trọng lượng hàng hóa
        public decimal? GoodsWeight { get; set; }
        //Create by
        public int MyProperty { get; set; }
        //Create by
        public Guid? CreateById { get; set; }
        public string CreateBy { get; set; }
        //Create on
        public DateTime? CreateOn { get; set; }
        //Change by
        public Guid? ChangeById { get; set; }
        public string ChangeBy { get; set; }
        public DateTime? ChangeOn { get; set; }

        //Material doc
        public string MatDoc { get; set; }
        //Reverse doc
        public string RevDoc { get; set; }
        //Đánh dấu xóa
        public bool isDelete { get; set; }
        //Được chỉnh sửa
        public bool isEdit { get; set; }
    }
}
