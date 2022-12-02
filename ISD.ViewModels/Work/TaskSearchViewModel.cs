using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class TaskSearchViewModel
    {
        //1. Mã yêu cầu
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaskCode")]
        public int? TaskCode { get; set; }

        //2. Yêu cầu
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_Summary")]
        public string Summary { get; set; }
        //Mô tả
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Description")]
        public string Description { get; set; }

        //3. Type: MyWork, MyFollow, TICKET, ACTIVITIES
        public string Type { get; set; }

        //4. Loại
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Type")]
        public Guid? WorkFlowId { get; set; }
        //chọn nhiều
        public List<Guid> WorkFlowIdList { get; set; }

        //5a. Trạng thái (Guid)
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaskStatus")]
        public string TaskStatusCode { get; set; }
        public List<string> TaskStatusCodeList { get; set; }

        //5b. Nhóm trạng thái (string)
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaskProcessCode")]
        public string TaskProcessCode { get; set; }

        //6. Người giao việc
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reporter")]
        //public string Reporter { get; set; }

        //7. Nhân viên được phân công
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignee")]
        public string Assignee { get; set; }
        public List<string> AssigneeList { get; set; }

        //8. Khách hàng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_CustomerId")]
        public Nullable<System.Guid> ProfileId { get; set; }

        public string ProfileName { get; set; }

        //9. Liên hệ
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Contact")]
        public Nullable<System.Guid> ContactId { get; set; }

        public string ContactName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Contact")]
        public Nullable<System.Guid> CompanyId { get; set; }

        public string CompanyName { get; set; }

        //10. Người tạo
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
        public string CreateBy { get; set; }

        //11. Mức độ
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_PriorityCode")]
        public string PriorityCode { get; set; }

        //12. Ngày tiếp nhận
        public string ReceiveCommonDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? ReceiveFromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? ReceiveToDate { get; set; }

        //13. Ngày bắt đầu
        public string StartCommonDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? StartFromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? StartToDate { get; set; }

        //14. Ngày kết thúc dự kiến
        public string EstimateEndCommonDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? EstimateEndFromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? EstimateEndToDate { get; set; }

        //15. Ngày kết thúc
        public string EndCommonDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? EndFromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? EndToDate { get; set; }

        //16. Ngày tạo
        //public string CreateCommonDate { get; set; }
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        //public DateTime? CreateFromTime { get; set; }

        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        //public DateTime? CreateToTime { get; set; }

        //17. Đơn vị thi công
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ConstructionUnit")]
        public Guid? ConstructionUnit { get; set; }

        public string ConstructionUnitName { get; set; }

        //18. Lỗi thường gặp
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_CommonMistakeCode")]
        public string CommonMistakeCode { get; set; }

        //19. Loại mã lỗi
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_ErrorTypeCode2")]
        public string ErrorTypeCode { get; set; }

        //20. Mã lỗi
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_ErrorCode2")]
        public string ErrorCode { get; set; }

        //21. Nhóm công ty
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_ServiceTechnicalTeamCode")]
        public string ServiceTechnicalTeamCode { get; set; }
        public Guid? KanbanId { get; set; }

        //mobile
        public bool? IsMobile { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CommonDate")]
        public string CommonDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_ProfileGroup")]
        public string ProfileGroupCode { get; set; }
        public List<string> ProfileGroupCodeList { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_Department")]
        public string RolesCode { get; set; }

        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        public bool? isUnassign { get; set; }

        //Mã màu
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductColorCode")]
        public List<string> ProductColorCode { get; set; }

        //Lỗi thường gặp
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UsualErrorCode")]
        public List<string> UsualErrorCode { get; set; }

        //Nhóm vật tư
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_ProductCategory")]
        public string ProductCategoryCode { get; set; }
        public List<string> ProductCategoryCodeList { get; set; }

        //Nhóm KH
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_General_CustomerGroup")]
        public string CustomerGroupCode { get; set; }

        //NV kinh doanh
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PersonInCharge")]
        public string SalesSupervisorCode { get; set; }

        //Phòng ban
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Department")]
        public string DepartmentCode { get; set; }
        public List<string> DepartmentCodeList { get; set; }

        //Ngày tạo
        public string CreatedCommonDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? CreatedFromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? CreatedToDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "GTB_VisitAddress")]
        public string VisitAddress { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? FromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? ToDate { get; set; }

        //Phân loại chuyến thăm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_VisitTypeCode")]
        public string VisitTypeCode { get; set; }

        //Trạng thái hoạt động
        [Display(Name = "Trạng thái hoạt động")]
        public bool? Actived { get; set; }
        [Display(Name = "Trạng thái hoạt động")] 
        public bool? isDeleted { get; set; }


        //Nhóm VT
        [Display(Name = "Nhóm vật tư")]
        public List<Guid> CategoryId { get; set; }

        //NV kết thúc (người nhập Đã hoàn thành)
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CompletedEmployee")]
        public string CompletedEmployee { get; set; }
        // ý kiến kh
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerResult")]
        //public string Property5 { get; set; }
        // Mã SAP SP
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_ERPProductCode")]
        public string ERPProductCode { get; set; }
        // Mã SAP phụ kiện
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_ERPAccessoryCode")]
        public string ERPAccessoryCode { get; set; }
        // Mã SAP phụ kiện
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_AccessoryTypeCode")]
        public string AccessoryTypeCode { get; set; }

        //Tỉnh/thành
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_ProvinceId")]
        public Guid? ProvinceId { get; set; }

        //Quận/huyện
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_DistrictId")]
        public Guid? DistrictId { get; set; }

        //Phường/xã
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WardId")]
        public Guid? WardId { get; set; }

        public string ProfileCode { get; set; }
        public bool IsView { get; set; }
        public Guid? DefaultWorkFlowId { get; set; }

        #region ProductionOrderViewModel
        public string ProductionOrder { get; set; }
        public string ProductionOrder_SAP { get; set; }
       
        public DateTime? ProductionOrder_StartDate { get; set; }
        public DateTime? ProductionOrder_EndDate { get; set; }
        public string Team { get; set; }
        public int? SLKH { get; set; }
        public string MaChiTiet { get; set; }
        public string TenChiTiet { get; set; }
        public string PhanXuong { get; set; }
        public DateTime? DateWorking { get; set; }
        public string LoaiGiaoDich { get; set; }
        public int? SoLuongDat { get; set; }
        public int? SoLuongKhongDat { get; set; }
        public bool? isComplete { get; set; }
        public int? SoLuongChuyen { get; set; }
        #endregion


        //LSX ĐT
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_Summary")]
        public string LSXDT { get; set; }        
        
        //LSX ĐT (SAP)
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDTSAP_Summary")]
        public string LSXDTSAP { get; set; }

        //Đợt SX
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Summary_DSX")]
        public string DSX { get; set; }

        //LSX SAP
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXSAP")]
        public string LSXSAP { get; set; }

        //LSX ĐT
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_Summary")]
        public string PPO_LSXDT { get; set; }

        //LSX SAP
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXSAP")]
        public string PPO_LSXSAP { get; set; }

        //Sản phẩm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductCode")]
        public string PPO_ProductCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductName")]
        public string PPO_ProductName { set; get; }

        //Công ty
        public string Plant { set; get; }

        //Xem theo điều kiện SL > 0
        [Display(Name = "Xem theo")]
        public bool? isViewQtyHasValue { get; set; }
        //Số lượng từ đến
        [Display(Name = "Từ")]
        public int? FromQty { get; set; }
        [Display(Name = "Đến")]
        public int? ToQty { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Property1")]
        public string Property1 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Property2")]
        public string Property2 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Date1")]
        public DateTime? Date1 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductCode")]
        public string ProductCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductName")]
        public string ProductName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaskStatusId")]
        public Guid? TaskStatusId   { get; set; }
        public string TaskStatusName   { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isUpdateSerial")]
        public bool? isUpdateSerial { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "isUpdateQuatity")]
        public bool? isUpdateQuatity { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_ReceiveDate")]
        public DateTime? ReceiveDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_EstimateDate")]
        public DateTime? EstimateDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_StartDate")]
        public DateTime? StartDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_EndDate")]
        public DateTime? EndDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Text1")]
        public string Text1 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Text2")]
        public string Text2 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Number1")]
        public decimal? Number1 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Property3")]
        public string Property3   { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Property4")]
        public string Property4 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Property5")]
        public string Property5 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Property6")]
        public string Property6 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Property7")]
        public string Property7 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Property8")]
        public string Property8 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Property9")]
        public string Property9 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Property10")]
        public string Property10 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Property11")]
        public string Property11 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_PrintMold")]
        public string PrintMold { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Serial")]
        public string Serial { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Date2")]
        public DateTime? Date2 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Date3")]
        public DateTime? Date3 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Reporter")]
        public string Reporter   { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Text3")]
        public string Text3 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Text4")]
        public string Text4 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Text5")]
        public string Text5 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Unit")]
        public string Unit { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Number2")]
        public decimal Number2 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Number3")]
        public decimal Number3 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Qty")]
        public decimal Qty { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_Component")]
        public string Component { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSX_SaleEmployeeCode")]
        public string SaleEmployeeCode { get; set; }
    }
}