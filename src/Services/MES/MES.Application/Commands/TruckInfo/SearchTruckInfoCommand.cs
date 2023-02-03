using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.TruckInfo
{
    public class SearchTruckInfoCommand
    {
        //Plant
        public string Plant { get; set; }
        //Số xe tải
        public string TruckNumberFrom { get; set; }
        public string TruckNumberTo { get; set; }
        //Ngày ghi nhận
        public DateTime? RecordTimeFrom { get; set; }
        public DateTime? RecordTimeTo { get; set; }
        //Create by
        public List<Guid> CreateBy { get; set; } = new List<Guid>();
    }
}
