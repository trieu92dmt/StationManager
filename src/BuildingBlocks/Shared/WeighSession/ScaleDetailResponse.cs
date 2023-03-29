namespace Shared.WeighSession
{
    public class ScaleDetailResponse
    {
        //Id đầu cân
        public Guid ScaleId { get; set; }
        //Nhà máy
        public string Plant { get; set; }
        //Tên nhà máy
        public string PlantName { get; set; }
        //Nhà máy | Tên nhà máy
        public string PlantFmt { get; set; }
        //Mã đầu cân
        public string ScaleCode { get; set; }
        //Tên đầu cân
        public string ScaleName { get; set; }
        //Cân tích hợp
        public bool isIntegrated { get; set; }
        //Cân xe tải
        public bool isTruckScale { get; set; }
        //Trạng thái
        public bool Status { get; set; }
        //Note
        public string Note { get; set; }
        //List màn hình đã chọn
        public List<string> Screens { get; set; } = new List<string>();
    }
}
