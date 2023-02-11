using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.XTHLSX
{
    public class GetDataInputResponse
    {
        public int IndexKey { get; set; }
        //Plant
        public string Plant { get; set; }
        //Production Order
        public string WorkOrder { get; set; }
        public string Material { get; set; }
        //Material Desc
        public string MaterialDesc { get; set; }
        //Component
        public string Component { get; set; }
        //Component
        public string ComponentDesc { get; set; }
        //Sales Order
        public string SalesOrder { get; set; }
        //OrderType
        public string OrderType { get; set; }
        //Schedule Star Time
        public DateTime? ScheduleStartTime { get; set; }
        //Schedule Finish Time
        public DateTime? ScheduleFinishTime { get; set; }
        //Storage Location
        public string Sloc { get; set; }
        public string SlocName { get; set; }
        //Batch
        public string Batch { get; set; }
        //Requirement Qty
        public decimal? RequirementQty { get; set; }
        //Withdrawn Qty
        public decimal? WithdrawQty { get; set; }
        //UoM
        public string Unit { get; set; }
    }
    public class SearchXTHLSXResponse
    {
        //ID NKPPPP
        public Guid XTHLSXId { get; set; }
        //Plant
        public string Plant { get; set; }
        //Production Order
        public string WorkOrder { get; set; }
        //Material
        public string Material { get; set; }
        //Material Desc
        public string MaterialDesc { get; set; }
        //Component
        public string Component { get; set; }
        //Component desc
        public string ComponentDesc { get; set; }
        //Stor.Loc
        public string Sloc { get; set; }
        public string SlocName { get; set; }
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
        public decimal? ConfirmQuantity { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Số lượng yêu cầu
        public decimal? RequirementQty { get; set; }
        //Số lượng đã nhập thu hồi
        public decimal? WithdrawnQty { get; set; }
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
        //Create by
        public Guid? CreateById { get; set; }
        public string CreateBy { get; set; }
        //Crete On
        public DateTime? CreateOn { get; set; }
        //Change by
        public Guid? ChangeById { get; set; }
        public string ChangeBy { get; set; }
        //Material Doc
        public string MaterialDoc { get; set; }
        //Reverse Doc
        public string ReverseDoc { get; set; }
        //Schedule Star Time
        public DateTime? ScheduleStartTime { get; set; }
        //Schedule Finish Time
        public DateTime? ScheduleFinishTime { get; set; }
        //Đơn bán hàng
        public string SalesOrder { get; set; }
        //Đánh dấu xóa
        public bool? isDelete { get; set; }
        //Có thể chỉnh sửa
        public bool? isEdit { get; set; }
    }

}
