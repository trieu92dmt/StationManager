using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.ReceiptFromProduction
{
    public class SearchNKTPSXCommand
    {
        //Plant
        public string Plant { get; set; }
        //Order Type
        public string OrderType { get; set; }
        //Production Order From
        public string WorkOrderFrom { get; set; }
        //Production Order To
        public string WorkOrderTo { get; set; }
        //Sale order from
        public string SaleOrderFrom { get; set; }
        //Sale order to
        public string SaleOrderTo { get; set; }
        //Material from
        public string MaterialFrom { get; set; }
        //Material to
        public string MaterialTo { get; set; }
        //Scheduled Start from
        public DateTime? ScheduledStartFrom { get; set; }
        //Scheduled Start to
        public DateTime? ScheduledStartTo { get; set; }
        
        //Dữ liệu đã lưu
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Số phiếu cân from
        public string WeightVoteFrom { get; set; }
        //Sô phiếu cân to
        public string WeightVoteTo { get; set; }
        //Ngày thực hiện cân from
        public DateTime? WeightDateFrom { get; set; }
        //Ngày thực hiện cân to
        public DateTime? WeightDateTo { get; set; }
        //Create by
        public Guid? CreateBy { get; set; }
    }
}
