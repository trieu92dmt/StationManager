using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class EmployeeAssignmentSearchViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_Store")]
        public Guid? StoreId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_WorkShop")]
        public Guid? AssignmentWorkShopId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_Department")]
        public Guid? AssignmentDepartmentId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_Routing")]
        public Guid? RoutingId { get; set; }
    }

    public class EmployeeAssignmentSearchResultViewModel
    {
        public Guid? DepartmentId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_EmployeeCode")]
        public string EmployeeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_EmployeeName")]
        public string EmployeeName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_Department")]
        public string Department { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_EmployeeRouting")]
        public string EmployeeRouting { get; set; }
    }
}
