using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NKPPPP
{
    public class GetDataByWoAndItemComponentResponse
    {
        //Material
        public string Material { get; set; }
        //Material desc
        public string MaterialName { get; set; }
        //Component
        public string Component { get; set; }
        //Component desc
        public string ComponentDesc { get; set; }
        //Batch
        public string Batch { get; set; }
        //Số lượng yêu cầu
        public decimal? RequiremenQty { get; set; }
        //Số lượng nhập đã thu hồi
        public decimal? WithdrawnQty { get; set; }
        //Open quantity
        public decimal? OpenQuantity => RequiremenQty - WithdrawnQty;
        //Scheduled Start Date
        public DateTime? ScheduledStartDate { get; set; }
        //Scheduled Finish Date
        public DateTime? ScheduledFinishDate { get; set; }
    }
}
