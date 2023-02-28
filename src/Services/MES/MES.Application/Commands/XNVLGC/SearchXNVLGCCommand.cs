using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.XNVLGC
{
    public class SearchXNVLGCCommand
    {
        //Plant
        public string Plant { get; set; }
        //Vendor
        public string VendorFrom { get; set; }
        public string VendorTo { get; set; }
        //Purchase Order
        public string PurchaseOrderFrom { get; set; }
        public string PurchaseOrderTo { get; set; }
        //Material
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }
        //Component
        public string ComponentFrom { get; set; }
        public string ComponentTo { get; set; }
        //Document Date
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
