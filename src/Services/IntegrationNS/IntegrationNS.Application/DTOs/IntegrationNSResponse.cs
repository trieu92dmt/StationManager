namespace IntegrationNS.Application.DTOs
{
    public class IntegrationNSResponse
    {
        //Tổng record
        public int? TotalRecord { get; set; } = 0; 
        //Records đồng bộ thành công
        public int? RecordSyncSuccess { get; set; } = 0;
        //Records đồng bộ thất bại 
        public int? RecordSyncFailed { get; set; } = 0;
        public List<DetailIntegrationFailResponse> ListRecordSyncFailed { get; set; } = new List<DetailIntegrationFailResponse>();
    }

    public class DetailIntegrationFailResponse
    {
        public string RecordFail { get; set; }
        public string Msg { get; set; }

    }
}
