using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class DivisionOfTaskByLSXDTViewModel
    {
        public Guid? DotId { get; set; }
        public string DotName { get; set; }
        public int? STTDot { get; set; }
        public DateTime? Date1 { get; set; }
        public DateTime? Date2 { get; set; }
        public DateTime? Date3 { get; set; }
        public DateTime? Date4 { get; set; }
        public DateTime? Date5 { get; set; }
        public DateTime? Date6 { get; set; }
        public DateTime? Date7 { get; set; }
        public DateTime? Date8 { get; set; }
        public DateTime? Date9 { get; set; }
        public string Property3 { get; set; }
        public string Property4 { get; set; }
        public string Property5 { get; set; }
        public string Property6 { get; set; }
        public string Property7 { get; set; }
        public string Property8 { get; set; }

        public List<DivisionOfTaskByLSXDTSubtaskViewModel> LSXSAPList { get; set; }
        public List<string> StepList { get; set; }
    }

    public class DivisionOfTaskByLSXDTSubtaskViewModel
    {
        public int? STTDot { get; set; }
        public Guid? LSXSAPId { get; set; }
        public string LSXSAP { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? Date1 { get; set; }

        public DateTime? EstimateEndDate { get; set; }

        public DateTime? ReceiveDate { get; set; }
        public int? Duration
        {
            get
            {
                if (StartDate.HasValue && EstimateEndDate.HasValue)
                {
                    return (int)(EstimateEndDate - StartDate).Value.TotalDays + 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int? Qty { get; set; }
        public int? Number2 { get; set; }
        public Guid? TaskStatusId { get; set; }
        public string TaskStatusName { get; set; }
        public int? SL_NVL_YC { get; set; }
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public string Property3 { get; set; }
        public string Property4 { get; set; }
        public string Property5 { get; set; }
        public Guid? ProductId { get; set; }
        public string Unit { get; set; }
        public string StepCode { get; set; }
        public bool? IsChoosen { get; set; }
    }
}
