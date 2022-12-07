using ISD.API.Applications.DTOs.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.WorkOrder
{
    public class DetailsWOResponse
    {
        //Lệnh SX
        public string WorkOrderCode { get; set; }
        //Mã LSX tổng hợp
        public string ParentWorkOrderCode { get; set; }
        //Tên TP/BTP
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        //BOM Version
        public string BOMVersion { get; set; }
        //SL phát lệnh
        public decimal? Quantity { get; set; }
        //Ngày phát lệnh
        public string DocumentDate { get; set; }
        //Ngày BĐSX dự kiến
        public string EstimateFromDate { get; set; }
        //Ngày KTSX dự kiến
        public string EstimateToDate { get; set; }
        //Ngày BĐSX thực tế
        public string ActualStartDate { get; set; }
        //Ngày KTSX thực tế
        public string ActualEndDate { get; set; }
        //Khách hàng
        public string CustomerName { get; set; }
        //Số PO/ Hợp đồng/ Báo giá
        public string POCode { get; set; }
        //SL SP đặt hàng
        public decimal? QuantityOrder { get; set; }
        //SL giao mẫu
        public decimal? SampleQuantity { get; set; }
        //Unit SO Detail
        public string UnitSO { get; set; }
        //ĐVT
        public string Unit { get; set; }
        //Phiếu tiếp nhận TT
        public string InforForm { get; set; }
        //Ngày giao hàng
        public DateTime? DeliveryDate { get; set; }
        //Kích thước SP(mm)
        public decimal? ProductSize { get; set; }
        //Kích thước khổ trải SP(mm)
        public string ProductSpreadSize { get; set; }
        //Số SP/Khổ in(hoặc khổ cắt)
        public decimal? PrintSize { get; set; }
        //Đơn vị gia công TP
        public string ProcessingUnit { get; set; }
        //Tình trạng SP	
        public string ProductStatus { get; set; }
        //Mô tả SP	
        public string Description { get; set; }
        //Số phiếu thiết kế
        public string DesignVote { get; set; }
        //Phụ trách thiết kế
        public string DesignBy { get; set; }
        //Mã bản vẽ Marquette
        public string MarquetteCode { get; set; }
        //Mã bản vẽ
        public string DrawCode { get; set; }
        //Mã bản vẽ khuôn
        public string MoldDrawCode { get; set; }
        //Ghi chú mẫu kèm theo
        public string AttachedNote { get; set; }
        //Created By
        public Guid? CreateBy { get; set; }
        //Ghép bài	
        //Lệnh ghép bài/ phiếu thiết kế
        public string MatchCard { get; set; }
        //Tên hạng mục in
        public string PrintItem { get; set; }
        //Kiểu in
        public string PrintStyle { get; set; }
        //Công ty gia công in
        public string PrintCompany { get; set; }
        //NCC kẽm
        public string ZincSupplier { get; set; }
        //Ngày yc có hàng in
        public string PrintReqDate { get; set; }
        //Trạng thái
        public string PrintStatus { get; set; }
        //Loại LSX
        public string WOType { get; set; }
        //Ghi chú sản phẩm
        public string ProductNote { get; set; }
        //Quy cách đóng gói
        public string Specification { get; set; }
        //Thông tim component
        public List<WOProductResponse> Components { get; set; }
        //Thôn tin khuôn
        public List<WOMoldResponse> WOMolds { get; set; }
        //Thông tin công đoạn sản xuất
        public List<RoutingDetailsResponse> RoutingDetails { get; set; }
        
    }
}
