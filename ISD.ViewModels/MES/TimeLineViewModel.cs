using System;

namespace ISD.ViewModels
{
    public class TimeLineViewModel
    {
        public Guid TaskId { set; get; }
        public string Summary { set; get; }
        public string SONumber { set; get; }
        public string SOLineNumber { set; get; }
        public int Duration { set; get; }
        public DateTime? StartDate { set; get; }
        public DateTime? EstimateEndDate { set; get; }
        public DateTime? ReceiveDate { set; get; }
        public string status { set; get; }
        public string ProductCode { set; get; }
        public DateTime? DeliveryDate { get; set; }
        public int? SLKH { set; get; }
        public decimal? SLDC { set; get; }

        public decimal? SL { 
            get
            {
                decimal? res;
                if (SLDC.HasValue && SLDC.Value > 0)
                {
                    res = SLDC.Value;
                }
                else
                {
                    res = (decimal)SLKH;
                }
                return res;
            } 
        }

        public int? SLTT { set; get; }
        public decimal? Complate
        {
            get
            {
                int res = 0;
                if (SLKH != null && SLKH != 0)
                {
                    res = SLTT.Value * 100 / SLKH.Value;
                }
                return res;
            }
        }
        public int SLNVLYC { set; get; }
        public Guid? ParentTaskId { set; get; }
        public string ProcessCode { set; get; }
        public Guid? WorkFlowId { set; get; }
        public string WorkFlowCode { set; get; }
        public string Button
        {
            get
            {
                if (WorkFlowCode == "LSXDT")
                {
                    return "<a href='#'><i class='fa fa-plus' data-action='add' data-summary='" + Summary + "'></i></a>";

                }
                else if (WorkFlowCode == "LSXC")
                {
                    return "<a href='#'><i class='fa fa-pencil' data-action='edit'></i></a>  <a class='dropdown-toggle' title='Xem NVL' data-action='show_detail' data-toggle='dropdown'><i class='fa fa-eye'></i></a>";
                }
                else if (WorkFlowCode == "LSXD")
                {
                    return " <a href='#'><i class='fa fa-pencil' data-action='edit-lsxd'></i></a>";
                }
                else if (WorkFlowCode == "")
                {
                    return "";
                }
                else if (WorkFlowCode == "")
                {
                    return "";
                }
                return "";
            }
        }
        public bool? isHidden { get; set; }
        public string txtStartDate
        {
            get
            {
                if (isHidden == true)
                {
                    return "";
                }
                var rs = StartDate.HasValue ? StartDate.Value.ToString("dd/MM") : string.Empty;
                return rs;
            }
        }
        public string txtEstimateEndDate
        {
            get
            {
                if (isHidden == true)
                {
                    return "";
                }
                var rs = EstimateEndDate.HasValue ? EstimateEndDate.Value.ToString("dd/MM") : string.Empty;
                return rs;
            }
        }

        public string txtReceiveDate
        {
            get
            {
                if (isHidden == true)
                {
                    return "";
                }
                return ReceiveDate.HasValue ? ReceiveDate.Value.ToString("dd/MM") : EstimateEndDate.Value.ToString("dd/MM");
            }
        }

        public DateTime? BDDC { get; set; }

        public string txtBDDC
        {
            get
            {
                if (isHidden == true)
                {
                    return "";
                }
                return BDDC.HasValue ? BDDC.Value.ToString("dd/MM") : StartDate.Value.ToString("dd/MM");
            }
        }

        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
    }
}
