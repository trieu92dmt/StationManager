using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MES
{
    public class ThucThiLenhSanXuatViewModel: ThucThiLenhSanXuatModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductCode")]
        public string ProductCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ProductName")]
        public string ProductName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_Summary")] 
        public string LSXDT_Summary { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_StepCode")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string StepCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TTLSX_StepName")] 
        public string StepName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
        public string CreateByName { get; set; }
        public Int64? STT { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RoutingDetail")]
        public string RoutingDetail { get; set; }

        public string LSXSAP { get; set; }
    }
}
