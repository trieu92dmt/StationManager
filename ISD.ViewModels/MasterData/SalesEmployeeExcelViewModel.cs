using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MasterData
{
    public class SalesEmployeeExcelViewModel
    {
      
        //Mã nhân viên
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeCode")]
        [Required]
        public string SalesEmployeeCode { get; set; }

        //Tên nhân vien
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer_FullName")]
        [Required]
        public string SalesEmployeeName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_Department")]
        public string DepartmentName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_Department")]
        public Nullable<System.Guid> DepartmentId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_DepartmentCode")]
        public string DepartmentCode { get; set; }
        //CompanyId
        public Guid? CompanyId { get; set; }
        //StoreId
        public Guid? StoreId { get; set; }
        //Email
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_Email")]
        public string Email { get; set; }
        //Số điện thoại
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PhoneNumber")]
        public string Phone { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Position")]
        public string Position { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
        public string Note { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public bool? Actived { get; set; }

        //Import Excel
        public int RowIndex { get; set; }
        public string Error { get; set; }
        public bool isNullValueId { get; set; }
    }
}
