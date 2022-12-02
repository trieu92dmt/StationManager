using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class CapacityRegisterSearchViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MasterData_WorkShop")]
        public string WorkShopCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Year")]
        public int? Year { get; set; }
    }
}
