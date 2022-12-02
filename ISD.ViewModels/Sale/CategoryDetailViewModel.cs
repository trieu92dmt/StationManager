using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ISD.ViewModels.Sale
{
    public class CategoryDetailViewModel
    {
        public Guid CategoryId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Brand_BrandCode")]
        public string BrandCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Brand_BrandName")]
        public string BrandName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Category_CategoryCodeDetail")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [Remote("CheckExistingCategoryCode", "CategoryDetail", AdditionalFields = "CategoryCodeValid,CategoryCode,ParentCategoryId", HttpMethod = "POST", ErrorMessageResourceName = "Validation_Already_Exists", ErrorMessageResourceType = typeof(Resources.LanguageResource))]
        public string CategoryCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Category_CategoryNameDetail")]
        public string CategoryName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Category_CategoryCode")]
        public string MaterialGroupCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Category_CategoryName")]
        public string MaterialGroupName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
        public Nullable<int> OrderIndex { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_Brand")]
        [Required(ErrorMessage = "Vui lòng nhập thông tin nhóm sản phẩm ")]
        public Guid? ParentCategoryId { get; set; }



        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductTypeId")]
        public int? ProductTypeId { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_Brand")]
        public string ParentCategoryName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public Nullable<bool> Actived { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Category_ImageUrl", Description = "Category_ImageUrl_Hint")]
        public string ImageUrl { get; set; }

        public bool isDelete { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Category_TrackTrend")]
        public bool? IsTrackTrend { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CategoryCode")]
        [Remote("CheckExistingCategoryCode", "Category", AdditionalFields = "CategoryCodeValid,CategoryCode", HttpMethod = "POST", ErrorMessageResourceName = "Validation_Already_Exists", ErrorMessageResourceType = typeof(Resources.LanguageResource))]
        public Guid? CategoryTypeId { get; set; }
        public string ADN { get; set; }
    }
}
