using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NK
{
    public class SearchNKCommand
    {
        //Plant
        public string Plant { get; set; }
        //Customer
        public string Customer { get; set; }
        //Material
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }

        //Dữ liệu đã lưu
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Số phiếu cân 
        public List<string> WeightVotes { get; set; } = new List<string>();
        //Ngày thực hiện cân from
        public DateTime? WeightDateFrom { get; set; }
        //Ngày thực hiện cân to
        public DateTime? WeightDateTo { get; set; }
        //Create by
        public Guid? CreateBy { get; set; }
    }
}
