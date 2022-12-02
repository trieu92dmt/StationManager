using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class CreateSubTaskViewModel
    {
        public Guid TaskId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BatchCode")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string Summary { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public Guid WorkFlowId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_StartDate")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public DateTime? StartDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_EstimateEndDate")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public DateTime? EstimateEndDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Status")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public Guid TaskStatusId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Qty")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public int? Qty { get; set; }
    }
}
