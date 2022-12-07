namespace ISD.API.Applications.DTOs.Report.EquipmentReport
{
    public class DetailReportTimeMachineHasTroubledResponse
    {
        public string WorkShopName { get; set; }
        public string EquipmentName { get; set; }
        public List<TimeMachineHasTroubledResponse> TimeMachineHasTroubleds { get; set; } = new List<TimeMachineHasTroubledResponse>();
    }
    public class TimeMachineHasTroubledResponse
    {
        public int STT { get; set; }
        public string Date { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public decimal? Hours { get; set; }
        public string ReasonStop { get; set; }
        public string Description { get; set; }
    }
}
