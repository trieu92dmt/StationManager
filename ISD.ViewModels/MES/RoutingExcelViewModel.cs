using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MES
{
    public class RoutingExcelViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StepId")]
        public System.Guid StepId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepCode")]
        [Required]
        public string StepCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepName")]
        [Required]
        public string StepName { get; set; }

        //Phân xưởng
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "Sale_WorkShop")]
        public Nullable<System.Guid> WorkShopId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Master_WorkShop")]
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

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Delete")]
        public bool? isDelete { get; set; }

        //Import Excel
        public int RowIndex { get; set; }

        public string Error { get; set; }
        public bool isNullValueId { get; set; }
    }
}
