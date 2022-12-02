using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.Work
{
    public class DivisionOfTaskViewModel : ProductionManagementViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT")]
        public string LSXDT { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_StartDate")]
        public DateTime? LSXDT_StartDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_EndDate")]
        public DateTime? LSXDT_EndDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_EstimateDate")]
        public DateTime? LSXDT_EstimateDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXDT_Duration")]
        public int? LSXDT_Duration
        {

            get
            {
                if (LSXDT_StartDate.HasValue && LSXDT_EstimateDate.HasValue)
                {
                    return (int)(LSXDT_EstimateDate - LSXDT_StartDate).Value.TotalDays;
                }
                else
                {
                    return 0;
                }
            }

        }
        
       
        
        
    }
}
