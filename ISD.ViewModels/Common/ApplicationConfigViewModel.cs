using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ApplicationConfigViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ResourceKey")]
        public string ConfigKey { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ResourceValue")]
        public string ConfigValue { get; set; }

    }
}
