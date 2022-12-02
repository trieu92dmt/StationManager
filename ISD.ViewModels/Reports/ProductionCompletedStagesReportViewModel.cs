using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ProductionCompletedStagesReportViewModel
    {
        //SO number
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_DateCompleted")]
        public DateTime? DateCompleted { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_Target")]
        public decimal? TargetQuantity { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CumulativeTarget")]
        public decimal? CumulativeTargetQuantity { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_Completed")]
        public decimal? CompletedQuantity { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CumulativeCompleted")]
        public decimal? CumulativeCompletedQuantity { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_Quantity")]
        public decimal? Quantity { get; set; }

        public string Type { get; set; }

        public bool IsView { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedFromDate")]
        [DisplayFormat()]
        public DateTime? CompletedFromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Reports_CompletedToDate")]
        public DateTime? CompletedToDate { get; set; }
        public string Step { get; set; }


        public string CreatedCommonDate { get; set; }

    }
}
