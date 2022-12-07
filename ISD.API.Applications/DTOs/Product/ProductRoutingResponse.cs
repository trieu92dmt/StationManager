using ISD.API.Applications.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.Product
{
    public class ProductRoutingResponse
    {
        public int STT { get; set; }
        //Phiên bản Routing
        public string RoutingVersion { get; set; }
        //Mã thành phẩm /bán thành phẩm
        public string ProductCode { get; set; }
        //Tên thành phẩm /bán thành phẩm
        public string ProductName { get; set; }
        //Thứ tự công đoạn
        public int? OrderIndex { get; set; }
        //Mã công đoạn
        public string StepCode { get; set; }
        //Tên công đoạn
        public string StepName { get; set; }
        //Hướng dẫn sản xuất
        public string ProductionGuide { get; set; }
        //% Ước tính hoàn thành
        public decimal? EstimateComplete { get; set; }
        //Thời gian định mức (giây)
        public decimal? RatedTime { get; set; }
        //Khuôn
        public List<string> Molds { get; set; } = new List<string>();
        //Số SP/ tờ
        public int? ProductPerPage { get; set; }
    }

    public class ProductRoutingListResponse
    {
        public List<ProductRoutingResponse> ProductRoutings { get; set; } = new List<ProductRoutingResponse>();

        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }
}
