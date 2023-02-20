using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.ScaleMonitor
{
    public class SearchScaleMonitorResponse
    {
        //Mã đầu cân
        public string WeightHeadCode { get; set; }
        //ID đợt cân
        public Guid WeightSessionId { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Đơn vị
        public string Unit { get; set; }
        //Nhà máy
        public string Plant { get; set; }
        //Thời gian bắt đầu
        public DateTime? StartTime { get; set; }
        //Thời gian kết thúc
        public DateTime? EndTime { get; set; }
        //Thời gian ghi nhận
        public DateTime? RecordTime { get; set; }
        //Loại
        public string Type { get; set; }
    }
}
