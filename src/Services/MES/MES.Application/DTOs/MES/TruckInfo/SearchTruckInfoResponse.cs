using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.TruckInfo
{
    public class SearchTruckInfoResponse
    {
        //ID cân xe tải
        public string TruckInfoId { get; set; }
        //Plant
        public string PlantCode { get; set; }
        //Số xe tải
        public string TruckNumber { get; set; }
        //Tài xế xe tải
        public string Driver { get; set; }
        //Cân xe đầu vào
        public decimal? InputWeight { get; set; }
        //Thời gian ghi nhận
        public DateTime? RecordTime { get; set; }
        //Create by
        public Guid? CreateById { get; set; }
        public string CreateBy { get; set; }
    }
}
