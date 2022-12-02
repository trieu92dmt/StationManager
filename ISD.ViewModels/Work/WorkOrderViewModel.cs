using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class WorkOrderViewModel
    {
        public List<WorkOrder_Mold_Mapping> WorkOrder_Mold_Mapping { get; set; }
        public List<WorkOrder_Product_Mapping> WorkOrder_Product_Mapping { get; set; }
        //ID
        public System.Guid WorkOrderId { get; set; }
        public Nullable<int> WorkOrderIdInt { get; set; }

        //STT
        public int STT { get; set; }

        //Ưu tiên 
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_PriorityCode")]
        public int? Priority { get; set; }

        //Lệnh sản xuất tổng hợp 
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_ParentWorkOrderCode")]
        public string ParentWorkOrderCode { get; set; }

        //Lệnh sản xuất 
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_WorkOrderCode")]
        public string WorkOrderCode { get; set; }

        //Mã TP/BTP
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_ProductCode")]
        public string ProductCode { get; set; }

        //Tên TP/BTP
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_ProductName")]
        public string ProductName { get; set; }

        //BOM Version
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_BOMVersion")]
        public string BOMVersion { get; set; }

        //Số lượng phát lệnh
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_WorkOrderQty")]
        public Nullable<decimal> WorkOrderQty { get; set; }

        //Đơn vị phát lệnh 
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_WorkOrderUnit")]
        public string WorkOrderUnit { get; set; }

        //Ngày phát lệnh
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_WorkOrderDate")]
        public Nullable<System.DateTime> DocumentDate { get; set; }

        //Ngày bắt đầu sx dự kiến
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_ReceiveDate")]
        public Nullable<System.DateTime> EstimateFromDate { get; set; }

        //Ngày kết thúc sx dự kiến 
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_EstimateDate")]
        public Nullable<System.DateTime> EstimateToDate { get; set; }

        //Ngày bắt đầu sx thực tế
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_StartDate")]
        public Nullable<System.DateTime> ActualStartDate { get; set; }

        //Ngày kết thúc sx thực tế
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_EndDate")]
        public DateTime? ActualEndDate { get; set; }

        //Mã khách hàng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer_CustomerCode")]
        public string CustomerCode { get; set; }

        //Tên khách hàng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer_CustomerName")]
        public string CustomerName { get; set; }

        //Số PO/Hợp đồng/Báo giá
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_PONumber")]
        public string PONumber { get; set; }

        //Số lượng SP đặt hàng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_SO_Quantity")]
        public decimal? SO_Quantity { get; set; }

        //Số lượng giao mẫu
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_SO_DeliveryQuantity")]
        public decimal? SO_DeliveryQuantity { get; set; }

        //ĐVT
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LXS_SO_Unit")]
        public string SO_Unit { get; set; }

        //Phiếu tiếp nhận thông tin
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LXS_SO_InforCard")]
        public string SO_InforCard { get; set; }

        //Ngày giao hàng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_SO_DeliveryDate")]
        public Nullable<System.DateTime> SO_DeliveryDate { get; set; }

        //Kích thước SP(mm)
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_ProductSize")]
        public string ProductSize { get; set; }

        //Kích thước khổ trải SP(mm)
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_ProductSpreadSize")]
        public string ProductSpreadSize { get; set; }

        //Số SP/Khổ in(hoặc khổ cắt)
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_ProductPerPrintSize")]
        public Nullable<int> ProductPerPrintSize { get; set; }

        //Đơn vị gia công TP
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_ProcessingUnit")]
        public string ProcessingUnit { get; set; }

        //Tình trạng SP
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_SO_StatusProduct")]
        public string SO_StatusProduct { get; set; }

        //Mô tả SP
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_SO_Description")]
        public string SO_Description { get; set; }

        //Mã khuôn
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_SO_MoldCode")]
        public List<string> SO_MoldCode { get; set; }

        //Tên khuôn 
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_SO_MoldName")]
        public List<string> SO_MoldName { get; set; }

        //Số serial
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_SerialNumber")]
        public List<string> SerialNumber { get; set; }

        //Số con/tờ
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_ProductPerPage")]
        public Nullable<int> ProductPerPage { get; set; }

        //Số phiếu thiết kế
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_DesignVote")]
        public string DesignVote { get; set; }

        //Phụ trách thiết kế
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_DesignBy")]
        public string DesignBy { get; set; }

        //Mã bản vẽ Marquette
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_MarquetteDrawingCode")]
        public string MarquetteDrawingCode { get; set; }
        //Mã bản vẽ khuôn
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_MoldDrawingCode")]
        public string MoldDrawingCode { get; set; }
        //Ghi chú mẫu kèm theo
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_AttachedNoteForm")]
        public string AttachedNoteForm { get; set; }
        //Created By
        public string CreatedBy { get; set; }

        //Ghép bài 
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_MatchCard")]
        public string MatchCard { get; set; }

        //Tên hạng mục in
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_PrintItem")]
        public string PrintItem { get; set; }

        //Kiểu in
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_PrintStyle")]
        public string PrintStyle { get; set; }

        //Công ty gia công in
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_PrintCompany")]
        public string PrintCompany { get; set; }

        //NCC kẽm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_ZincSupplier")]
        public string ZincSupplier { get; set; }

        //Ngày yêu cầu có hàng in
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_PrintReqDate")]
        public Nullable<System.DateTime> PrintReqDate { get; set; }

        //Trạng thái
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_PrintStatus")]
        public string PrintStatus { get; set; }

        //Component
        public List<string> Component { get; set; }

        //Description
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_ProductName")]
        public string LSX_ProductName { get; set; }

        //Số lượng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_Quantity")]
        public decimal? Component_Quantity { get; set; }

        //Component Unit
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_Unit")]
        public string Component_Unit { get; set; }

        //Mã Marquette
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_MarquetteCode")]
        public List<string> Component_MarquetteCode { get; set; }

        //Khổ nguyên(mm)
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_Size")]
        public decimal? Component_Size { get; set; }
        //Khổ in(mm)
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_SizePrint")]
        public decimal? Component_SizePrint { get; set; }

        //Số lượng NVL xuất kho
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_QuantityExport")]
        public decimal? Component_QuantityExport { get; set; }

        //Số lượng tờ in (tờ cắt ) (được xả từ tờ nguyên)
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_QuantityPaperCut")]
        public decimal? Component_QuantityPaperCut { get; set; }

        //Số lượng tờ in ( tờ cắt) cần đạt tối thiểu
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_QuantityPaperCutMin")]
        public decimal? Component_QuantityPaperCutMin { get; set; }

        //Thông số màu
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_ColorParam")]
        public List<string> Component_ColorParam { get; set; }

        //Mã màu pha
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_ColorCode")]
        public List<string> Component_ColorCode { get; set; }

        //Quy cách in
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_PrintSpecifications")]
        public string Component_PrintSpecifications { get; set; }

        //Khổ kẽm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_ZincSize")]
        public string Component_ZincSize { get; set; }

        //Số kẽm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_ZincNumber")]
        public decimal? Component_ZincNumber { get; set; }

        //Ghi chú
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component_Note")]
        public string Component_Note { get; set; }
        //Gia công ngoài
        //public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> LastEditBy { get; set; }
        public Nullable<System.DateTime> LastEditTime { get; set; }
        public Nullable<bool> Actived { get; set; }

        //Search
        //Cần cập nhật số serial
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isUpdateSerial")]
        public bool? IsUpdateSerial { get; set; }
        //Ngày bắt đầu SX dự kiến
        //Từ ngày 
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkingDate_FromDate")]
        public DateTime? FromDate { get; set; }
        //Đến ngày
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkingDate_ToDate")]
        public DateTime? ToDate { get; set; }
    }
    public class WorkOrderDetailViewModel
    {
       public Guid WorkOrderId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepCode")]
        public string StepCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepName")]
        public string StepName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductionGuide")]
        public string ProductionGuide    { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_WorkOrderUnit")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? ProductPerPage { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductByStep")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? ProductByStep  { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Unit")]
        public string Unit { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalProduct")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? TotalProduct  { get; set; }
        public int OrderIndex  { get; set; }
    }
}
