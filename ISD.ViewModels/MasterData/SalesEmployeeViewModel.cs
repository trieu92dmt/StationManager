using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ISD.ViewModels
{
    public class SalesEmployeeViewModel
    {
        //Mã nhân viên
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeCode")]
        public string SalesEmployeeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_CompanyId")]
        public Nullable<System.Guid> CompanyId { get; set; }
        public string CompanyName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_Store")]
        public Nullable<System.Guid> StoreId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_Department")]
        public Nullable<System.Guid> DepartmentId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeNote")]
        public string DepartmentName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_Department")]
        public string DepartmentCode { get; set; }

        //Tên nhân vien
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer_FullName")]
        public string SalesEmployeeName { get; set; }

        public string AbbreviatedName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PhoneNumber")]
        public string Phone { get; set; }

        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> LastEditBy { get; set; }
        public Nullable<System.DateTime> LastEditTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public bool? Actived { get; set; }

        public Guid? AccountId { get; set; }

        //Phòng ban
        public string RolesName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SerialTag")]
        public string SerialTag { get; set; }
        public bool? IsHasSerialTag
        {
            get
            {
                if (!string.IsNullOrEmpty(SerialTag))
                {
                    return true;
                }
                return false;
            }
        }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_WorkShop")]
        public Nullable<System.Guid> WorkShopId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_WorkShop")]
        public string WorkShopName { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Position")]
        public string Position { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
        public string Note { get; set; }
    }
}