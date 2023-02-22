using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.XKLXH
{
    public class SearchXKLXHCommand
    {
        //Plant
        public string Plant { get; set; }
        //Delivery Type
        public string DeliveryType { get; set; }
        //Purchase order
        public string PurchaseOrderFrom { get; set; }
        public string PurchaseOrderTo { get; set; }
        //Sales Order
        public string SalesOrderFrom { get; set; }
        public string SalesOrderTo { get; set; }
        //Ship to party
        public string ShipToPartyFrom { get; set; }
        public string ShipToPartyTo { get; set; }
        //Outbound Delivery
        public string OutboundDeliveryFrom { get; set; }
        public string OutboundDeliveryTo { get; set; }
        //Material
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }
        //Document date
        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }

        //Dữ liệu đã lưu
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Số phiếu cân
        public List<string> WeightVotes { get; set; } = new List<string>();
        //Ngày thực hiện cân
        public DateTime? WeightDateFrom { get; set; }
        public DateTime? WeightDateTo { get; set; }
        //CreateBy
        public Guid? CreateBy { get; set; }
    }
}
