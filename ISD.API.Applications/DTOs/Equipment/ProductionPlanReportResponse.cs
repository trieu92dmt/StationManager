using ISD.API.Constant.Common;

namespace ISD.API.Applications.DTOs.Equipment
{
    public class ProductionPlanReportResponse
    {
        //Tên máy/chuyền
        public string EquipmentName { get; set; }
        //Số công nhân đang đứng ca
        public int EmployeeQuantity { get; set; }
        //LSX đang thực hiện
        public string WorkOrder { get; set; }
        //TP/BTP đang thực hiện
        public string ProductName { get; set; }
        //Số phiếu nhận diện
        public string KSSXNumber { get; set; }
        //Số lượng phát lệnh
        public string Quantity { get; set; }
        //Thời gian bắt đầu
        public DateTime? TimeStart { get; set; }
        public string TimeStartStr => TimeStart.HasValue ? TimeStart?.ToString(ISDDateTimeFormat.Hour) : string.Empty;
        //Thời gian dự kiến hoàn thành
        public DateTime? EstimateComplete { get; set; }
        public string EstimateCompleteStr => EstimateComplete.HasValue ? EstimateComplete?.ToString(ISDDateTimeFormat.Hour) : string.Empty;
        //Hướng dẫn sản xuất
        public string ProductionGuide { get; set; }
        //NVL sử dụng
        public string ProductCode { get; set; }
        //LSX đã thực hiện
        public List<WorkOrderItem> ReleasedWorkOrders { get; set; } = new List<WorkOrderItem>();
        //LSX sắp thực hiện
        public List<WorkOrderItem> PlannedWorkOrders { get; set; } = new List<WorkOrderItem>();
    }

    public class WorkOrderItem
    {
        //Số phiếu KSSX
        public string KSSXNumber { get; set; }
        //Lệnh sản xuất
        public string WorkOrderCode { get; set; }
        //Tên thành phẩm
        public string ProductName { get; set; }
        //Số lượng TT/KH
        public string Quantity { get; set; }
        //Công đoạn tiếp theo
        public string NextStep { get; set; }
        //Thời gian hoàn thành công đoạn hiện tại dự kiến
        public DateTime? CurrentEstimateComplete { get; set; }
        public string CurrentEstimateCompleteStr => CurrentEstimateComplete.HasValue ? CurrentEstimateComplete?.ToString(ISDDateTimeFormat.Hour) : null;
    }
}
