using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.Scale
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
        public string PlantFmt => !string.IsNullOrEmpty(Plant) && !string.IsNullOrEmpty(PlantName) ? $"{Plant} | {PlantName}" : "";
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
        //List màn hình đã chọn
        public List<string> Screens { get; set; } = new List<string>();
    }
}
