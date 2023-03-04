using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NKMH
{
    public class GetWeighNumResponse
    {
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Số lần cân
        public int? WeightQuantity { get; set; }
        //Thời gian bắt đâu
        public DateTime? StartTime { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Is success
        public bool isSuccess { get; set; }
    }
}
