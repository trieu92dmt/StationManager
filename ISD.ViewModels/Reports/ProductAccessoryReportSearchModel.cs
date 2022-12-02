using System;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class ProductAccessoryReportSearchModel
    {
        //15. Ngày kết thúc
        public string EndCommonDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? EndFromDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? EndToDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductSAPCode")]
        public string SearchERPProductCode { get; set; }

        [Display(Name = "Phân loại sản phẩm")]
        public string ProductCategoryCode { get; set; }

        //19. Hình thức bảo hành
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_ErrorTypeCode2")]
        public string ErrorTypeCode { get; set; }

        //20. Phương thức xử lý
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_ErrorCode2")]
        public string ErrorCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductAccessorySAPCode")]
        public string SearchERPAccessoryCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Accessory_AccessoryType")]
        public string SearchAccessoryTypeCode { get; set; }
        public bool IsView { get; set; }
    }
}