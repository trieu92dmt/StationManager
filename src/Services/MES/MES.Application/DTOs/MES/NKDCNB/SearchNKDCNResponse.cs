using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NKDCNB
{
    //Bảng 1
    public class GetInputDataResponse
    {
        public int IndexKey { get; set; }
        //Plant
        public string Plant { get; set; }
        //Shipping point
        public string ShippingPoint { get; set; }
        //Outbound Delivery
        public string OutboundDelivery { get; set; }
        //Outbound Delivery Item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //Material Description
        public string MaterialDesc { get; set; }
        //Storage Location
        public string Sloc { get; set; }
        //Storage Location Description
        public string SlocDesc { get; set; }
        //Sloc formate
        public string SlocFmt => !string.IsNullOrEmpty(Sloc) && !string.IsNullOrEmpty(SlocDesc) ? $"{Sloc} | {SlocDesc}" : "";
        //Batch
        public string Batch { get; set; }
        //Total Quantity
        public decimal? TotalQty { get; set; }
        //Delivered Quantity
        public decimal? DeliveryQty { get; set; }
        //Open Quantity
        public decimal? OpenQty => TotalQty.HasValue && DeliveryQty.HasValue ? TotalQty - DeliveryQty : 0;
        //Units of Measure
        public string Unit { get; set; }
        //Purchase order
        public string PurchasOrder { get; set; }
        //Document Date
        public DateTime? DocumentDate { get; set; }
    }

    //Bảng 2
    public class SearchNKDCNBResponse
    {
        //Id
        public Guid NKDCNBId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Shipping point
        public string ShippingPoint { get; set; }
        //Outbound Delivery
        public string OutboundDelivery { get; set; }
        //Outbound Delivery Item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //Material Desc
        public string MaterialDesc { get; set; }
        //Sloc
        public string Sloc { get; set; }
        public string SlocDesc { get; set; }
        public string SlocFmt => !string.IsNullOrEmpty(Sloc) && !string.IsNullOrEmpty(SlocDesc) ? $"{Sloc} | {SlocDesc}" : "";
        //Batch
        public string Batch { get; set; }
        //SL bao
        public decimal? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm Quantity
        public decimal? ConfirmQty { get; set; }
        //SL kèm bao bì
        public decimal? QtyWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public int? QtyWeight { get; set; }
        //Total Quantity
        public decimal? TotalQty { get; set; }
        //Delivered Quantity
        public decimal? DeliveryQty { get; set; }
        //Open Quantity
        public decimal? OpenQty => TotalQty - DeliveryQty;
        //UoM
        public string Unit { get; set; }
        //Purchase Order
        public string PurchaseOrder { get; set; }
        //Số xe tải
        public string TruckNumber { get; set; }
        //Số cân đầu vào
        public decimal? InputWeight { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
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
        //Create by
        public Guid? CreateById { get; set; }
        public string CreateBy { get; set; }
        //Create On
        public DateTime? CreateOn { get; set; }
        //Chamge By
        public Guid? ChangeById { get; set; }
        public string ChangeBy { get; set; }
        public DateTime? ChangeOn { get; set; }

        //Material Doc
        public string MatDoc { get; set; }
        //Reverse Doc
        public string RevDoc { get; set; }
        //Đánh dấu xóa
        public bool? isDelete { get; set; }
        //Có thể chỉnh sửa
        public bool isEdit { get; set; }
    }    
}
