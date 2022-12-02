using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ISD.ViewModels
{
   public class RoutingViewModel
    {
        public System.Guid StepId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepCode")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [Remote("CheckExistingStepCode", "Routing", AdditionalFields = "StepCodeValid,Code", HttpMethod = "POST", ErrorMessageResourceName = "Validation_Already_Exists", ErrorMessageResourceType = typeof(Resources.LanguageResource))]
        public string StepCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_StepName")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string StepName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Validation_OrderIndex")]
        public Nullable<int> OrderIndex { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public Nullable<bool> Actived { get; set; }

        //Phân xưởng
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_WorkShop")]
        public Nullable<System.Guid> WorkShopId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_WorkShop")]
        public string WorkShopName { get; set; }

        public string CreatedUser { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public string LastModifiedUser { get; set; }
        public Nullable<System.DateTime> LastModifiedTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NumberOfMachines")]
        public Nullable<int> NumberOfMachines { get; set; }       
        
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_WorkCenter")]
        public string WorkCenter { get; set; }
        public string Plant { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EquipmentList")]
        public List<Guid?> EquipmentList { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EquipmentName")]
        public string EquipmentName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EquipmentCode")]
        public string EquipmentCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
        public string Description { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EquipmentProduction")]
        public decimal? EquipmentProduction { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EquipmentProductionUnit")]
        public string EquipmentProductionUnit { get; set; }

    }
}
