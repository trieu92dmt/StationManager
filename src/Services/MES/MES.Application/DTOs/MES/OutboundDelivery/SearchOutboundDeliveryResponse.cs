using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.OutboundDelivery
{
    //Bảng 1
    public class OutboundDeliveryResponse
    {
        //Plant
        public string Plant { get; set; }
        //Ship-to party name
        public string ShipToPartyName { get; set; }
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
        //Batch
        public string Batch { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Total Quantity
        public decimal? TotalQty { get; set; }
        //Delivered Quantity
        public decimal? DeliveryQty { get; set; }
        //Units of Measure
        public string Unit { get; set; }
        //Document Date
        public DateTime? DocumentDate { get; set; }
        //Ship to Party
        public string ShipToParty { get; set; }
    }

    //Bảng 2
    public class GoodsReturnResponse
    {
        //Id
        public Guid GoodsReturnId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Ship-to party
        public string ShipToPartyName { get; set; }
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
        //Batch
        public string Batch { get; set; }
        //SL bao
        public decimal? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm Quantity
        public decimal? ConfỉmQty { get; set; }
        //SL kèm bao bì
        public decimal? QtyWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public int QtyWeight { get; set; }
        //Total Quantity
        public decimal? TotalQty { get; set; }
        //Delivered Quantity
        public decimal? DeliveryQty { get; set; }
        //Open Quantity
        public decimal? OpenQty { get; set; }
        //UoM
        public string UOM { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        //Status
        public string Status { get; set; }
        //Số phiếu cân
        public string WeightVote { get; set; }
        //Thời gian bắt đầu
        public DateTime StartTime { get; set; }
        //Thời gian kết thúc
        public DateTime EndTime { get; set; }
        //Document Date
        public DateTime DocumentDate { get; set; }
        //Create by
        public string CreateBy { get; set; }
        //Create On
        public string CreateOn { get; set; }
        //Chamge By
        public string ChangeBy { get; set; }
        //Material Doc
        public string MatDoc { get; set; }
        //Reverse Doc
        public string ReverseDoc { get; set; }
    }    
}
