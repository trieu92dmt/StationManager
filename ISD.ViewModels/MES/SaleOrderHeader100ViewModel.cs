using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class SaleOrderHeader100ViewModel : SaleOrderHeader100Model
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Document_Date_From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Document_Date_From { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Document_Date_To")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Document_Date_To { get; set; }

        public string Plant { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Created_On_From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Created_On_From { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Created_On_To")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Created_On_To { get; set; }

        public int? STT { get; set; }
        public string SONumber { get { return VBELN; } }
        [Display(Name = "LSX ĐT")]
        public string LSXDT { get; set; }
    }
}
