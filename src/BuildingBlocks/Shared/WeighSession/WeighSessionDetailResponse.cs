using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.WeighSession
{
    public class WeighSessionDetailResponse
    {
        //MÃ đợt cân
        public string WeighSessionCode { get; set; }
        //Mã đầu cân
        public string ScaleCode { get; set; }
        //DateKey
        public string DateKey { get; set; }
        //OrderIndex
        public int? OrderIndex { get; set; }
        //Start time
        public DateTime? StartTime { get; set; }
        //End Time
        public DateTime? EndTime { get; set; }
        //TotalNumberOfWeigh
        public int? TotalNumberOfWeigh { get; set; }
    }
}
