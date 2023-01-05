namespace MasterData.Application.DTOs
{
    public class CompanySearchResponse
    {
        public int STT { get; set; }
        //Id
        public Guid CompanyId { get; set; }
        //Mã công ty
        public string CompanyCode { get; set; }
        //Tên công ty
        public string CompanyName { get; set; }
        //Logo
        public string Logo { get; set; }
        //Trạng thái
        public bool Actived { get; set; }
    }
}
