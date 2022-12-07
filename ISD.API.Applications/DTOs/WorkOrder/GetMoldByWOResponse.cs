namespace ISD.API.Applications.DTOs.WorkOrder
{
    public class GetMoldByWOResponse
    {
        public List<MoldSelectedResponse> MoldSelectedResponses { get; set; } = new List<MoldSelectedResponse>();
        public List<MoldNotSelectedResponse> MoldNotSelectedResponses { get; set; } = new List<MoldNotSelectedResponse>();
    }

    public class MoldSelectedResponse : MoldNotSelectedResponse
    {
        public Guid Id { get; set; }     
    }

    public class MoldNotSelectedResponse
    {
        public int STT { get; set; }
        public string StepCode { get; set; }
        public string StepName { get; set; }
        public string MoldCode { get; set; }
        public string MoldName { get; set; }
        public string Serial { get; set; }
    }
}
