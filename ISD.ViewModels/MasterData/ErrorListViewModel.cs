using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ErrorListViewModel : ErrorListModel
    {
     
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StepName")]
        public string StepName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TestTargetName")]
 
        public string TestTargetName { get; set; }
    }
}
