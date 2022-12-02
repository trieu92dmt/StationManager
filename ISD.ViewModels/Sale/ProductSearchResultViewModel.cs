using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ProductSearchResultViewModel
    {
        public int STT { get; set; }
        public Guid ProductId { get; set; }

        public string ERPProductCode { get; set; }
        [Display(Name ="Mã sản phẩm")]
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public Nullable<int> OrderIndex { get; set; }
        public Guid? ParentCategoryId { get; set; }

        public string ParentCategoryName { get; set; }
        [Display(Name = "Mã phân cấp")]
        public string ParentCategoryCode { get; set; }
        public Guid? CategoryId { get; set; }

        public string CategoryName { get; set; }
        [Display(Name = "Mã nhóm sản phẩm")]
        public string CategoryCode { get; set; }
        public Guid? CategoryDetailId { get; set; }

        public string CategoryDetailName { get; set; }
        [Display(Name = "Mã nhóm sản phẩm chi tiết")]
        public string CategoryDetailCode { get; set; }
        public decimal? Price { get; set; }
        public string CompanyCode { get; set; }
        public bool Actived { get; set; }
        public bool? HasRouting { get; set; }
        [Display(Name = "BOM")]
        public bool? HasBOMSAP { get; set; }
        [Display(Name = "BOM Inventer")]
        public bool? HasBOMInventer { get; set; }
        [Display(Name = "Bản vẽ")]
        public bool? HasDrawing { get; set; }
        public decimal? CountRouting { get; set; }
        public decimal? CountDrawing { get; set; }
        public decimal? CoutBOMInventer { get; set; }
        public decimal? CoutBOMSAP { get; set; }

        //khuôn
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications_Length")]
        public Nullable<decimal> Specifications_Length { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications_Width")]
        public Nullable<decimal> Specifications_Width { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications_Overalls")]
        public Nullable<decimal> Specifications_Overalls { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications_Height")]
        public Nullable<decimal> Specifications_Height { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications_Side")]
        public Nullable<decimal> Specifications_Side { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductPerMold")]
        public Nullable<int> ProductPerMold { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NoteLocation")]
        public string LocationNote { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PrintMoldFilm")]
        public string PrintMoldFilm { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PrintMoldName")]
        public string PrintMoldName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications")]
        public string Specifications { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PrintMoldDate")]
        public Nullable<System.DateTime> PrintMoldDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastMaintenanceDate")]
        public Nullable<System.DateTime> LastMaintenanceDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MaintenanceAlert")]
        public Nullable<int> MaintenanceAlert { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StampQuantity")]
        public Nullable<int> StampQuantity { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CurrentStampeQuantity")]
        public Nullable<int> CurrentStampeQuantity { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
        public string Description { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Serial")]
        public string Serial { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Bin")]
        public string Bin { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Status")]
        public string Status { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StampQuantityAlert")]
        public Nullable<int> StampQuantityAlert { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Product_Size")]
        public string Size { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Size_Width")]
        public Nullable<decimal> Size_Width { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Size_Height")]
        public Nullable<decimal> Size_Height { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SpreadSize_Width")]
        public Nullable<decimal> SpreadSize_Width { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SpreadSize_Height")]
        public Nullable<decimal> SpreadSize_Height { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductType")]
        public bool? Type { get; set; }
    }
}
