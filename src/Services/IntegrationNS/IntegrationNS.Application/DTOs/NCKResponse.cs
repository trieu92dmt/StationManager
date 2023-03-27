using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.DTOs
{
    public class NCKResponse
    {
        //Id
        public Guid NCKId { get; set; }
        //Plant
        public string Plant { get; set; }

        //2. Material Doc
        public string MaterialDoc { get; set; }
        //3. Material Doc Item
        public string MaterialDocItem { get; set; }
        //Reservation
        public string Reservation { get; set; }
        //Material
        public string Material { get; set; }
        //MaterialDesc
        public string MaterialDesc { get; set; }
        //Stor.Sloc
        public string Sloc { get; set; }
        public string SlocName { get; set; }
        public string SlocFmt => !string.IsNullOrEmpty(Sloc) && !string.IsNullOrEmpty(SlocName) ? $"{Sloc} | {SlocName}" : "";
        //Batch
        public string Batch { get; set; }
        //Sl bao
        public decimal? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm Qty
        public decimal? ConfirmQty { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Total Quantity
        public decimal? TotalQty { get; set; }
        //Delivered Quantity
        public decimal? DeliveredQty { get; set; }
        //Open Quantity
        public decimal? OpenQty => TotalQty - DeliveredQty;
        //UoM
        public string Unit { get; set; }
        //Document date
        public DateTime? DocumentDate { get; set; }
        //Số xe tải
        public Guid? TruckInfoId { get; set; }
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
        //Create by
        public Guid? CreateById { get; set; }
        public string CreateBy { get; set; }
        //Create on
        public DateTime? CreateOn { get; set; }
        //Change by
        public Guid? ChangeById { get; set; }
        public string ChangeBy { get; set; }
        //Material doc
        public string MatDoc { get; set; }
        //Mat doc item
        public string MatDocItem { get; set; }
        //Reverse doc
        public string RevDoc { get; set; }
        //Đánh dấu xóa
        public bool isDelete { get; set; }
        //Được chỉnh sửa
        public bool isEdit { get; set; }
    }
}
