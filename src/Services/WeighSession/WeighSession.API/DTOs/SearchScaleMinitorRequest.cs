using Core.SeedWork;

namespace WeighSession.API.DTOs
{
    public class SearchScaleMinitorRequest
    {
        public PagingQuery Paging { get; set; } = new PagingQuery();
        //Plant
        public string PlantFrom { get; set; }
        public string PlantTo { get; set; }
        //Đầu cân
        public string WeightHeadCodeFrom { get; set; }
        public string WeightHeadCodeTo { get; set; }
        //Loại
        public string Type { get; set; }
        //Ngày ghi nhận
        public DateTime? RecordTimeFrom { get; set; }
        public DateTime? RecordTimeTo { get; set; }
    }
}
