namespace ISD.API.Applications.DTOs.Report.OrderProgressReport
{
    /// <summary>
    /// Báo cáo tiến độ đơn hàng
    /// </summary>
    public class OrderProgressReportResponse
    {
        public int STT { get; set; }
        public Guid? Id { get; set; }
        public string WOGeneral { get; set; }                               //LSX tổng hợp
        public string WODesign { get; set; }                                //Lệnh thiết kế
        public string MatchCard { get; set; }                               //Lệnh ghép bài
        public string WOCode { get; set; }                                  //LSX
        public string ProductCode { get; set; }                             //Mã TP/BTP
        public string ProductName { get; set; }                             //Tên TP/BTP
        public decimal? QuantityOrder { get; set; }                         //Số lượng phát lệnh
        public decimal? CompleteTimeStage { get; set; }                     //Số giờ cần để HT công đoạn còn lại
        public string Unit { get; set; }                                    //Đơn vị tính
        public DateTime? CreateWODate { get; set; }                         //Ngày ra LSX
        public string EstimateCompleteWODate { get; set; }                  //Ngày hoàn thành SX dự kiến theo ghi nhận
        public DateTime? EstimateDeliveryDate { get; set; }                 //Ngày giao hàng dự kiến
        public string EstimateDeliveryTime => EstimateDeliveryDate.HasValue ? EstimateDeliveryDate.Value.ToString("HH:mm") : null; //Giờ giao hàng dự kiến
        public List<RoutingWO> ListRouting { get; set; } = new List<RoutingWO>();
        public List<StepFinish> StepFinishes { get; set; } = new List<StepFinish>();
    }

    public class StepFinish
    {
        public string StepCode { get; set; }
        public string StepName { get; set; }
        public string Color { get; set; }
        public decimal? PercentComplete { get; set; }
    }

    public class RoutingWO
    {
        public string StepCode { get; set; }
        public int? OrderIndex { get; set; }
        public decimal? RatedTime { get; set; }
        public bool? IsFinish { get; set; }
    }

    public class RoutingWOQuantity : RoutingWO
    {
        public decimal? Quantity { get; set; }
    }
}
