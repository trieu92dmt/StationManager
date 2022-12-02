using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class DateClosedViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DateClosed_ModifiedUser")]
        public string ModifiedUserName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DateClosed_DateClosedUpdate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateClosed { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DateClosed_ModifiedTime")]
        [DisplayFormat(DataFormatString = "{0:HH:mm dd/MM/yyyy}")]
        public DateTime? ModifiedTime { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DateClosed_DateClosedInput")]
        public DateTime? DateClosedInput { get; set; }
    }
}
