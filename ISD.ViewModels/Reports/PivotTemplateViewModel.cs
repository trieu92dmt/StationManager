using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class PivotTemplateViewModel
    {
        public Guid SearchResultTemplateId { get; set; }
        public string TemplateName { get; set; }
        public bool? IsDefault { get; set; }
    }
}
