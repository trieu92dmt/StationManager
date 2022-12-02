using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BC19ReportViewModel
    {
        public int? STT { get; set; }
        public bool? IsView { get; set; }
        public string Plant { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BC19_DSX")]
        public string DSX { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BC19_LSXSAP")]
        public string LSXSAP { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? EndFromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? EndToDate { get; set; }
        public DateTime? EndDateDSX { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal? PlanQuantity { get; set; }
        public decimal? CompletedQuantity { get; set; }
        public string CDL { get; set; }
        public string CDN { get; set; }
        public decimal? SLCTKH { get; set; }
        public decimal? SLCTTT { get; set; }
        public string TransferName { get; set; }
        public DateTime? CompletedPreviousStepTime { get; set; }
        public DateTime? FromTime { get; set; }
        public int? TransferWaitTime { get; set; }
        public string Warning { get; set; }
        public string CommonDate { get; set; }
        public string WarningSearch { get; set; }
        public string CDLCode { get; set; }
        public int? CDLIndex { get; set; }
        public string CDNCode { get; set; }
        public int? CDNIndex { get; set; }
        public string ImageUrl { get; set; }
        public decimal? SLTPHT { get; set; }
    }
}
