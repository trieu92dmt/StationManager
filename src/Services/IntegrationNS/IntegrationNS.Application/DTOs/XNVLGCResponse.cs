using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.DTOs
{
    public class XNVLGCResponse
    {
        //Id
        public Guid XNVLGCId { get; set; }
        //Plant
        public string Plant { get; set; }
        public string PlantName { get; set; }
        //PO
        public string PurchaseOrder { get; set; }
        //PO Item
        public string PurchaseOrderItem { get; set; }
        //Material
        public string Material { get; set; }
        //Material desc
        public string MaterialDesc { get; set; }
        //Component
        public string Component { get; set; }
        //Component desc
        public string ComponentDesc { get; set; }
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
        //Confirm quantity
        public decimal? ConfirmQty { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public int? QuantityWeight { get; set; }
        //Order quantity
        public decimal? OrderQuantity { get; set; }
        //Order unit
        public string OrderUnit { get; set; }
        //Requirement quantity
        public decimal? RequirementQuantity { get; set; }
        //Requirement Unit
        public string RequirementUnit { get; set; }
        //Document date
        public DateTime? DocumentDate { get; set; }
        //Vendor name
        public string Vendor { get; set; }
        public string VendorName { get; set; }
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
        //Crete On
        public DateTime? CreateOn { get; set; }
        //Change by
        public Guid? ChangeById { get; set; }
        public string ChangeBy { get; set; }
        public DateTime? ChangeOn { get; set; }
        //Material Doc
        public string MaterialDoc { get; set; }
        //Reverse Doc
        public string ReverseDoc { get; set; }
        //Đánh dấu xóa
        public bool? isDelete { get; set; }
    }
}
