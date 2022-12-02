using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class TimeLineSearchViewModel
    {
        
        public Guid CompanyId { set; get; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Company_CompanyName")]
        public Guid CompanyName { set; get; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Summary")]
        public string Summary { set; get; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Summary_D")]
        public string Summary_Dot { set; get; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SaleOrderHeader")]
        public string VBELN { set; get; }  
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MaterialCode")]
        public string Material { set; get; }
        //Common Ngày bắt đầu
        public string StartCommonDate { get; set; }
        //Ngày bắt đầu - Từ ngày
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? StartFromDate { get; set; }
        //Ngày bắt đầu- đến ngày
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? StartToDate { get; set; }
        //Common Ngày kết thúc 
        public string EndCommonDate { get; set; }
        //Ngày Ngày kết - Từ ngày
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? EndFromDate { get; set; }
        //Ngày Ngày kết - đến ngày
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? EndToDate { get; set; } 
        
        //Common Ngày bắt đầu điều chỉnh
        public string StartDCCommonDate { get; set; }
        //Ngày bắt đầu điều chỉnh - Từ ngày
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? StartDCFromDate { get; set; }
        //Ngày bắt đầu điều chỉnh - đến ngày
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? StartDCToDate { get; set; }
        //Common Ngày kết thúc điều chỉnh
        public string EndDCCommonDate { get; set; }
        //Ngày Ngày kết điều chỉnh - Từ ngày
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? EndDCFromDate { get; set; }
        //Ngày Ngày kết điều chỉnh - đến ngày
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? EndDCToDate { get; set; }

        public bool IsView { get; set; }

        //Trạng thái hoạt động
        [Display(Name = "Trạng thái hoạt động")]
        public bool? Actived { get; set; }
        [Display(Name = "Trạng thái hoạt động")]
        public bool? isDeleted { get; set; }

    }
}
