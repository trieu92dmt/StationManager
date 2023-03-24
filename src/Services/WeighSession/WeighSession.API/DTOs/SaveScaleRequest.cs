using System.ComponentModel.DataAnnotations;

namespace WeighSession.API.DTOs
{
    public class SaveScaleRequest
    {
        //Plant
        [Required]
        public string Plant { get; set; }
        //Mã đầu cân
        [Required]
        public string ScaleCode { get; set; }
        //Tên đầu cân
        [Required]
        public string ScaleName { get; set; }
        //Cân tích hợp
        public bool isIntegrated { get; set; } = false;
        //Cân không tích hợp
        public bool isTruckScale { get; set; } = false;
        //List màn hình
        public List<string> Screens { get; set; } = new List<string>();
    }
}
