using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.XCK
{
    public class SearchXCKResponse
    {
        //Id
        public Guid XCKId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Reservation
        public string Reservation { get; set; }
        //Reservation Item
        public string ReservationItem { get; set; }
        //Material
        public string Material { get; set; }
        //MaterialDesc
        public string MaterialDesc { get; set; }
        //MVT
        public string MVT { get; set; }
        //Stor.Sloc
        public string MovementType { get; set; }
        //Receiving Stor.Sloc
        public string ReceivingSloc { get; set; }
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
        public int QuantityWeight { get; set; }
        //Total Quantity
        public decimal? TotalQty { get; set; }
        //Delivered Quantity
        public decimal? DeliveredQty { get; set; }
        //Open Quantity
        public decimal? OpenQty { get; set; }
        //UoM
        public string Unit { get; set; }
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
        //Create by
        public Guid? CreateBy { get; set; }
        //Create on
        public DateTime? CreateOn { get; set; }
        //Change by
        public Guid? ChangeBy { get; set; }
        //Material doc
        public string MatDoc { get; set; }
        //Reverse doc
        public string RevDoc { get; set; }
        //Đánh dấu xóa
        public bool isDelete { get; set; }
        //Được chỉnh sửa
        public bool isEdit { get; set; }
    }

    public class GetInputDataResponse
    {
        //1. Plant
        public string Plant { get; set; }
        public string PlantName { get; set; }
        //2. Reservation
        public string Reservation { get; set; }
        //3. Reservation Item
        public string ReservationItem { get; set; }
        //4. Material
        public string Material { get; set; }
        //5. Material Desc
        public string MaterialDesc { get; set; }
        //6. Movement Type
        public string MovementType { get; set; }
        //7. Stor.Loc
        public string Sloc { get; set; }
        //8. Receving Sloc
        public string ReceivingSloc { get; set; }
        //9. Batch
        public string Batch { get; set; }
        //10. Total Quantity
        public decimal? TotalQty { get; set; }
        //11. Delivered Quantity
        public decimal? DeliveredQty { get; set; }
        //12. Open Quantity
        public decimal? OpenQty { get; set; }
        //13. UoM
        public string Unit { get; set; }
    }
}
