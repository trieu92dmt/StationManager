using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class AssignmentLenhSanXuatViewModel
    {
        public int? STT { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXDT")]
        public string LSXDT { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXD")]
        public string LSXD { get; set; }
        public Guid? ProductionOrderBatch { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXC")]
        public string LSXC { get; set; }
        public Guid? ProductionOrder { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_ProductCode")]
        public string ProductCode { get; set; }
        public Guid? ProductId { get; set; }
        public string CompanyCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_ProductName")]
        public string ProductName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_PlanQty")]
        //[DisplayFormat(DataFormatString = "{0:n0}")]
        public int? PlanQty { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_ActualQty")]
        //[DisplayFormat(DataFormatString = "{0:n0}")]
        public int? ActualQty { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_PlanBOMQty")]
        public int? PlanBOMQty { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_ActualBOMQty")]
        public int? ActualBOMQty { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_AssignQty")]
        public int? Qty { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_ItemCode")]
        public string ItemCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_ItemName")]
        public string ItemName { get; set; }
        public bool? IsCompleteBigStep { get; set; }
    }
}
