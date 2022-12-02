using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MES
{
    public class ProductionOrderDetailViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VBELN")]
        public string VBELN { get; set; }   
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ITMNO")]
        public string ProductAttributes { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ITMNO")]
        public string ITMNO { get; set; }   
        public Guid? RoutingInventorId { get; set; }
        public Guid TaskId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_KTEXT")]
        public string KTEXT { get; set; }
        public int? Phase { get; set; }

        public string POT12 { get; set; }
        public decimal? Qty { get; set; }
        public string StepCode { get; set; }
        public string StockCode { get; set; }
        public string StepName { get; set; }
        public int? OrderIndex { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? Quantity_DLD { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? Quantity_D { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? Quantity_DLKD { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? Quantity_KD { get; set; }
        public Guid? CustomerReference { get; set; }
        public string Unit { get; set; }
        public bool Check { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Document_Date_From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? WorkDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Document_Date_From")]
        public DateTime? From { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Document_Date_From")]
        public DateTime? To { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }  
        public DateTime? CreateTime { get; set; }
        public string CreateByName { get; set; }
        public DateTime? FromDate
        {
            get
            {
                if (FromTime.HasValue && FromTime.Value.Date != ToTime.Value.Date)
                {
                    return FromTime.Value;
                }
                else
                {
                    return null;
                };
            }
        }
        public DateTime? ToDate { get {
                if (ToTime.HasValue)
                {
                    return ToTime.Value;
                } else {
                    return null;
                };
            } 
        }

        public string StockRecevingType { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Quantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? BMSCH { get; set; }
        public string radio { get; set; }

        public DateTime? FromTimeToSave { 
            get {
                DateTime? ret = null;
                if (WorkDate.HasValue)
                {
                    ret = WorkDate;
                    if (FromTime.HasValue)
                    {
                        ret = new DateTime(WorkDate.Value.Year, WorkDate.Value.Month, WorkDate.Value.Day, FromTime.Value.Hour, FromTime.Value.Minute, 0);
                    }
                }
                return ret;
            } 
        }

        public DateTime? ToTimeToSave
        {
            get
            {
                DateTime? ret = null;
                if (WorkDate.HasValue)
                {
                    ret = WorkDate;
                    if (ToTime.HasValue)
                    {
                        ret = new DateTime(WorkDate.Value.Year, WorkDate.Value.Month, WorkDate.Value.Day, ToTime.Value.Hour, ToTime.Value.Minute, 0);
                    }
                }
                return ret;
            }
        }
    }
}
