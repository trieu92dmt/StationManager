using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels.Marketing
{
    public class CampaignReportViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalMember")]
        public int TotalMember { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalMailSend")]
        public int TotalMailSend { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalMailBounce")]
        public int TotalMailBounce { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalMailOpened")]
        public int TotalMailOpened { get; set; }
    }
}
