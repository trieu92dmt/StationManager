using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.XK
{
    public class GetInputDataResponse
    {
        //IKey
        public int IndexKey { get; set; }
        //Plant
        public string Plant { get; set; }
        //Reservation
        public string Reservation { get; set; }
        //Reserbation Item
        public string ReservationItem { get; set; }
        //Material
        public string Material { get; set; }
        //Material Desc
        public string MaterialDesc { get; set; }
        //Movement Type
        public string MovementType { get; set; }
        //Customer
        public string Customer { get; set; }
        //Customer name
        public string CustomerName { get; set; }
        //Special Stock
        public string SpecialStock { get; set; }
        //Stor Sloc
        public string Sloc { get; set; }
        //Stor Sloc Fmt
        public string SlocFmt { get; set; }
        //Batch
        public string Batch { get; set; }
        //Total Quantity
        public decimal? TotalQuantity { get; set; }
        //Delivered Quantity
        public decimal? DeliveredQuantity { get; set; }
        //Open Quantity
        public decimal? OpenQuantity => TotalQuantity - DeliveredQuantity;
        //UoM
        public string Unit { get; set; }
    }
    public class SearchXKResponse
    {
        //ID
        public Guid XKId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Reservation
        public string Reservation { get; set; }
        //Reservation Item
        public string ReservationItem { get; set; }
        //Material
        public string Material { get; set; }
        //Material Desc
        public string MaterialDesc { get; set; }
        //MVT
        public string MovementType { get; set; }
        //Stor Sloc
        public string Sloc { get; set; }
        public string SlocFmt { get; set; }
        //Receiving Sloc
        public string ReceivingSloc { get; set; }
        public string ReceivingSlocFmt { get; set; }
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
        //Customer
        public string Customer { get; set; }
        //Customer name
        public string CustomerName { get; set; }
        //Special Stock
        public string SpecialStock { get; set; }
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
        public decimal? OpenQty { get; set; }
        //UoM
        public string Unit { get; set; }
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
        //Create On
        public DateTime? CreateOn { get; set; }
        //Chamge By
        public Guid? ChangeById { get; set; }
        public string ChangeBy { get; set; }
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
