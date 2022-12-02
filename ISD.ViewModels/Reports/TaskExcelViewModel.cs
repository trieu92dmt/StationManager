using System;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class TaskExcelViewModel
    {
        [Display(Name = "Ngày tiếp nhận")]
        public DateTime? ReceiveDate { get; set; }
       

        [Display(Name = "Ngày bắt đầu")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "Ngày kết thúc")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Người tạo")]
        public string CreateByName { get; set; }
        [Display(Name = "Tên khách hàng")]
        public string ProfileName { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "SĐT liên hệ")]
        public string Phone { get; set; }
        [Display(Name = "Loại")]
        public string WorkFlowName { get; set; }
        [Display(Name = "Trạng thái")]
        public string TaskStatusName { get; set; }
        [Display(Name = "Trung tâm bảo hành")]
        public string ServiceTechnicalTeam { get; set; }
        [Display(Name = "NV được phân công")]
        public string AssigneeName { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Kết quả")]
        public string CustomerReviews { get; set; }
        [Display(Name = "Đánh giá chất lượng dịch vụ")]
        public string ServiceRating { get; set; }
        [Display(Name = "Đánh giá chất lượng sản phẩm")]
        public string ProductRating { get; set; }
        [Display(Name = "Ý kiến khách hàng")]
        public string Review { get; set; }
        [Display(Name = "Mã SAP sản phẩm")]
        public string ERPProductCode { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Display(Name = "Số lượng")]
        public int? Qty { get; set; }

        [Display(Name = "Phân loại sản phẩm")]
        public string ProductCategoryName { get; set; }
        [Display(Name = "Phương thức xử lý")]
        public string ErrorName { get; set; }
        [Display(Name = "Hình thức bảo hành")]
        public string ErrorTypeName { get; set; }
        [Display(Name = "Mã SAP phụ kiện")]
        public string ERPAccessoryCode { get; set; }
        [Display(Name = "Tên phụ kiện")]
        public string AccessoryName { get; set; }
        [Display(Name = "Số lượng PK")]
        public int AccessoryQty { get; set; }
        [Display(Name = "Loại phụ kiện")]
        public string AccessoryCategoryName { get; set; }
        public Guid? ProfileId { get; set; }

        //THKH: Ngày thực hiện => EndDate
        [Display(Name = "Ngày thực hiện")]
        public DateTime? THKH_EndDate
        {
            get
            {
                if (EndDate.HasValue)
                {
                    return EndDate;
                }
                return null;
            }
        }

        [Display(Name = "Tiêu đề")]
        public string Summary { get; set; }

        public string TaskStatusCode { get; set; }

       

        

        [Display(Name = "Mã khách hàng")]
        public int? ProfileCode { get; set; }

       

        [Display(Name = "NV kinh doanh")]
        public string PersonInCharge { get; set; }

        [Display(Name = "Phòng ban")]
        public string RoleInCharge { get; set; }

       

       

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Địa điểm ghé thăm")]
        public string VisitAddress { get; set; }

        [Display(Name = "Khu vực")]
        public string SaleOfficeName { get; set; }

       

       

        [Display(Name = "Ngày thi công")]
        public DateTime? ConstructionDate { get; set; }

       

        [Display(Name = "Ngày tạo")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "Ngày dự kiến")]
        public DateTime? EstimateEndDate { get; set; }

        //THKH: Ngày dự kiến => StartDate
        [Display(Name = "Ngày dự kiến")]
        public DateTime? THKH_StartDate
        {
            get
            {
                if (StartDate.HasValue)
                {
                    return StartDate;
                }
                return null;
            }
        }

        [Display(Name = "Thời gian checkin")]
        public DateTime? CheckInTime { get; set; }

        [Display(Name = "Số đơn hàng")]
        public string OrderCode { get; set; }

        [Display(Name = "Giá trị bảo hành")]
        public string WarrantyValue { get; set; }

        
        

        [Display(Name = "Mã màu")]
        public string ProductColorCode { get; set; }

        [Display(Name = "Danh sách lỗi")]
        public string UsualErrorName { get; set; }

       

        

        
       
       
        

        #region //Bảo hành ACC

