using ISD.API.Applications.DTOs.Common;

namespace ISD.API.Applications.DTOs.TestTarget
{
    public class TestTargetResponse
    {
        public int STT { get; set; }
        public Guid TestTargetId { get; set; }
        public int TargetCode { get; set; }
        public string TargetName { get; set; }
        public string StepCode { get; set; }
        public string StepName { get; set; }
        public string Tolerance { get; set; }
        public bool? IsQualityIndiCation { get; set; }
        public bool? Actived { get; set; }
    }
    public class TestTargetListResponse
    {
        public List<TestTargetResponse> TestTargets { get; set; } = new List<TestTargetResponse>();

        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }
}
