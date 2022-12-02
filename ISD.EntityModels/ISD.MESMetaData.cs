using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ISD.EntityModels
{
    [MetadataTypeAttribute(typeof(ThucThiLenhSanXuatModel.MetaData))]
    public partial class ThucThiLenhSanXuatModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateTime")]
            public Nullable<System.DateTime> CreateTime { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToStockCode")]
            public string ToStockCode { get; set; }
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> ProductAttributesQty { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TaskStatusId")]
            public Guid? TaskStatusId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
            public Nullable<System.Guid> CreateBy { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(WorkShopModel.MetaData))]
    public partial class WorkShopModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_WorkShop")]
            public System.Guid WorkShopId { get; set; }

            public Nullable<System.Guid> CompanyId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkShop_WorkShopCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [Remote("CheckExistingWorkShopCode", "WorkShop", AdditionalFields = "WorkShopCodeValid", HttpMethod = "POST", ErrorMessageResourceName = "Validation_Already_Exists", ErrorMessageResourceType = typeof(Resources.LanguageResource))]
            public string WorkShopCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkShop_WorkShopName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string WorkShopName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            [RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            public Nullable<int> OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }

            public string CreatedUser { get; set; }
            public Nullable<System.DateTime> CreatedTime { get; set; }
            public string LastModifiedUser { get; set; }
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(RoutingModel.MetaData))]
    public partial class RoutingModel
    {
        internal sealed class MetaData
        {
            public System.Guid StepId { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string StepCode { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string StepName { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            [RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            public Nullable<int> OrderIndex { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }
            public Nullable<System.Guid> CreateBy { get; set; }
            public Nullable<System.DateTime> CreateTime { get; set; }
            public Nullable<System.Guid> LastEditBy { get; set; }
            public Nullable<System.DateTime> LastEditTime { get; set; }


            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NumberOfMachines")]
            public Nullable<int> NumberOfMachines { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(SaleOrderHeader100Model.MetaData))]
    public partial class SaleOrderHeader100Model
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepCode")]
            public System.Guid SO100HeaderId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VBELN")]
            public string VBELN { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_BEDAE")]
            public string BEDAE { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_AUDAT")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> AUDAT { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERDAT")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> ERDAT { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERZET")]
            public Nullable<System.TimeSpan> ERZET { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERNAM")]
            public string ERNAM { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_AUART")]
            public string AUART { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VKORG")]
            public string VKORG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VTWEG")]
            public string VTWEG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_SPART")]
            public string SPART { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_BSTNK")]
            public string BSTNK { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_KUNNR")]
            public string KUNNR { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_PS_PSP_PNR")]
            public string PS_PSP_PNR { get; set; }  
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_PS_PSP_PNR")]
            public string PS_PSP_PNR_OUTPUT { get; set; }

            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VDATU")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> VDATU { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_LFGSK")]
            public string LFGSK { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateTime")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> CreateTime { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastEditTime")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> LastEditTime { get; set; }
        }
    }   
    
    [MetadataTypeAttribute(typeof(SaleOrderItem100Model.MetaData))]
    public partial class SaleOrderItem100Model
    {
        internal sealed class MetaData
        {
            public System.Guid SO100ItemId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VBELN")]
            public string VBELN { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_POSNR")]
            public string POSNR { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MATNR")]
            public string MATNR { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ARKTX")]
            public string ARKTX { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_BEDAE")]
            public string BEDAE { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_WERKS")]
            public string WERKS { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_KBMENG")]
            [DisplayFormat(DataFormatString = "{0:n3}")]
            public Nullable<decimal> KBMENG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_KLMENG")]
            [DisplayFormat(DataFormatString = "{0:n3}")]
            public Nullable<decimal> KLMENG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ETENR")]
            public Nullable<int> ETENR { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_EDATU")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> EDATU { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_WMENG")]
            [DisplayFormat(DataFormatString = "{0:n3}")]
            public Nullable<decimal> WMENG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VRKME")]
            public string VRKME { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_LMENG")]
            public Nullable<decimal> LMENG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_DLVQTY_BU")]
            public Nullable<decimal> DLVQTY_BU { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ORDQTY_BU")]
            public Nullable<decimal> ORDQTY_BU { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_SOBKZ")]
            public string SOBKZ { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_PS_PSP_PNR")]
            public string PS_PSP_PNR { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_PS_PSP_PNR")]
            public string PS_PSP_PNR_OUTPUT { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_UMVKZ")]
            public Nullable<decimal> UMVKZ { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_UMVKN")]
            public Nullable<decimal> UMVKN { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MEINS")]
            public string MEINS { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ABGRU")]
            public string ABGRU { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_GBSTA")]
            public string GBSTA { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERDAT")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> ERDAT { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERNAM")]
            public string ERNAM { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERZET")]
            public Nullable<System.TimeSpan> ERZET { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateTime")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> CreateTime { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastEditTime")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> LastEditTime { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_UPMAT")]
            public string UPMAT { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(SaleOrderHeader80Model.MetaData))]
    public partial class SaleOrderHeader80Model
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepCode")]
            public System.Guid SOHeaderId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VBELN")]
            public string VBELN { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_BEDAE")]
            public string BEDAE { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_AUDAT")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> AUDAT { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERDAT")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> ERDAT { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERZET")]
            public Nullable<System.TimeSpan> ERZET { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERNAM")]
            public string ERNAM { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_AUART")]
            public string AUART { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VKORG")]
            public string VKORG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VTWEG")]
            public string VTWEG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_SPART")]
            public string SPART { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_BSTNK")]
            public string BSTNK { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_KUNNR")]
            public string KUNNR { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_PS_PSP_PNR")]
            public string PS_PSP_PNR { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_PS_PSP_PNR")]
            public string PS_PSP_PNR_OUTPUT { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VDATU")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> VDATU { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_LFGSK")]
            public string LFGSK { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateTime")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> CreateTime { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastEditTime")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> LastEditTime { get; set; }
        }
    }   
    
    [MetadataTypeAttribute(typeof(SaleOrderItem80Model.MetaData))]
    public partial class SaleOrderItem80Model
    {
        internal sealed class MetaData
        {
            public System.Guid SOItemId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VBELN")]
            public string VBELN { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_POSNR")]
            public string POSNR { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MATNR")]
            public string MATNR { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ARKTX")]
            public string ARKTX { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_BEDAE")]
            public string BEDAE { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_WERKS")]
            public string WERKS { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_KBMENG")]
            [DisplayFormat(DataFormatString = "{0:n3}")]
            public Nullable<decimal> KBMENG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_KLMENG")]
            [DisplayFormat(DataFormatString = "{0:n3}")]
            public Nullable<decimal> KLMENG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ETENR")]
            public Nullable<int> ETENR { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_EDATU")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> EDATU { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_WMENG")]
            [DisplayFormat(DataFormatString = "{0:n3}")]
            public Nullable<decimal> WMENG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_VRKME")]
            public string VRKME { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_LMENG")]
            public Nullable<decimal> LMENG { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_DLVQTY_BU")]
            public Nullable<decimal> DLVQTY_BU { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ORDQTY_BU")]
            public Nullable<decimal> ORDQTY_BU { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_SOBKZ")]
            public string SOBKZ { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_PS_PSP_PNR")]

            public string PS_PSP_PNR { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_PS_PSP_PNR")]
            public string PS_PSP_PNR_OUTPUT { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_UMVKZ")]
            public Nullable<decimal> UMVKZ { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_UMVKN")]
            public Nullable<decimal> UMVKN { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_MEINS")]
            public string MEINS { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ABGRU")]
            public string ABGRU { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_GBSTA")]
            public string GBSTA { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERDAT")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> ERDAT { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERNAM")]
            public string ERNAM { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ERZET")]
            public Nullable<System.TimeSpan> ERZET { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateTime")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> CreateTime { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastEditTime")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> LastEditTime { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(HangTagModel.MetaData))]
    public partial class HangTagModel
    {
        internal sealed class MetaData
        {
            public System.Guid HangTagId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LSXSAP")]
            public string MassProductionOrder { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BatchPrinting")]
            public Nullable<int> BatchPrinting { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EffectiveDate")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            public Nullable<System.DateTime> EffectiveDate { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Barcode")]
            public string QRCode { get; set; }
            public string CreatedUser { get; set; }
            public Nullable<Guid> CustomerReference { get; set; }
            public Nullable<System.DateTime> CreatedTime { get; set; }
            public string LastModifiedUser { get; set; }
            public Nullable<System.DateTime> LastModifiedTime { get; set; }
        }
    }    
    
    [MetadataTypeAttribute(typeof(ProductionOrderModel.MetaData))]
    public partial class ProductionOrderModel
    {
        internal sealed class MetaData
        {
            public System.Guid ProductionOrderId { get; set; }
            public string AUFNR { get; set; }
            public Nullable<System.DateTime> GLTRP { get; set; }
            public Nullable<System.DateTime> GSTRP { get; set; }
            public Nullable<System.DateTime> FTRMS { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_GLTRS")]
            public Nullable<System.DateTime> GLTRS { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_GSTRS")]
            public Nullable<System.DateTime> GSTRS { get; set; }
            public Nullable<int> RSNUM { get; set; }
            public Nullable<decimal> GASMG { get; set; }
            public Nullable<decimal> GAMNG { get; set; }
            public string KDAUF { get; set; }
            public string KDPOS { get; set; }
            public Nullable<decimal> PSMNG { get; set; }
            public Nullable<decimal> WEMNG { get; set; }
            public string AMEIN { get; set; }
            public string MEINS { get; set; }
            public string MATNR { get; set; }
            public Nullable<decimal> PAMNG { get; set; }
            public Nullable<decimal> PGMNG { get; set; }
            public Nullable<System.DateTime> LTRMI { get; set; }
            public Nullable<decimal> UEBTO { get; set; }
            public string UEBTK { get; set; }
            public Nullable<decimal> UNTTO { get; set; }
            public string INSMK { get; set; }
            public string DWERK { get; set; }
            public string DAUAT { get; set; }
            public string ZZSLANSUA { get; set; }
            public string ZZGHICHU { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_ZZLSX")]
            public string ZZLSX { get; set; }
            public string VERID { get; set; }
            public Nullable<System.DateTime> CreateTime { get; set; }
            public Nullable<System.DateTime> LastEditTime { get; set; }
        }
    }
    [MetadataTypeAttribute(typeof(NFCCheckInOutModel.MetaData))]
    public partial class NFCCheckInOutModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_CheckIn")]
            public Guid CheckInId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_CheckInDate")]
            public DateTime? CheckInDate { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_SerialTag")]
            public string SerialTag { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MES_Description")]
            public string Description { get; set; }
            public Nullable<System.Guid> WorkingDepartment { get; set; }
            public Nullable<System.Guid> CheckInOutDepartment { get; set; }
        }
    }


    [MetadataTypeAttribute(typeof(PlantRoutingConfigModel.MetaData))]
    public partial class PlantRoutingConfigModel
    {
        internal sealed class MetaData
        {
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_PlantRoutingCode")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public int PlantRoutingCode { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_PlantRoutingName")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            public string PlantRoutingName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_PlantRoutingGroup")]
            public string PlantRoutingGroup { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
            [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
            [RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
            public Nullable<int> OrderIndex { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
            public Nullable<bool> Actived { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_FromData")]
            public string FromData { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute1")]
            public string Attribute1 { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute2")]
            public string Attribute2 { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_LeadTimeType")]
            public Nullable<bool> LeadTimeType { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_LeadTime")]
            public Nullable<int> LeadTime { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_FromDate")]
            public string FromDate { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_ToDate")]
            public string ToDate { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_Condition")]
            public string Condition { get; set; }
        }
    }
    
    [MetadataTypeAttribute(typeof(QualityControlModel.MetaData))]
    public partial class QualityControlModel
    {
        internal sealed class MetaData
        {
            public System.Guid QualityControlId { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_Code")]
            public int QualityControlCode { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_Store")]
            public string SaleOrgCode { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_WorkShop")]
            public string WorkShopCode { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_WorkCenter")]
            public string WorkCenterCode { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_SO")]
            public string VBELN { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_LSXDT")]
            public string LSXDT { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_LSXSAP")]
            public string LSXSAP { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_ProductAttribute")]
            public string ProductAttribute { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_ProductCode")]
            public string ProductCode { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_Product")]
            public string ProductName { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_CustomerReference")]
            public Nullable<System.Guid> CustomerReference { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_ConfirmDate")]
            public Nullable<System.DateTime> ConfirmDate { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_QuantityConfirm")]
            public Nullable<decimal> QuantityConfirm { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_QualityDate")]
            public Nullable<System.DateTime> QualityDate { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_QualityChecker")]
            public Nullable<System.Guid> QualityChecker { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_QualityType")]
            public string QualityType { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_InspectionLotQuantity")]
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> InspectionLotQuantity { get; set; }
            public string Environmental { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_Descriptions")]
            public string Descriptions { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_Status")]
            public Nullable<bool> Status { get; set; }
            [Display(ResourceType = typeof(Resources.LanguageResource), Name = "QuanlityControl_Result")]
            public string Result { get; set; }
            public Nullable<System.DateTime> CreateTime { get; set; }
            public Nullable<System.Guid> CreateBy { get; set; }
            public Nullable<System.DateTime> LastEditTime { get; set; }
            public Nullable<System.Guid> LastEditBy { get; set; }
        }
    }
    
    [MetadataTypeAttribute(typeof(QualityControl_Error_Mapping.MetaData))]
    public partial class QualityControl_Error_Mapping
    {
        internal sealed class MetaData
        {
            public Nullable<System.Guid> QuanlityControl_Error_Id { get; set; }
            public Nullable<System.Guid> QualityControlId { get; set; }
            public string CatalogCode { get; set; }
            public string LevelError { get; set; }
            [DisplayFormat(DataFormatString = "{0:n0}")]
            public Nullable<decimal> QuantityError { get; set; }
            public string Notes { get; set; }
        }
    }

}
