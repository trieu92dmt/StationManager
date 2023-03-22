using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.WeighSession
{
    public class ScaleDetailResponse
    {
        //Mã đầu cân
        public string ScaleCode { get; set; }
        //Tên đầu cân
        public string ScaleName { get; set; }
        //Nhà máy
        public string Plant { get; set; }
        //Note
        public string Note { get; set; }
    }
}
