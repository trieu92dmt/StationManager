namespace ISD.API.Applications.DTOs.WorkOrder
{
    public class WOMoldResponse
    {
        public Guid? WorkOderId { get; set; }
        public string MoldCode { get; set; }
        public string StepCodeNoDisplay { get; set; }
        public string StepCode { get; set; }
        public string SerialNumber { get; set; }
    }
}
