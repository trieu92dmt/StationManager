using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MES
{
    public class CheckInHistoryViewModel
    {
        public int? STT { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SerialTag")]
        public string SerialTag { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeCode")]
        public string SalesEmployeeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SalesEmployee_Name")]
        public string SalesEmployeeName { get; set; }
        [Display(Name = "Thời gian")]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss dd/MM/yyyy}")]
        public DateTime? CheckInDate { get; set; }
        public Guid? WorkingDepartment { get; set; }
        public Guid? CheckInOutDepartment { get; set; }
        public string WorkingDepartmentName { get; set; }
        public string WorkingDepartmentCode { get; set; } 
        public string CheckInOutDepartmentName { get; set; }
        public string CheckInOutDepartmentCode { get; set; }
        public Guid? WorkShopWorkingId { get; set; }
        public Guid? WorkShopCheckInOutId { get; set; }
        public string WorkShopWorkingName { get; set; }
        public string WorkShopWorkingpCode { get; set; }   
        public string WorkShopCheckInOutName { get; set; }
        public string WorkShopCheckInOutCode { get; set; }
    }
}
