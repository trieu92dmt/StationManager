using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class SO100ReportViewModel
    {
        public int? STT { get; set; }
        //SaleOrg
        public string SaleOrg { get; set; }
        public string WERKS { get; set; }

        //SO number
        public long OderIndex { get; set; } 
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VBELN")]
        public string VBELN { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_AUART")]
        public string AUART { get; set; }
        //SO Line
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_POSNR")]
        public string POSNR { get; set; }
        //Item component
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_BOM_POSNR")]
        public string ItemComponent { get; set; }
        //Mã nguyên liệu
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_IDNRK")]
        public string IDNRK_MES { get; set; }
        // MType
        public string MTART { get; set; }
        //Tên nguyên liệu
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MAKTX")]
        public string MAKTX { get; set; }
        //Unit
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MEINS")]
        public string MEINS { get; set; }
        //Số lượng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MENGE")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public Nullable<decimal> MENGE { get; set; }
        public Nullable<decimal> MENGE_WITH_SCRAP { get; set; }

        public decimal? SLXVL { get; set; }
        public decimal? SLTFSO { get; set; }
        public decimal? SL311 { get; set; }
        public decimal? SL311HQ { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_UMVKZ")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? UMVKZ { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_UMVKZ")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? KBMENG { get; set; }
        //SLYCPR
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_SLYCPR")]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        public decimal? SLYCPR { get; set; }

        //Đang mua
        public decimal? BSMNG_SUM_PO { get; set; }
        //Số nhập kho PO
        public decimal? GR_QTY { get; set; }

        public decimal? CanThem { get; set; }
        public decimal? CanThemWithScrap { get; set; }


        public bool IsView { get; set; }
        //Danh sách VBELN
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public List<string> LSXSAPList { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXDT")]
        public string LSXDT { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXC")]
        public string LSXSAP { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Summary_DSX")]
        public string DSX { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_PYCSXDT")]
        public string PYCSXDT { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Document_Date_From")]
        public DateTime? Document_Date_From { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Document_Date_To")]
        public DateTime? Document_Date_To { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Created_On_From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Created_On_From { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Created_On_To")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Created_On_To { get; set; }
        public Guid? DSXId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public decimal? SLYCPRSOItem { get; set; }
        public decimal? SLYCPRConLai { get; set; }
        public decimal? MENGE_SU { get; set; }
        public string MEINS_SU { get; set; }
        public decimal? MENGE_UoM { get; set; }
        public string MEINS_UoM { get; set; }
        public decimal? DangMuaSOItem { get; set; }
        public decimal? DangMuaConLai { get; set; }
        public decimal? SNKTPOSOItem { get; set; }
        public decimal? SNKTPOConLai { get; set; }
        public decimal? SLXVLSOItem { get; set; }
        public decimal? SLXVLConLai { get; set; }
        public decimal? SLTFSOItem { get; set; }
        public decimal? SLTFConLai { get; set; }
        public decimal? SL311SOItem { get; set; }
        public decimal? SL311ConLai { get; set; }
        public decimal? SL311HQSOItem { get; set; }
        public decimal? SL311HQConLai { get; set; }
        public string MATNR { get; set; }
        public string WBS { get; set; }
        public decimal? SLTPCKCTSO { get; set; }
        public decimal? SLTPCKCTSOItem { get; set; }
        public decimal? SLTPCKCTConLai { get; set; }
        public decimal? SLNVLTTSO { get; set; }
        public decimal? SLNVLTTSOItem { get; set; }
        public decimal? SLNVLTTConlai { get; set; }
        public decimal? SLNVLTruTonSO { get; set; }
        public decimal? SLNVLTruTonChuaCoDonHang { get; set; }
        public decimal? DinhMucBOM { get; set; }
        public decimal? Scrap { get; set; }
        public string MATNR_COMBO { get; set; }
    }
}
