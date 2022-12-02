using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ConsumableMaterialsDeliveryViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ConsumableMaterialsDelivery_DocumentDate")]
        public DateTime? DocumentDate { get; set; }

        public List<DataAllotmentViewModel> dataAllotmentList { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ConsumableMaterialsDelivery_StepCode")]
        public string StepCode { get; set; }
    }

    public class DataAllotmentViewModel
    {
        public string DataAllotment { get; set; }
        public decimal? DataAllotmentQuantity { get; set; }
    }

    public class ConsumableMaterialsDeliveryFormViewModel
    {
        public DateTime? DocumentDate { get; set; }
        public List<AllotmentResultViewModel> detail { get; set; }
    }
}
