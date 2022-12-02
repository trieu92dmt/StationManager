using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class BomDetailViewModel
    {
        public System.Guid BomDetailId { get; set; }
        public string MANDT { get; set; }
        [Display(Name = "BOM category")]
        public string STLTY { get; set; }
        [Display(Name = "BOM")]
        public string STLNR { get; set; }
        [Display(Name = "Item node")]
        public string STLKN { get; set; }
        [Display(Name = "Counter")]
        public string STPOZ { get; set; }
        [Display(Name = "Valid From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DATUV { get; set; }
        [Display(Name = "Deletion Ind.")]
        public string LKENZ { get; set; }
        [Display(Name = "Created on")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ANDAT { get; set; }
        [Display(Name = "Created by")]
        public string ANNAM { get; set; }
        //Component
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_IDNRK")]
        public string IDNRK { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_POSNR")]
        public string SOLineNumber { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MATNR")]
        public string MATNR { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MATNR_DES")]
        public string MATNR_DES { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_BOMBaseQty")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? BOMBaseQty { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_BOMBaseUnit")]
        public string BOMBaseUnit { get; set; }
        public int? IDNRK_DISPLAY
        {
            get
            {
                int ret;
                if (int.TryParse(IDNRK, out ret))
                {
                    return ret;
                }
                return null;
            }
        }
        //Description
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MAKTX")]
        public string MAKTX { get; set; }
        [Display(Name = "Material type")]
        public string MTART { get; set; }
        [Display(Name = "Item category")]
        public string POSTP { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_BOM_POSNR")]
        public string POSNR { get; set; }
        //Quantity
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MENGE")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public Nullable<decimal> MENGE { get; set; }
        //Unit
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MEINS")]
        public string MEINS { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_KLMENG")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? KLMENG { get; set; }
        //Component Scrap (%)
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_AUSCH")]
        public Nullable<decimal> AUSCH { get; set; }
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? Total
        {
            get
            {
                return (KLMENG ?? 0) * (MENGE ?? 0) / (BOMBaseQty ?? 1);
            }
        }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_TotalWithScrap")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? TotalWithScrap
        {
            get
            {
                return (KLMENG ?? 0) * (MENGE ?? 0) / (BOMBaseQty ?? 1) + ((KLMENG ?? 0) * (MENGE ?? 0) / (BOMBaseQty ?? 1) * ((AUSCH ?? 0) / 100));
            }
        }
        [Display(Name = "Operation Scrap")]
        public Nullable<decimal> AVOAU { get; set; }
        [Display(Name = "Item Text 1")]
        public string POTX1 { get; set; }
        [Display(Name = "Item Text 2")]
        public string POTX2 { get; set; }
        public string WERKS { get; set; }
        [Display(Name = "MATNR")]
        public string MaterialNumber { get { return MATNR; } }

        [Display(Name = "Change number")]
        public string AENNR { get; set; }
        public string LSXD { get; set; }
        public string LSXC { get; set; }
        public string LSXDT { get; set; }
      
       
        public string POSNR_DISPLAY
        {
            get
            {
                if (!string.IsNullOrEmpty(POSNR))
                {
                    return POSNR.TrimStart(new Char[] { '0' });
                }
                return string.Empty;
            }
        }
       
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_UMVKZ")]
        public decimal? UMVKZ { get; set; }

        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> LastEditTime { get; set; }
       
    }
}
