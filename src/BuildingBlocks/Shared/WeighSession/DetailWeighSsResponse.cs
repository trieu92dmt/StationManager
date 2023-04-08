using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.WeighSession
{
    public class DetailWeighSsResponse
    {
        //Id chi tiết đợt cân
        public string WeighSessionDetailId { get; set; }
        //Id đợt cân
        public string WeighSessionCode { get; set; }
        //Số lần cân
        public int? NumberOfWeigh { get; set; }
        //Trọng lượng cân chi tiết
        public decimal? DetailWeigh { get; set; }
    }
}
