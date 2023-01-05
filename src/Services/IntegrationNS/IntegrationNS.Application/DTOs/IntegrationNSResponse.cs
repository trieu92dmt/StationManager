namespace IntegrationNS.Application.DTOs
{
    public class IntegrationNSResponse
    {
        //Tổng record
        public int? TotalRecord { get; set; } = 0; 
        //Records đồng bộ thành công
        public int? RecordSyncSuccess { get; set; } = 0;
        //Records đồng không thất bại 
        public int? RecordSyncFailed { get; set; } = 0;
        public List<string> ListRecordSyncFailed { get; set; } = new List<string>();
    }
}
