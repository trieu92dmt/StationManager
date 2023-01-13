using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NKMH
{
    public class GetWeighNumResponse
    {
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm Qty
        public decimal? ConfirmQty { get; set; }
        //Số lần cân
        public int WeightQuantity { get; set; }
        //Trạng thái
        public string Status { get; set; }
    }
}
