using System;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class ProfileGroupReportViewModel
    {
        [Display(Name = "Mã nhóm KH")]
        public string ProfileGroupCode { get; set; }

        [Display(Name = "Nhóm KH")]
        public string ProfileGroupName { get; set; }

        [Display(Name = "Số lượng")]
        public int? NumberOfProfiles { get; set; }

        [Display(Name = "%")]
        public decimal? PercentOfProfiles { get; set; }
    }
}
