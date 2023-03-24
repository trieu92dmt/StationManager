using System.ComponentModel.DataAnnotations;

namespace WeighSession.API.DTOs
{
    public class UpdateScaleRequest
    {
        public List<ScaleUpdate> Scales { get; set; } = new List<ScaleUpdate>();
    }

    public class ScaleUpdate
    {
        //Id
        [Required]
        public string ScaleCode { get; set; }
        //ScaleName
        [Required]
        public string ScaleName { get; set; }
        //Cân tích hợp
        public bool isIntegrated { get; set; }
        //Cân xe tải
        public bool isTruckScale { get; set; }
        //Trạng thái
        public bool isActived { get; set; }
        //Màn hình đã chọn
        public List<string> Screens { get; set; } = new List<string>();
    }
}
