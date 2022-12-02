using System;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class ProductSearchViewModel
    {
        public System.Guid ProductId { get; set; }
        
        //Product details
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_ProductCode")]
        public string SearchProductCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_ERPProductCode")]
        public string SearchERPProductCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_ProductName")]
        public string SearchProductName { get; set; }

        //Brand
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_Brand")]
        public System.Guid? SearchBrandId { get; set; }

        //Parent Category
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_Brand")]
        public System.Guid? SearchParentCategoryId { get; set; }

        //Category
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_Category")]
        public System.Guid? SearchCategoryId { get; set; }

        //Configuration
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_Configuration")]
        public System.Guid? SearchConfigurationId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_isHot")]
        public Nullable<bool> isHot { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public Nullable<bool> Actived { get; set; }
        public Nullable<bool> HasRouting { get; set; }

        [Display(Name = "BOM")]
        public bool? HasBOMSAP { get; set; }
        [Display(Name = "BOM Inventer")]
        public bool? HasBOMInventer { get; set; }
        [Display(Name = "Bản vẽ")]
        public bool? HasDrawing { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_ProductCode")]

        public string ProductCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_ERPProductCode")]

        public string ERPProductCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_ProductName")]

        public string ProductName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_Brand")]

        public Guid? BrandId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_Category")]

        public Guid? CategoryId { get; set; }
        
        public Guid? ConfigurationId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_Brand")]

        public Guid? ParentCategoryId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Unit")]
        public string Unit { get; set; }
        public string CategoryCode { get; set; }
        public Guid? CategoryDetailId { get; set; }
        [Display(Name = "Quá hạn ngày bảo trì")]
        public bool? HasExpiredDate { get; set; }
        [Display(Name = "Quá số lần dập")]
        public bool? HasExpiredTimes { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductType")]
        public bool? Type { get; set; }
    }
}

