using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class PhysicsWorkShopSearchViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PhysicsWorkShop_PhysicsWorkShopName")]
        public string PhysicsWorkShopName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public bool? Actived { get; set; }
    }
}
