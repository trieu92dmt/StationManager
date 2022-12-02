using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ContRegisterSearchViewModel
    {
        public string Plant { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Year")]
        public int? Year { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Month")]
        public int? Month { get; set; }
    }
}
