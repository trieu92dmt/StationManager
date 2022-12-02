using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ConfirmWorkCenterViewModel
    {
        public System.Guid? TaskId { get; set; }
        public System.Guid? ParentTaskId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXDT")]
        public string ProductionOrder { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_LSXC")]
        public string ProductionOrder_SAP { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Task_Summary")]
        public string Summary { get; set; }
        //Sản phẩm
        public Guid? ProductId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Stock_ProductCode")]
        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Qty")]
        public int? Qty { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Barcode")]
        public string ToBarcode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Unit")]
        public string Unit { get; set; }
        public bool? Actived { get; set; }

        //Từ công đoạn
        public string FromStepId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_StepCode")]
        public string FromStepCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_StepName")]
        public string FromStepName { get; set; }
        public Guid? DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        //Đến công đoạn
        public string ToStepId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_StepCode")]
        public string ToStepCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_StepName")]
        public string ToStepName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_WorkDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? WorkDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreateBy")]
        public string CreateByFullName { get; set; }
        public DateTime? CreateTime { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastEditBy")]
        public string LastEditByFullName { get; set; }
        public DateTime? LastEditTime { get; set; }
        public string StockRecevingType { get; set; }


        //StockReceivingDetail
        public Guid? CustomerReference { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal? Quantity { get; set; }
        public string KTEXT { get; set; }
        public int? Phase { get; set; }
        public string ITMNO { get; set; }
        public DateTime? DateTime { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IsWorkCenterCompleted")]
        public bool? IsWorkCenterCompleted { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_WorkCenter")]
        public string ConfirmWorkCenter { set; get; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkCenterConfirmTime")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? WorkCenterConfirmTime { set; get; }
        public Guid? ConfirmBy { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Assignment_StepCode")]
        public string StepCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TTLSX_StepName")]
        public string StepName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ToStockCode")]
        public string ToStockCode { get; set; }
        public string ToStockName { get; set; }



    }
}
