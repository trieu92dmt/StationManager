using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class AssignmentViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_WorkShop")]
        public Guid? WorkShopId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_Department")]
        public Guid? DepartmentId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_WorkingDate")]
        public DateTime? WorkingDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_FromTime")]
        public TimeSpan? FromTime { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_ToTime")]
        public TimeSpan? ToTime { get; set; }

        public List<string> LSXC { get; set; }
        public int? DateKey { get; set; }
    }
}
