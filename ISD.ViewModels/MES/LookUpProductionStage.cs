using System;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace ISD.ViewModels
{
    public class LookUpProductionStage
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductCode_Routing")]
        public string ProductCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductAttributes")]
        public string ProductAttributes { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepCode")]
        public string StepCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_LTXA1")]
        public string LTXA1 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ARBPL")]
        public string ARBPL { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_PLNNR")]
        public string PLNNR { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "POT12")]

        public string POT12 { get; set; }
    }

}