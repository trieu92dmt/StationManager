using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class MobileWorkFlowConfigViewModel
    {
        public string FieldCode { get; set; }
        public string FieldName { get; set; }
        public int? OrderIndex { get; set; }
        public bool? IsRequired { get; set; }
        public Nullable<bool> HideWhenAdd { get; set; }
        public string AddDefaultValue { get; set; }
        public Nullable<bool> HideWhenEdit { get; set; }
        public string EditDefaultValue { get; set; }
    }
}
