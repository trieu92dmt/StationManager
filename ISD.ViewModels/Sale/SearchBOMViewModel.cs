using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class SearchBOMViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BOM_Version")]
        public string Version { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "BOMCreate")]
        public string CommonDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FromDate")]
        public DateTime? FromDate  { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToDate")]
        public DateTime? ToDate  { get; set; }
    }
}
