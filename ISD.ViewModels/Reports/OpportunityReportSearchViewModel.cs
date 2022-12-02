using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class OpportunityReportSearchViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TabInvestor")]
        public Guid? Investor { get; set; }
        public string InvestorName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TabConsultingDesign")]
        public Guid? Designer { get; set; }
        public string DesignerName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Profile_SaleOfficeCode")]
        public string SaleOfficeCode { get; set; }
        public bool IsView { get; set; }
    }
}
