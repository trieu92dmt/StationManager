using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class AssignmentNhanVienViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_WorkShop")]
        public string WorkShop { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_Department")]
        public string Department { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_EmployeeCode")]
        public string EmployeeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_EmployeeName")]
        public string EmployeeName { get; set; }
    }
}