[Display(Name = "Ngày tiếp nhận")]
        public DateTime? TICKET_ReceiveDate { get; set; }

        [Display(Name = "Mã khách hàng")]
        public int? TICKET_ProfileCode { get; set; }

        [Display(Name = "Tên khách hàng")]
        public string TICKET_ProfileName { get; set; }

        [Display(Name = "Địa chỉ công trình")]
        public string TICKET_Address { get; set; }

        [Display(Name = "SĐT liên hệ")]
        public string TICKET_Phone { get; set; }

        [Display(Name = "Mô tả")]
        public string TICKET_Description { get; set; }

        [Display(Name = "Ngày thực hiện")]
        public DateTime? TICKET_StartDate { get; set; }

        [Display(Name = "Ngày kết thúc")]
        public DateTime? TICKET_EndDate { get; set; }

        [Display(Name = "Kết quả/Ý kiến KH")]
        public string TICKET_CustomerReviews { get; set; }

        [Display(Name = "Danh sách lỗi")]
        public string TICKET_UsualErrorName { get; set; }

        [Display(Name = "Hình thức bảo hành")]
        public string TICKET_ErrorTypeName { get; set; }

        [Display(Name = "Nguồn tiếp nhận")]
        public string TICKET_TaskSourceName { get; set; }

        [Display(Name = "Khu vực")]
        public string TICKET_SaleOfficeName { get; set; }

        [Display(Name = "NV tiếp nhận")]
        public string TICKET_CreateByName { get; set; }

        [Display(Name = "Mã ĐVTC")]
        public string TICKET_ConstructionUnit { get; set; }

        [Display(Name = "Tên ĐVTC")]
        public string TICKET_ConstructionUnitName { get; set; }

        [Display(Name = "NV Kinh doanh")]
        public string TICKET_PersonInCharge { get; set; }

        [Display(Name = "Ngày thi công")]
        public DateTime? TICKET_ConstructionDate { get; set; }

        [Display(Name = "Số đơn hàng")]
        public string TICKET_OrderCode { get; set; }

        [Display(Name = "NV được phân công")]
        public string TICKET_AssigneeName { get; set; }

        [Display(Name = "Nhóm vật tư")]
        public string TICKET_ProductCategoryName { get; set; }

        [Display(Name = "Tên sản phẩm/ Mã màu")]
        public string TICKET_ProductColorCode { get; set; }

        [Display(Name = "Số lượng bảo hành")]
        public int? TICKET_Qty { get; set; }

        [Display(Name = "Giá trị bảo hành")]
        public string TICKET_WarrantyValue { get; set; }

        [Display(Name = "Trạng thái")]
        public string TICKET_TaskStatusName { get; set; }

        [Display(Name = "Loại")]
        public string TICKET_WorkFlowName { get; set; }

        #endregion //Bảo hành ACC

        #region //Điểm trưng bày

        [Display(Name = "Mã")]
        public int? GTB_ProfileCode { get; set; }

        [Display(Name = "Mã SAP")]
        public string GTB_ProfileForeignCode { get; set; }

        [Display(Name = "Tên ngắn KH")]
        public string GTB_ProfileShortName { get; set; }

        [Display(Name = "Nhóm KH")]
        public string GTB_CustomerGroupName { get; set; }

        [Display(Name = "NV kinh doanh")]
        public string GTB_PersonInCharge { get; set; }

        [Display(Name = "Phòng ban")]
        public string GTB_RoleInCharge { get; set; }

        [Display(Name = "Trạng thái")]
        public string GTB_TaskStatusName { get; set; }

        [Display(Name = "Ngày lắp")]
        public DateTime? GTB_StartDate { get; set; }

        [Display(Name = "Địa chỉ GVL")]
        public string GTB_VisitAddress { get; set; }
        [Display(Name = "Địa chỉ ĐTB")]
        public string DTB_VisitAddress { get; set; }

        [Display(Name = "Quận/Huyện")]
        public string GTB_DistrictName { get; set; }

        [Display(Name = "Tỉnh/Thành")]
        public string GTB_ProvinceName { get; set; }

        [Display(Name = "Khu vực")]
        public string GTB_SaleOfficeName { get; set; }

        [Display(Name = "Liên hệ")]
        public string GTB_ContactName { get; set; }

        [Display(Name = "SĐT liên hệ")]
        public string GTB_ContactPhone { get; set; }

        [Display(Name = "Giá trị GVL")]
        public decimal? GTB_ValueOfShowroom { get; set; }
        [Display(Name = "Giá trị ĐTB")]
        public decimal? DTB_ValueOfShowroom { get; set; }

        [Display(Name = "Ngày chăm sóc gần nhất")]
        public DateTime? GTB_NearestDate_THKH { get; set; }
        [Display(Name = "Nhân viên chăm sóc")]
        public string GTB_AssigneeName_THKH { get; set; }
        [Display(Name = "Nội dung chăm sóc")]
        public string GTB_Description_THKH { get; set; }

        [Display(Name = "Ngày chăm sóc dự kiến")]
        public DateTime? GTB_RemindDate_THKH { get; set; }

        [Display(Name = "Doanh thu năm trước đó")]
        public decimal? GTB_LastYearrevenue { get; set; }

        [Display(Name = "Doanh thu năm hiện tại")]
        public decimal? GTB_CurrentRevenue { get; set; }

        #endregion //Điểm trưng bày

        #region Nhiệm vụ

        [Display(Name = "Ngày bắt đầu")]
        public DateTime? ACTI_StartDate { get; set; }

        [Display(Name = "Tiêu đề")]
        public string ACTI_Summary { get; set; }

        [Display(Name = "Trạng thái")]
        public string ACTI_TaskStatusName { get; set; }

        [Display(Name = "Loại")]
        public string ACTI_WorkFlowName { get; set; }

        [Display(Name = "Mô tả")]
        public string ACTI_Description { get; set; }

        [Display(Name = "NV Kinh doanh")]
        public string ACTI_PersonInCharge { get; set; }

        [Display(Name = "Phòng Ban")]
        public string ACTI_RoleInCharge { get; set; }

        [Display(Name = "Mã khách")]
        public int? ACTI_ProfileCode { get; set; }

        [Display(Name = "Tên Khách")]
        public string ACTI_ProfileName { get; set; }

        [Display(Name = "Địa chỉ")]
        public string ACTI_Address { get; set; }

        [Display(Name = "SDT liên hệ")]
        public string ACTI_Phone { get; set; }

        [Display(Name = "Email")]
        public string ACTI_Email { get; set; }

        [Display(Name = "Ngày tiếp nhận")]
        public DateTime? ACTI_ReceiveDate { get; set; }

        [Display(Name = "NV theo dõi/giám sát")]
        public string ACTI_ReporterName { get; set; }

        [Display(Name = "NV được phân công")]
        public string ACTI_AssigneeName { get; set; }

        #endregion Nhiệm vụ

        #region Giao Việc
        [Display(Name = "Mã yêu cầu")]
        public string MIS_TaskCode { get; set; }
        [Display(Name = "Tiêu Đề")]
        public string MIS_Summary { get; set; }
        [Display(Name = "Trạng Thái")]
        public string MIS_TaskStatusName { get; set; }
        [Display(Name = "Mức Độ")]
        public string MIS_PriorityName { get; set; }
        [Display(Name = "Mô tả")]
        public string MIS_Description { get; set; }
        [Display(Name = "NV Giao việc")]
        public string MIS_CreateByName { get; set; }
        [Display(Name = "NV theo dõi/giám sát")]
        public string MIS_ReporterName { get; set; }
        [Display(Name = "NV được phân công")]
        public string MIS_AssigneeName { get; set; }
        [Display(Name = "Ngày bắt đầu")]
        public DateTime? MIS_StartDate { get; set; }
        [Display(Name = "Ngày đến hạn")]
        public DateTime? MIS_EstimateEndDate { get; set; }
        [Display(Name = "Ngày kết thúc")]
        public DateTime? MIS_EndDate { get; set; }
        #endregion
    }
}