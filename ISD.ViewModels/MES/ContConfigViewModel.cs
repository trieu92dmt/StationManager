using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ContConfigViewModel
    {
        public Guid? ContConfigId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ContConfig_MaterialType")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string MaterialType { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ContConfig_Plant")]
        public string Plant { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ContConfig_Formula")]
        public string Formula { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
        public int? OrderIndex { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public Nullable<bool> Actived { get; set; }
    }
}
