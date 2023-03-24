using Core.Models;

namespace WeighSession.API.DTOs
{
    public class SearchScaleRequest
    {
        public DatatableViewModel Paging { get; set; }
        //Nhà máy
        public string Plant { get; set; }
        //Đầu cân
        public string ScaleCode { get; set; }
        //Trạng thái
        public bool? Status { get; set; }
    }
}
