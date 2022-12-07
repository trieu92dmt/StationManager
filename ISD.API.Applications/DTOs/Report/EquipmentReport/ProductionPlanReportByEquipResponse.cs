namespace ISD.API.Applications.DTOs.Report.EquipmentReport
{
    public class ProductionPlanReportByEquipResponse
    {
        public string BarcodeName { get; set; }             //Tên máy/chuyền
        public string WorkOrderProcessing { get; set; }     //LSX đang thực hiện
        public string ProductionIns { get; set; }           //Hướng dẫn sản xuất
        public string ProductUsed { get; set; }             //NVL sử dụng
        public int? QuantityWorker { get; set; }            //Số công nhân đứng ca
        public string ProductProcessing { get; set; }       //TP/BTP đang thực hiện
        public string StartTime { get; set; }               //Thời gian bắt đầu
        public string EstimateTimeComplete { get; set; }    //Thời gian dự kiến hoàn thành
        public string NumberPallet { get; set; }            //Số thẻ treo
        public decimal? QuantityOrder { get; set; }         //SL phát lệnh
        public decimal? QuantityByPallet { get; set; }      //SL theo thẻ treo
        public decimal? QuantityRecord { get; set; }        //SL đã ghi nhận
        public decimal? ProductPerPage { get; set; }        //Số SP/Tờ/Công đoạn
        public string Unit { get; set; }                    //ĐƠN VỊ TÍNH
        public List<string> Employees { get; set; } = new List<string>(); //Danh sách nhân viên
        public List<WorkOrderPendingProductionPlanReportResponse> WorkOrderPendings { get; set; } = new List<WorkOrderPendingProductionPlanReportResponse>(); //Danh sách LSX đang chờ

    } 

    //Lệnh sản xuất đang chờ
    public class WorkOrderPendingProductionPlanReportResponse : WorkOrderExcutedReportResponse
    {
        public string NextStep { get; set; }              //Công đoạn tiếp theo
        public string EstimateTimeComplete { get; set; }  //Thời gian hoàn thành dự kiến
        public string EquipmentName { get; set; }         //Máy/Chuyền thực hiện
    }

    public class WorkOrderExcutedReportResponse
    {
        public int STT { get; set; }
        public Guid? WorkOrderId { get; set; }
        public string KSSXVote { get; set; }              //Số phiếu KSSX
        public string WorkOrder { get; set; }             //LSX
        public string ProductName { get; set; }           //Tên thành phẩm
        public decimal? QuantityByPallet { get; set; }    //Số lượng theo thẻ treo
    }
}
