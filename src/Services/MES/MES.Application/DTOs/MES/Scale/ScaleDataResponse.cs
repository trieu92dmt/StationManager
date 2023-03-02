using MES.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.Scale
{
    public class ScaleDataResponse
    {
        //STT
        public int STT { get; set; }
        //Id
        public Guid ScaleId { get; set; }
        //Nhà máy
        public string Plant { get; set; }
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
    }

    public class ScaleListResponse
    {
        //Danh sách cân
        public List<ScaleDataResponse> Scales { get; set; } = new List<ScaleDataResponse>();
        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }
}
