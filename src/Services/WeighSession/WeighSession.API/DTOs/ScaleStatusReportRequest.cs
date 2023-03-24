namespace WeighSession.API.DTOs
{
    public class ScaleStatusReportRequest
    {
        //Plant
        public string Plant { get; set; }
        //Đầu cân
        public List<string> ScaleCode { get; set; }
    }
}
