using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MES
{
    public class ProductionOrderViewModel : ProductionOrderModel
    {
        //Plant
        public string SaleOrg { get; set; }
        //Print
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuantityPrinted")]
        public int? QuantityPrinted { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuantityPrintMore")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public int? QuantityPrintMore { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EffectiveDate")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EffectiveDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TimesIssued")]
        public int? TimesIssued { get; set; }
        [Display(Name = "LSX ĐT")]
        public string LSXDT
        {
            get
            {
                return ZZLSX;
            }
        }
        [Display(Name = "Số đợt LSX")]
        public int? SoDSX { get; set; }
        [Display(Name = "Số LSX SAP")]
        public int? SoLSXSAP { get; set; }

        [Display(Name = "Số cont")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? SoCont { get; set; }
        [Display(Name = "BĐ DK")]
        public DateTime? BDDK
        {
            get
            {
                return GSTRS;
            }
        }
        //KTDK
        [Display(Name = "KT DK")]
        public DateTime? KTDK
        {
            get
            {
                return GLTRS;
            }
        }
        [Display(Name = "BĐ ĐC")]
        public DateTime? BDDC { get; set; }
        //KTĐC
        [Display(Name = "KT ĐC")]
        public DateTime? KTDC { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Duration")]
        public int? Duration
        {
            get
            {
                //if (GLTRS.HasValue && GSTRS.HasValue)
                //{
                //    return (int)(GLTRS - GSTRS).Value.TotalDays + 1;
                //}
                //else
                //{
                //    return 0;
                //}
                if (KTDC.HasValue && BDDC.HasValue)
                {
                    return (int)(KTDC - BDDC).Value.TotalDays + 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
    public class LSXDTExportViewModel
    {
        public string ZZLSX { get; set; }
        public DateTime? GSTRS { get; set; }
        public DateTime? GLTRS { get; set; }
        [Display(Name = "LSX ĐT")]
        public string LSXDT
        {
            get
            {
                return ZZLSX;
            }
        }
        [Display(Name = "Số đợt LSX")]
        public int? SoDSX { get; set; }
        [Display(Name = "Số LSX SAP")]
        public int? SoLSXSAP { get; set; }

        [Display(Name = "Số cont")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? SoCont { get; set; }
        [Display(Name = "BĐ DK")]
        public DateTime? BDDK
        {
            get
            {
                return GSTRS;
            }
        }
        //KTDK
        [Display(Name = "KT DK")]
        public DateTime? KTDK
        {
            get
            {
                return GLTRS;
            }
        }
        [Display(Name = "BĐ ĐC")]
        public DateTime? BDDC { get; set; }
        //KTĐC
        [Display(Name = "KT ĐC")]
        public DateTime? KTDC { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Duration")]
        public int? Duration
        {
            get
            {
                if (KTDC.HasValue && BDDC.HasValue)
                {
                    return (int)(KTDC - BDDC).Value.TotalDays + 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
