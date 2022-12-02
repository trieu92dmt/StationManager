using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ISD.ViewModels
{
   public class WorkShopViewModel
    {
        //public int? STT { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_WorkShop")]
        public System.Guid WorkShopId { get; set; }
        public System.Guid? CompanyId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_Store")]
        public System.Guid? StoreId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_Store")]
        public string StoreName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkShop_Compamy")]
        public string CompanyName { get; set; }

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
