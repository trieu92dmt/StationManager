namespace Shared.WeighSession
{
    public class ScaleStatusReportResponse
    {
        //STT
        public int STT { get; set; }
        //Nhà máy
        public string Plant { get; set; }
        //Đầu cân
        public string ScaleCode { get; set; }
        public string ScaleName { get; set; }
        public string ScaleFmt => !string.IsNullOrEmpty(ScaleCode) && !string.IsNullOrEmpty(ScaleName) ? $"{ScaleCode} | {ScaleName}" : "";
        //Cân tích hợp
        public bool isIntegrate { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Đợt cân
        public string WeighSession { get; set; }
        //Thời gian bắt đầu
        public DateTime? StartTime { get; set; }
    }
}
