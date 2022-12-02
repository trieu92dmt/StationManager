using System;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class TransferDetailViewModel
    {
        public System.Guid TransferDetailId { get; set; }
        public int? STT { get; set; }
        public Nullable<System.Guid> TransferId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Promotion_ProductName")]
        public Nullable<System.Guid> ProductId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Stock_ProductCode")]
        public string ProductCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductName")]
        public string ProductName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Serial")]
        public string Serial { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Transfer_FromStock")]
        public Nullable<System.Guid> FromStockId { get; set; }
        public string FromStockCode { get; set; }
        public string FromStockName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Tranfer_ToStock")]
        public Nullable<System.Guid> ToStockId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StockCode")]
        public string ToStockCode { get; set; }

        public string ToStockName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DocumentDate")]
        public Nullable<int> DateKey { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_Quantity")]
        public Nullable<decimal> QuantinyOnHand { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SaleOrder_Quantity")]
        [DisplayFormat(DataFormatString = "{0:#}")]
        public Nullable<decimal> Quantity { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price")]
        public Nullable<decimal> Price { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SaleOrder_Total")]
        public Nullable<decimal> UnitPrice { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
        public string DetailNote { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TransferredQuantity")]
        public Nullable<decimal> TransferredQuantity { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RemainingQuantity")]
        public Nullable<decimal> RemainingQuantity { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RequestQuantity")]
        public Nullable<decimal> RequestQuantity { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TransferQuantity")]
        public Nullable<decimal> TransferQuantity { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_Store")]
        public Nullable<Guid> StoreId { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public string ProductAttributes { get; set; }
        public Nullable<System.Guid> FromCustomerReference { get; set; }
        public Nullable<System.Guid> CustomerReference { get; set; }
        public string StockRecevingType { get; set; }
        public Nullable<System.DateTime> FromTime { get; set; }
        public Nullable<System.DateTime> ToTime { get; set; }
        public Nullable<int> Phase { get; set; }
        public string MovementType { get; set; }
        public Nullable<System.Guid> DepartmentId { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }


    }
}