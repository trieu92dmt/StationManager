using System;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class ProfileReportViewModel
    {
        public Guid? ProfileId { get; set; }
        [Display(Name = "Mã CRM")]
        public int? ProfileCode { get; set; }

        [Display(Name = "Mã SAP")]
        public string ProfileForeignCode { get; set; }

        [Display(Name = "Mã SAP")]
        public string CompanyProfileForeignCode { get; set; }

        [Display(Name = "Tên")]
        public string ProfileName { get; set; }

        [Display(Name = "Tên")]
        public string CompanyName { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "SĐT")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Đối tượng")]
        public string ForeignCustomer { get; set; }

        [Display(Name = "Đối tượng")]
        public string CompanyForeignCustomer { get; set; }

        [Display(Name = "Nguồn KH")]
        public string CustomerSourceName { get; set; }

        [Display(Name = "Chi nhánh")]
        public string SaleOrgName { get; set; }

        [Display(Name = "Phân loại KH")]
        public string CustomerTypeName { get; set; }

        [Display(Name = "Nhóm KH")]
        public string CustomerGroupName { get; set; }

        [Display(Name = "Ngành nghề")]
        public string CustomerCareerName { get; set; }

        [Display(Name = "Phường/Xã")]
        public string WardName { get; set; }

        [Display(Name = "Quận/Huyện")]
        public string DistrictName { get; set; }

        [Display(Name = "Tỉnh/Thành phố")]
        public string ProvinceName { get; set; }

        [Display(Name = "Khu vực")]
        public string SaleOfficeName { get; set; }

        [Display(Name = "MST")]
        public string TaxNo { get; set; }

        [Display(Name = "Độ tuổi")]
        public string Age { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Display(Name = "Mã liên hệ (LH chính)")]
        public string ContactCode { get; set; }

        [Display(Name = "Tên liên hệ")]
        public string ContactName { get; set; }

        [Display(Name = "SĐT liên hệ")]
        public string ContactPhone { get; set; }

        [Display(Name = "Email liên hệ")]
        public string ContactEmail { get; set; }

        [Display(Name = "Chức vụ")]
        public string ContactPositionName { get; set; }

        [Display(Name = "Chức vụ")]
        public string PositionName { get; set; }

        [Display(Name = "Phòng ban")]
        public string ContactDepartmentName { get; set; }

        [Display(Name = "Phòng ban")]
        public string DepartmentName { get; set; }

        [Display(Name = "NV kinh doanh")]
        public string PersonInCharge { get; set; }

        [Display(Name = "Phòng ban (NV kinh doanh)")]
        public string RoleInCharge { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? Actived { get; set; }

        [Display(Name = "Doanh số năm ngoái")]
        public decimal? PreRevenue { get; set; }

        [Display(Name = "Doanh số năm nay")]
        public decimal? CurrentRevenue { get; set; }
    }

    public class ProfileOpportunityReportViewModel
    {
        [Display(Name = "NV kinh doanh")]
        public string PersonInCharge { get; set; }

        [Display(Name = "NV Sales Admin")]
        public string SalesAdmin { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime? LastEditTime { get; set; }

        [Display(Name = "Tên dự án")]
        public string ProfileName { get; set; }

        [Display(Name = "Khu vực")]
        public string SaleOfficeName { get; set; }

        [Display(Name = "Địa điểm dự án")]
        public string ProjectLocation { get; set; }

        [Display(Name = "Loại hình")]
        public string OpportunityType { get; set; }

        [Display(Name = "Quy mô")]
        public string ProjectGabarit { get; set; }

        [Display(Name = "ĐVT")]
        public string OpportunityUnit { get; set; }

        [Display(Name = "Chủ đầu tư")]
        public string Investor { get; set; }

        [Display(Name = "Tư vấn-Thiết kế")]
        public string ConsultingAndDesign { get; set; }

        [Display(Name = "Tổng thầu")]
        public string GeneralContractor { get; set; }

        [Display(Name = "Căn mẫu")]
        public string Internal { get; set; }

        [Display(Name = "Đại trà")]
        public string Competitor { get; set; }

        [Display(Name = "Laminate")]
        public string Laminate { get; set; }

        [Display(Name = "MFC/UV/ARC")]
        public string MFC { get; set; }

        [Display(Name = "Veneer")]
        public string Veneer { get; set; }

        [Display(Name = "Ván sàn")]
        public string Flooring { get; set; }

        [Display(Name = "Phụ kiện")]
        public string Accessories { get; set; }

        [Display(Name = "Thiết bị")]
        public string KitchenEquipment { get; set; }

        [Display(Name = "Thương hiệu khác")]
        public string OtherBrand { get; set; }

        [Display(Name = "Nội thất bàn giao")]
        public string HandoverFurniture { get; set; }

        [Display(Name = "Tình trạng dự án")]
        public string ProjectStatus { get; set; }

        [Display(Name = "Hoàn thiện")]
        public string ProjectComplete { get; set; }
    }
}
