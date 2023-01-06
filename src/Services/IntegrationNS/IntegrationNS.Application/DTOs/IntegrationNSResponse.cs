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
        public List<string> ListRecordSyncFailed { get; set; } = new List<string>();
    }

    public class DeleteNSResponse
    {
        //Tổng record
        public int? TotalRecord { get; set; } = 0;
        //Records xóa thành công
        public int? RecordDeleteSuccess { get; set; } = 0;
        //Records xóa thất bại 
        public int? RecordDeleteFail { get; set; } = 0;
        public List<string> ListRecordDeleteFailed { get; set; } = new List<string>();
    }
}
