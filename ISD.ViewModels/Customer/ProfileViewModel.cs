using ISD.Constant;
using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class ProfileViewModel : ProfileModel
    {
        public int? STT { get; set; }
        public string CustomerTypeName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_ContactName")]
        public string ContactName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Position")]
        public string PositionB { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_CustomerCategoryCode")]
        public string CustomerCategoryCode { get; set; }

        //Customer
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_CompanyId")]
        public Guid? CompanyId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Position")]
        public string PositionC { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
        public string CreateUser { get; set; }

        public string CompanyName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Position")]
        public string ProfileContactPosition { get; set; }

        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PrimaryContact")]
        public Nullable<bool> IsMain { get; set; }

        public List<CatalogProfileViewModel> CatalogList { get; set; }
        public List<ColorProfileViewModel> ColorList { get; set; }
        public List<WoodGrainProfileViewModel> WoodGrainList { get; set; }
        public List<ColorToneProfileViewModel> ColorToneList { get; set; }
        public List<CollectionProfileViewModel> CollectionList { get; set; }
        public List<ProductGroupProfileViewModel> ProductGroupList { get; set; }
        public List<ConstructionViewModel> internalList { get; set; }
        public List<ConstructionViewModel> competitorList { get; set; }

        //Contact
        public string Position { get; set; }
        public string PositionName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Department")]
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string CategoryCode { get; set; }
        public string Summary { get; set; }
        public string CustomerTitle { get; set; }
        public string VisitDate_String { get; set; }
        public Guid? TaskStatusId { get; set; }
        public string SearchProfileName { get; set; }

        public string PersonInChargeListName { get; set; }

        public string DateOfBirthWithFormat
        {
            get
            {
                if (DayOfBirth.HasValue && MonthOfBirth.HasValue)
                {
                    return string.Format("{0}/{1}", DayOfBirth.Value, MonthOfBirth.Value);
                }
                return string.Empty;
            }
        }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_Store")]
        public List<Guid> StoreId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Phone")]
        public string PhoneBusiness { get; set; }

        public string EmailBusiness { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_ProfileForeignCode")]
        public string SearchProfileForeignCode { get; set; }
        public bool? SearchProfileForeignCodeIsNull { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_SaleOfficeCode")]
        [RequiredIf("Type", "Account", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string RequiredSaleOfficeCode {
            get
            {
                return SaleOfficeCode;
            }
            set
            {
                SaleOfficeCode = value;
            }
        }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_ProvinceId")]
        [RequiredIf("Type", "Account", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public Guid? RequiredProvinceId {
            get
            {
                return ProvinceId;
            }
            set
            {
                ProvinceId = value;
            }
        }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AddressBook_AddressTypeCode")]
        [RequiredIf("Type", "Account", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string RequiredAddressTypeCode
        {
            get
            {
                return AddressTypeCode;
            }
            set
            {
                AddressTypeCode = value;
            }
        }

        public string Type { get; set; }

        public List<ProfileGroupCreateViewModel> profileGroupList { get; set; }

        public List<string> profileGroupOtherCompanyList { get; set; }

        public List<string> profileCareerOtherCompanyList { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PersonInCharge")]
        public string SalesEmployeeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Department")]
        public string RolesCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateFromDate")]
        public DateTime? CreateFromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateToDate")]
        public DateTime? CreateToDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
        public string CreateByCode { get; set; }
        public string ContactPhone { get; set; }
        public string Contact { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer_DateOfBirth")]
        public string DateOfBirth { get; set; }

        public string LeadCategory { get; set; }
        public string OpportunityCategory { get; set; }
        public string OpportunityPercentage { get; set; }
        public string OpportunityType { get; set; }
        public string ProjectScale { get; set; }
        public string OpportunityStatus { get; set; }
        public string ConsultingAndDesign { get; set; }
        public string Construction { get; set; }
        public string ProjectStatus { get; set; }
        public string ProjectComplete { get; set; }
        public string ProjectGabarit { get; set; }
        public string OpportunityUnit { get; set; }
        public string CompleteYear { get; set; }
        public string CompleteQuarter { get; set; }
        public Guid? OpportunityPartnerId { get; set; }

        public string ProfileTypeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateRequestTimeFrom")]
        public DateTime? CreateRequestTimeFrom { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateRequestTimeTo")]
        public DateTime? CreateRequestTimeTo { get; set; }

        //Mobile additional field
        public Guid? ProfileContactId { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPosition { get; set; }
        public string ContactDepartment { get; set; }
        public string ChannelCode { get; set; }
        public bool? IsHasAppointment { get; set; }
        public string SaleEmployeeOffer { get; set; }
        public string Reviews { get; set; }
        public string Ratings { get; set; }
        public bool? TaxNoIsNull { get; set; }
        public bool? AddressIsNull { get; set; }
        public bool? ProvinceIdIsNull { get; set; }
        public bool? DistrictIdIsNull { get; set; }
        public bool? WardIdIsNull { get; set; }
        public bool? CustomerTypeCodeIsNull { get; set; }
        public bool? EmailIsNull { get; set; }
        public bool? CustomerSourceCodeIsNull { get; set; }
        public bool? CustomerGroupCodeIsNull { get; set; }
        public bool? CustomerCareerCodeIsNull { get; set; }
        public bool? SalesEmployeeCodeIsNull { get; set; }
        public bool? isVisitCabinetPro { get; set; }
        public string VisitCabinetProRequest { get; set; }
    }

    public class ProfileSearchResultViewModel
    {
        public int? STT { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProfileId")]
        public System.Guid ProfileId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_ProfileCode")]
        public int ProfileCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_ProfileForeignCode")]
        public string ProfileForeignCode { get; set; }
        public string CreateAtCompanyName { get; set; }
        public string ProfileShortName { get; set; }
        public string CountryName { get; set; }
        public string ReconcileAccountCode { get; set; }
        public string ReconcileAccountName { get; set; }
        public string PaymentTermName { get; set; }
        public string TitleType { get; set; }
        public string CustomerAccountAssignmentGroupName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_CustomerTypeCode")]
        public string CustomerTypeCode { get; set; }

        public string CustomerTypeName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_ProfileName")]
        public string ProfileName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Phone")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Address")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
        public string CreateUser { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateTime")]
        public Nullable<System.DateTime> CreateTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public Nullable<bool> Actived { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_CompanyId")]
        public Nullable<System.Guid> CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_Store")]
        public Guid? StoreId { get; set; }

        //Tạo tại công ty
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_CompanyId")]
        public Nullable<System.Guid> AtCompanyId { get; set; }

        //Đối tượng
        public bool? isForeignCustomer { get; set; }

        //Nguồn khách hàng
        public string CustomerSourceName { get; set; }

        //Chi nhánh
        public string SaleOrgName { get; set; }

        //Độ tuổi
        public string Age { get; set; }

        //MST
        public string TaxNo { get; set; }

        //Nhóm khách hàng
        public string CustomerGroupName { get; set; }

        //Ngành nghề
        public string CustomerCareerName { get; set; }

        //Khu vực
        public string SaleOfficeName { get; set; }

        //Ghi chú
        public string Note { get; set; }

        //NV phụ trách
        public string PersonInCharge { get; set; }

        //Phòng ban
        public string RoleInCharge { get; set; }

        //Danh xưng
        public string Title { get; set; }

        //Doanh số năm trước
        //[DisplayFormat(DataFormatString = "{0:0n}")]
        public decimal? PreRevenue { get; set; }

        //Doanh số năm hiện tại
        //[DisplayFormat(DataFormatString = "{0:0n}")]
        public decimal? CurrentRevenue { get; set; }

        //Mobile additional field
        public Guid? ProfileContactId { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPosition { get; set; }
        public string ContactDepartment { get; set; }
        public string SalesSupervisorCode { get; set; }
        public string SalesSupervisorName { get; set; }
        public string DepartmentName { get; set; }
        public List<string> AddressList { get; set; }
        public string PaymentMethodName { get; set; }
        public string PartnerFunctionName { get; set; }
        public string TaxClassificationName { get; set; }
        public string CurrencyName { get; set; }
        public string ManagerName { get; set; }
        public string DebsEmployeeName { get; set; }

    }

    public class CatalogProfileViewModel
    {
        public string ProductCode { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

    public class ColorProfileViewModel
    {
        public string CompanyCode { get; set; }
        public string MaterialCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string SaleOrgCode { get; set; }
        public int Like { get; set; }
    }

    public class WoodGrainProfileViewModel
    {
        public string WoodGrainCode { get; set; }
        public string WoodGrainName { get; set; }
    }

    public class ColorToneProfileViewModel
    {
        public string ColorToneName { get; set; }
    }

    public class CollectionProfileViewModel
    {
        public string CollectionCode { get; set; }
        public string CollectionName { get; set; }
        public string Ratings { get; set; }
    }

    public class ProductGroupProfileViewModel
    {
        public string ProductGroupCode { get; set; }
        public string ProductGroupName { get; set; }
    }

    public class ProfileGroupCreateViewModel
    {
        public System.Guid? ProfileGroupId { get; set; }

        public Nullable<System.Guid> ProfileId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_CustomerCategoryCode")]
        //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string ProfileGroupCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProfileGroupCode")]
        public string ProfileGroupName { get; set; }

        public string CompanyCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
        public Nullable<System.Guid> CreateBy { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateTime")]
        [DisplayFormat(DataFormatString = SystemConfig.DateTimeFormat)]
        public Nullable<System.DateTime> CreateTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastEditBy")]
        public Nullable<System.Guid> LastEditBy { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastEditTime")]
        [DisplayFormat(DataFormatString = SystemConfig.DateTimeFormat)]
        public Nullable<System.DateTime> LastEditTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
        public string CreateUser { get; set; }
    }

    public class ProfileCareerViewModel
    {
        public System.Guid? ProfileCareerId { get; set; }

        public Nullable<System.Guid> ProfileId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_CustomerCareerCode")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string ProfileCareerCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProfileGroupCode")]
        public string ProfileCareerName { get; set; }

        public string CompanyCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
        public Nullable<System.Guid> CreateBy { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateTime")]
        [DisplayFormat(DataFormatString = SystemConfig.DateTimeFormat)]
        public Nullable<System.DateTime> CreateTime { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastEditBy")]
        public Nullable<System.Guid> LastEditBy { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastEditTime")]
        [DisplayFormat(DataFormatString = SystemConfig.DateTimeFormat)]
        public Nullable<System.DateTime> LastEditTime { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
        public string CreateUser { get; set; }
    }
}