using ISD.API.Applications.DTOs.Common;
using ISD.API.Constant.Common;

namespace ISD.API.Applications.DTOs.WorkOrder
{
    public class WOResponse
    {
        public int STT { get; set; }
        public Guid WorkOrderId { get; set; }
        //Ưu tiên
        public int? Priority { get; set; }
        //Mã LSX tổng hợp
        public string ParentWorkOrderCode { get; set; }
        //LSX
        public string WorkOrderCode { get; set; }
        //Thành phẩm/Bán thành phẩm
        public string ProductCode { get; set; }
        //Tên thành phẩm/Bán thành phẩm
        public string ProductName { get; set; }
        //BOM version
        public string BOMVersion { get; set; }
        //Số lượng phát lệnh
        public decimal? Quantity { get; set; }
        //ĐVT số lượng phát lệnh
        public string Unit { get; set; }
        //Ngày phát lệnh
        public DateTime? DocumentDate { get; set; }
        public string DocumentDateStr => DocumentDate.HasValue ? DocumentDate.Value.ToString(ISDDateTimeFormat.DdMmyyyy) : string.Empty;
        //Ngày bắt đầu SX dự kiến
        public DateTime? EstimateFromDate { get; set; }
        public string EstimateFromDateStr => EstimateFromDate.HasValue ? EstimateFromDate.Value.ToString(ISDDateTimeFormat.DdMmyyyy) : string.Empty;  
        //Ngày kết thúc SX dự kiến
        public DateTime? EstimateToDate { get; set; }
        public string EstimateToDateStr => EstimateToDate.HasValue ? EstimateToDate.Value.ToString(ISDDateTimeFormat.DdMmyyyy) : string.Empty;
        //Ngày bắt đầu SX thực tế
        public DateTime? ActualStartDate { get; set; }
        public string ActualStartDateStr => ActualStartDate.HasValue ? ActualStartDate.Value.ToString(ISDDateTimeFormat.DdMmyyyy) : string.Empty;
        //Ngày kết thúc SX thực tế
        public DateTime? ActualEndDate { get; set; }
        public string ActualEndDateStr => ActualEndDate.HasValue ? ActualEndDate.Value.ToString(ISDDateTimeFormat.DdMmyyyy) : string.Empty;
        //Số PO/ Hợp đồng/ Báo giá
        public string POCode { get; set; }
        //Khuôn
        public List<MoldResponse> Molds { get; set; } = new List<MoldResponse>();
        public string PrintStatus { get; set; }
        //Trạng thái
        public bool? Actived { get; set; }
        //Cần cập nhật serial khuôn
        public bool? IsUpdateMold { get; set; }
        public bool? IsEdit { get; set; }
    }

    public class WOListResponse
    {
        public List<WOResponse> WorkOrders { get; set; } = new List<WOResponse>();

        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }

    public class MoldResponse
    {
        public string MoldCode { get; set; }
        public string MoldName { get; set; }
        public string Serial { get; set; }
    }
}
