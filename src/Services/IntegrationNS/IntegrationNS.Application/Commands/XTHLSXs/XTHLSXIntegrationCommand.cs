namespace IntegrationNS.Application.Commands.XTHLSXs
{
    public class XTHLSXIntegrationCommand
    {
        //Plant
        public string Plant { get; set; }
        //Material
        public string Material { get; set; }
        //Component
        public string ComponentFrom { get; set; }
        public string ComponentTo { get; set; }
        //Production Order
        public string WorkorderFrom { get; set; }
        public string WorkorderTo { get; set; }
        //Sales Order
        public string SalesOrderFrom { get; set; }
        public string SalesOrderTo { get; set; }
        //Order Type
        public string OrderType { get; set; }
        //Scheduled Start from
        public DateTime? ScheduledStartFrom { get; set; }
        //Scheduled Start to
        public DateTime? ScheduledStartTo { get; set; }


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

        public string Status { get; set; }

    }
}
