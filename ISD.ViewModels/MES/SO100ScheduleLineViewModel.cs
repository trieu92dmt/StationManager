using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class SO100ScheduleLineViewModel
    {
        public System.Guid SO100ScheduleLineId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VBELN")]
        public string VBELN { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_POSNR")]
        public string POSNR { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ETENR")]
        public string ETENR { get; set; }
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_LFREL")]
        public string LFREL { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_EDATU")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> EDATU { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_WMENG")]
        public Nullable<decimal> WMENG { get; set; }
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_BMENG")]
        public Nullable<decimal> BMENG { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VRKME")]
        public string VRKME { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_LMENG")]
        public Nullable<decimal> LMENG { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MEINS")]
        public string MEINS { get; set; }
        public string LIFSP { get; set; }
   
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_DLVQTY_BU")]
        public Nullable<decimal> DLVQTY_BU { get; set; }
        public Nullable<decimal> DLVQTY_SU { get; set; }
        public Nullable<decimal> OCDQTY_BU { get; set; }
        public Nullable<decimal> OCDQTY_SU { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ORDQTY_BU")]
        public Nullable<decimal> ORDQTY_BU { get; set; }
        public Nullable<decimal> ORDQTY_SU { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")] 
        public Nullable<System.DateTime> CREA_DLVDATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")] 
        public Nullable<System.DateTime> REQ_DLVDATE { get; set; }

    }
}
