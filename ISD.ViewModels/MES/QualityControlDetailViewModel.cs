using ISD.EntityModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ISD.ViewModels
{
    public class QualityControlDetailViewModel
    {
        public System.Guid? QualityControlDetailId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_InspectionLotQuantity")]
        public Nullable<System.Guid> QualityControlId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_TestMethod")]
        public string TestMethod { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_SamplingLevel")]
        public string SamplingLevel { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_AcceptableLevel")]
        public string AcceptableLevel { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_InspectionLotQuantity")]
        public Nullable<decimal> InspectionQuantity { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_Result")]
        public string Result { get; set; }
        public string SamplingLevelName { get; set; }
    }
}
