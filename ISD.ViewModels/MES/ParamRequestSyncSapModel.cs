using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MES
{
    public class ParamRequestSyncSapModel
    {
        public string CompanyCode { get; set; }
        public string MATNR { get; set; }
        public string ProVer { get; set; }
        public string IUpdate { get; set; }
        public string INew { get; set; }
        public string IRecord { get; set; }
        public string STLAN { get; set; }
        public string STLAL { get; set; }
        public string STLNR { get; set; }
        public string VBELN { get; set; }
        public string AUFNR { get; set; }
        public string BEDAE { get; set; }
        public string RSNUM { get; set; }
        public string IType { get; set; }
        public string FromDate { get; set; }
        public string FromTime { get; set; }
        public string ToDate { get; set; }
        public string ToTime { get; set; }
        public string IBANFN { get; set; }
        public string IEBELN { get; set; }
        public string IZZLSX { get; set; }
    }

    public class ParamRequestSyncRoutingModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_WERKS")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string WERKS { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_MATNR")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string MATNR { get; set; }
    }
    public class ParamRequestSyncBOMModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_WERKS")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string WERKS { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_MATNR")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string MATNR { get; set; }
    }

    public class ParamRequestSyncMaterialModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_WERKS")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string WERKS { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_MATNR")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string MATNR { get; set; }
    }

    public class ParamRequestSyncPPOrderModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_WERKS")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string WERKS { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_PPOrder")]
        //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string PPOrder { get; set; }
        [Display(Name = "LSX ĐT")]
        //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string LSXDT { get; set; }
    }

    public class ParamRequestSyncPRPOModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_WERKS")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string WERKS { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_VBELN")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string VBELN { get; set; }
    }

    public class ParamRequestSyncSOModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_WERKS")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string WERKS { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_VBELN")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string VBELN { get; set; }
    }

    public class ParamRequestSyncSOTEXTPRModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_WERKS")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string WERKS { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_BANFN")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string BANFN { get; set; }
    }

    public class ParamRequestSyncPOTEXT_SO_PRModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_WERKS")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string WERKS { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SyncData_EBELN")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string EBELN { get; set; }
    }
}
