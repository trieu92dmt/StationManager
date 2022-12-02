using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MES
{
    public class EmployeeCheckinViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SerialTag")]
        public string SerialTag { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeCode")]
        public string SalesEmployeeCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer_FullName")]
        public string SalesEmployeeName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_Department")]
        public string DepartmentName { get; set; }
    }
}
