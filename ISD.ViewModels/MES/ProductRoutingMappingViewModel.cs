using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
   public class ProductRoutingMappingViewModel
    {
        //Phiên bản Routing
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RoutingVersion")]
        [Required]
        public string RoutingVersion { get; set; }
        //Mã thành phầm/bán thành phẩm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductCode_Routing")]
        [Required]
        public string ProductCode { get; set; }
        //Tên thành phẩm/bán tành phẩm
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductName_Routing")]
        [Required]
        public string ProductName { get; set; }
        //Thứ tự công đoạn
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex_Routing")]
        public int? OrderIndex { get; set; }
        //Mã công đoạn
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepCode")]
        [Required]
        public string StepCode { get; set; }
        //Tên công đoạn
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepName")]
        [Required]
        public string StepName { get; set; }
        //Hướng dẫn sản xuất
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductionGuide")]
        public string ProductionGuide { get; set; }
        //%Ước tính hoàn thành
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EstimateComplete")]
        [Required]
        public decimal? EstimateComplete { get; set; }
        //Thời gian định mức 
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "RatedTime")]
        public decimal? RatedTime { get; set; }
        //Khuôn
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MoldCode")]
        public string MoldCode { get; set; }
        //Số SP/tờ
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductPerMold")]
        public int? ProductPerPage { get; set; }

        //DropdownList ProductCode
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductCode_Routing")]
        public List<string> ProductList { get; set; }

    }
}
