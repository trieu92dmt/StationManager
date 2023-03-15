using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.OutboundDelivery
{
    public class SearchOutboundDeliveryCommand
    {
        //Mã nhà máy
        public string PlantCode { get; set; }
        //Sales order
        public string SalesOrderFrom { get; set; }
        public string SalesOrderTo { get; set; }
        //Outbound delivery
        public string OutboundDeliveryFrom { get; set; }
        public string OutboundDeliveryTo { get; set; }
        //Ship to party
        public string ShipToPartyFrom { get; set; }
        public string ShipToPartyTo { get; set; }
        //Material
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }
        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }

        //Search dữ liệu đã lưu
        public string WeightHeadCode { get; set; }
        public List<string> WeightVotes { get; set; } = new List<string>();
        public DateTime? WeightDateFrom { get; set; }
        public DateTime? WeightDateTo { get; set; }
        public Guid? CreateBy { get; set; }
    }
}
