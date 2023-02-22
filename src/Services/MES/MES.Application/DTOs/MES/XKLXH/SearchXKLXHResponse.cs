using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.XKLXH
{
    public class GetInputDataResponse
    {
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
        public string SlocDesc { get; set; }
        public string SlocFmt { get; set; }
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
        //Material
        //Material desc
        //Sloc
        //Batch
        //SL bao
        //Đơn trọng
        //Đầu cân
        //Confirm Qty
        //Sl kèm bao bì
        //Số phương tiện
        //Số lần cân
        //Total quantity
        //Delivered Quantity
        //Open quantity
        //UOM
        //Ghi chú
        //Hình ảnh
        //Status
        //Số phiếu cân
        //Thời gian bắt đầu
        //Thời gian kết thúc
        //Document Date
        //Số xe tải
        //Số cân đầu vào
        //Số cân đầu ra
        //Trọng lượng hàng hóa
        //Create by
        //Create on
        //Change by
        //Material Doc
        //Reverse Doc
        //Dánh dấu xóa
        //Có thể chỉnh sửa
    }
}
