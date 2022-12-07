namespace ISD.API.Applications.DTOs.MatchCardCommand
{
    public class DetailsMCCommandResponse
    {
        //Mã ghép bài
        public string MatchCardCode { get; set; }
        //Tên hạng mục in
        public string PrintItem { get; set; }
        //Danh sách LSX ghép bài
        public string WorkOrderCodes { get; set; }
        //Công ty gia công in
        public string PrintCompany { get; set; }
        //Số phiếu thiết kế
        public string DesignVote { get; set; }
        //Ngày yêu cầu có hàng in
        public string PrintReqDate { get; set; }
        //WorkOrderRouting Details
        public List<WorkOrderMCResponse> WorkOrderMCs { get; set; } = new List<WorkOrderMCResponse> ();
        //Chia tỉ lệ ghép bài
        public List<RoutingRateResponse> RoutingRates { get; set; } = new List<RoutingRateResponse> ();
    }

    public class WorkOrderMCResponse
    {
        //LSX
        public string WorkOrderCode { get; set; }
        //TP/BTP
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public List<WorkOrderRoutingResponse> WorkOrderRoutings { get; set; } = new List<WorkOrderRoutingResponse> ();
    }

    public class WorkOrderRoutingResponse
    {
        //Thứ tự công đoạn
        public int? OrderIndex { get; set; }
        //Mã công đoạn
        public string StepCode { get; set; }
        //Tên công đoạn
        public string StepName { get; set; }
        //Hướng dẫn sản xuất
        public string ProductionGuide { get; set; }
        //Số con trên 1 tờ 
        public int? ProductPerPage { get; set; }
        //Số lượng thực hiện theo từng công đoạn
        public decimal? ProductPerStep { get; set; }
        //ĐVT công đoạn
        public string Unit { get; set; }
        //Tổng số sản phẩm
        public decimal? TotalQuantity => ProductPerPage.HasValue && ProductPerStep.HasValue ? ProductPerStep*ProductPerPage : 0;
        //% Ước tính hoàn thành
        public decimal? EstimateComplete { get; set; }
        //Thời gian chuẩn bị máy (min)
        public decimal? SetupTime { get; set; }
        //Thời gian định mức (min)
        public decimal? RatedTime { get; set; }
        //Ghép bài
        public bool? MatchCard { get; set; }
        //Tỷ lệ
        public decimal? Rated { get; set; }
    }

    public class RoutingRateResponse
    {
        //Công đoạn
        public string StepCode { get; set; }
        //Tỉ lệ từng lsx
        public List<WorkOrderRateResponse> WorkOrderRates { get; set; } = new List<WorkOrderRateResponse> ();
    }

    public class WorkOrderRateResponse
    {
        //Id
        public Guid WorkOrderId { get; set; }
        //Mã lsx
        public string WorkOrderCode { get; set; }
        //Số SP/Tờ/Công đoạn
        public int? ProductPerPage { get; set; }
        //Ghép bài
        public bool? MatchCard { get; set; }
        //Tỷ lệ
        public decimal? Rated { get; set; }
        //Disable
        public bool IsDisable { get; set; }
    }
}