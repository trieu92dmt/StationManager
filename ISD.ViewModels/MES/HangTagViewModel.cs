using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MES
{
    public class HangTagViewModel : HangTagModel
    {
        //Print
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuantityPrinted")]
        public int? QuantityPrinted { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuantityPrintMore")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public int? QuantityPrintMore { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public string ProductSOCode { get; set; }
        public string ProductSOName { get; set; }
        public string LSXDT { get; set; }
        public string LSXD { get; set; }
        public int? Qty { get; set; }
        public decimal? Number2 { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SL
        {
            get
            {
                decimal? ret;
                if (Number2.HasValue)
                {
                    ret = Number2.Value;
                }
                else
                {
                    ret = 0;
                }
                return ret;
            }
        }
        [Display(Name = "Chi tiết")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string ChiTiet { get; set; }
        public string CompanyCode { get; set; }
    }
}
