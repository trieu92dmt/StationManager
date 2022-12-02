using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ErrorListSearchViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ErrorList_ErrorListName")]
        public string ErrorListName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public bool? Actived { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ErrorList_ErrorListCode")]
        public string ErrorListCode { get; set; }
        public string ErrorListId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StepName")]
        public string StepCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TestTargetName")]
        public int? TargetCode { get; set; }
    }
}
