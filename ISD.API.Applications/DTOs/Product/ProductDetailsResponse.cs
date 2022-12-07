using ISD.API.Constant.Common;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.Product
{
    public class ProductDetailsResponse
    {
        //Phân cấp sản phẩm
        public string ParentCategoryName { get; set; }
        //Nhóm sản phẩm
        public string CategoryName { get; set; }
        //Nhóm sản phẩm chi tiết
        public string CategoryDetailName { get; set; }
        //Mã TP/BTP
        public string ProductCode { get; set; }
        //Tên TP/BTP
        public string ProductName { get; set; }
        //Mô tả
        public string Description { get; set; }
        //ĐVT
        public string Unit { get; set; }
        //Mã bản vẽ khuôn
        public string MoldDrawCode { get; set; }
        public string Serial { get; set; }
        //Mã phim kiểm tra
        public string PrintMoldFilm { get; set; }
        //Tên khắc trên khuôn
        public string PrintMoldName { get; set; }
        //Bin
        public string Bin { get; set; }
        //Kho khuôn
        public string LocationNote { get; set; }
        //Tình trạng
        public string Status { get; set; }
        //Ngày bảo trì gần nhất
        public DateTime? LastMaintenanceDate { get; set; }
        public string LastMaintenanceDateStr => LastMaintenanceDate.HasValue ? LastMaintenanceDate.Value.ToString(ISDDateTimeFormat.DdMmyyyy) : null;
        //Cảnh báo bảo trì (số ngày)
        public int? MaintenanceAlert { get; set; }
        //Ngày bảo trì tiếp theo
        public DateTime? NextMaintenanceDate => (LastMaintenanceDate.HasValue && MaintenanceAlert !=null) ? 
                                                        LastMaintenanceDate.Value.AddDays((double)MaintenanceAlert) : null;
        //Số lượng dập lũy kế
        public int? StampQuantity { get; set; }
        //Số lượng dập  hiện tại
        public int? CurrentStampQuantity { get; set; }
        //Số lượng dập cần cảnh báo
        public int? StampQuantityAlert { get; set; }
        //Trạng thái
        public bool? Actived { get; set; }
        public bool IsMold { get; set; }
        //Ngày mua mới
        public DateTime? BuyDate { get; set; }
        //Ngày thử nghiệm
        public DateTime? TestDate { get; set; }
        //Ngày đưa vào sử dụng
        public DateTime? UsedDate { get; set; }
        //Thử nghiệm khuôn
        public bool? isMoldTest { get; set; }
        //List Routing
        public List<ProductRoutingDetailResponse> ProductRoutings { get; set; } = new List<ProductRoutingDetailResponse>();
    }

    public class ProductRoutingDetailResponse
    {
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
}
