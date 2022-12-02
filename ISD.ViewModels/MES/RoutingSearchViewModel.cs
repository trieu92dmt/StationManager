using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class RoutingSearchViewModel
    {
        public System.Guid RoutingInventorId { get; set; }
        public string MANDT { get; set; }
        public string VERSO { get; set; }
        public string ARBPL { get; set; }
        public string ARBPL_SUB { get; set; }
        public string WERKS { get; set; }
        public string MATNR { get; set; }
        //Mã chi tiết
        public string ITMNO { get; set; }
        //Tên chi tiết
        public string KTEXT { get; set; }
        //Mã nguyên liệu
        public string IDNRK { get; set; }
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
        //Tên nguyên liệu
        public string MAKTX { get; set; }
        public string BMEIN { get; set; }
        public Nullable<decimal> BMSCH { get; set; }
        public string PLNNR { get; set; }
        //Công đoạn con
        public string LTXA1 { get; set; }
        public string MEINS { get; set; }
        public Nullable<decimal> MENGE { get; set; }
        public Nullable<decimal> VGW01 { get; set; }
        public Nullable<decimal> VGW02 { get; set; }
        public Nullable<decimal> VGW03 { get; set; }
        public Nullable<decimal> VGW04 { get; set; }
        public Nullable<decimal> VGW05 { get; set; }
        public Nullable<decimal> VGW06 { get; set; }
        public string ACTON { get; set; }
        public Nullable<System.DateTime> ANDAT { get; set; }
        public string AENAM { get; set; }
        public Nullable<System.DateTime> AEDAT { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> LastEditTime { get; set; }
        public Guid? WorkShopId { get; set; }
        public string StepName { set; get; }
        public string StepCode { set; get; }
        public bool? Actived { get; set; }
        public Nullable<System.Guid> StockReceivingId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Promotion_ProductName")]
        public Nullable<System.Guid> ProductId { get; set; }
        public string ProductAttributes { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Warehouse_Stock")]
        public Nullable<System.Guid> StockId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DocumentDate")]
        public Nullable<int> DateKey { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SaleOrder_Quantity")]
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> Quantity_D { get; set; }
        public Nullable<decimal> Quantity_KD { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price")]
        public Nullable<decimal> Price { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SaleOrder_Total")]
        public Nullable<decimal> UnitPrice { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
        public string Note { get; set; }
        public Nullable<System.Guid> CustomerReference { get; set; }
        public string StockRecevingType { get; set; }

        public string check { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_WorkCenter")]
        public string WorkCenter { get; set; }
        public string Plant { get; set; }

    }
}
