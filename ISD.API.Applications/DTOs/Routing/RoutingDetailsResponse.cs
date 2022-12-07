namespace ISD.API.Applications.DTOs.Routing
{
    public class RoutingDetailsResponse
    {
        //Mã WORouting
        public Guid WORoutingId { get; set; }
        //Mã công đoạn
        public string StepCode { get; set; }
        //Tên công đoạn
        public string StepName { get; set; }
        //Hướng dẫn sản xuất
        public string ProductionGuide { get; set; }
        public string ComponentUsed { get; set; }
        public string MoldCode { get; set; }
        public decimal? RatedTime { get; set; }
        public decimal? SetupTime { get; set; }
        public decimal? EstimateComplete { get; set; }
        //SP/Tờ/Công đoạn
        public int? ProductPerPage { get; set; }
        //Số lượng thực hiện theo từng công đoạn
        public decimal? ProductPerStep { get; set; }
        //Tổng số sản phẩm
        public decimal? TotalQuantity { get; set; }  
    }
}
