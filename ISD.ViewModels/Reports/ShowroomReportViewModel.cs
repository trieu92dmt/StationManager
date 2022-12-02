using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels
{
    public class ShowroomReportViewModel
    {
        [Display(Name = "Loại")]
        public string WorkFlowName { get; set; }

        [Display(Name = "Trạng thái")]
        public string TaskStatusName { get; set; }

        [Display(Name = "Khu vực")]
        public string Area { get; set; }

        [Display(Name = "Số lượng")]
        public decimal? NumberOfShowroom { get; set; }

        [Display(Name = "Giá trị")]
        public decimal? ValueOfShowroom { get; set; }
    }

    public class ShowroomReportSearchViewModel
    {
        [Display(Name = "Công ty")]
        public Guid? CompanyId { get; set; }

        [Display(Name = "Loại")]
        public Guid? WorkFlowId { get; set; }
        public List<Guid> WorkFlowList { get; set; }

        [Display(Name = "Trạng thái")]
        public string TaskStatusCode { get; set; }
        public List<string> TaskStatusCodeList { get; set; }

        [Display(Name = "Khu vực")]
        public string Area { get; set; }

        [Display(Name = "Ngày thực hiện")]
        public string CommonDate { get; set; }

        [Display(Name = "Từ ngày")]
        public DateTime? StartFromDate { get; set; }

        [Display(Name = "Đến ngày")]
        public DateTime? StartToDate { get; set; }

        [Display(Name = "Nhóm vật tư")]
        public Guid? CategoryId { get; set; }
        public List<Guid> CategoryIdList { get; set; }
        public bool IsView { get; set; }
    }
}
