using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class AssignmentCongDoanViewModel
    {
        public int? STT { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_StepCode")]
        public string StepCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_StepName")]
        public string StepName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_NumberOfEmployees")]
        public int? NumberOfEmployees { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_NumberOfMachines")]
        public int? NumberOfMachines { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_Assignee")]
        public List<string> Assignee { get; set; }
        public Guid? WorkShopId { get; set; }
        public Guid? DepartmentId { get; set; }
        public string EmployeeCode { get; set; }
        public Guid? RoutingId { get; set; }
    }
}
