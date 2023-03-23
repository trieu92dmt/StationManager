namespace Shared.WeighSession
{
    public class SearchScaleMonitorResponse2
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<SearchScaleMonitorResponse> Data { get; set; }
    }
    public class SearchScaleMonitorResponse
    {
        public int STT { get; set; }
        //Mã đầu cân
        public string WeightHeadCode { get; set; }
        //ID đợt cân
        public string WeightSessionId { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Đơn vị
        public string Unit { get; set; }
        //Nhà máy
        public string Plant { get; set; }
        //Thời gian bắt đầu
        public DateTime? StartTime { get; set; }
        //Thời gian kết thúc
        public DateTime? EndTime { get; set; }
        //Thời gian ghi nhận
        public DateTime? RecordTime { get; set; }
        //Loại
        public string Type { get; set; }
    }
}
