using ISD.API.Applications.DTOs.StageTranfer;

namespace ISD.API.Applications.DTOs.WorkOrder
{
    public class GetBarcodeWorkOrderResponse
    {
        public string SOCode { get; set; }
        public string WorkOrderCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        //Số phiếu KSSX
        public string KSSXNumber { get; set; }
        public string MathCard { get; set; }
        public DateTime? ManuDate { get; set; }
        public string ManuDateStr => ManuDate.HasValue ? ManuDate.Value.ToString("dd/MM/yyyy") : null;
        public string StepCode { get; set; }
        public string StepName { get; set; }
        public decimal? QuantityCommand { get; set; }
        //Số lượng theo thẻ treo
        public decimal? QuantityByCard { get; set; }
        public List<MoldByWOResponse> Molds { get; set; } = new List<MoldByWOResponse>();
        public int? ProductOnSheet { get; set; }
        public string Unit { get; set; }
        public InfoOutputRecordResponse InfoOutputRecord { get; set; } = new InfoOutputRecordResponse();
        public InfoQCResponse InfoQC { get; set; } = new InfoQCResponse();
        public string ImagePath { get; set; }
    }

    

    //Khuôn
    public class MoldByWOResponse
    {
        public string MoldCode { get; set; }
        public string MoldName { get; set; }
        public string Serial { get; set; }
    }

    //Thông tin ghi nhận sản lượng
    public class InfoOutputRecordResponse
    {
        public string DepartmentName { get; set; }
        public string RecordBy { get; set; }
        public string CurrentStep { get; set; }
        public string CurrentStepName { get; set; }
        public string NextStep { get; set; }
        public string NextStepName { get; set; }
        public List<QuantityOutputRecordResponse> QuantityOutputRecords { get; set; } = new List<QuantityOutputRecordResponse>();
        public List<EquipmentOuputResponse> Equipments { get; set; } = new List<EquipmentOuputResponse>();
        public List<CommentRecordResponse> DescriptionRecords { get; set; } = new List<CommentRecordResponse>();
    }


    public class EquipmentOuputResponse
    {
        public string EquipmentCode { get; set; }
        public string EquipmentName { get; set; }
    }

    //Thông tin QC
    public class InfoQCResponse
    {
        public string ResultQC { get; set; } 
        public string Description { get; set; } 
        public string StaffInspection { get; set; } 
    }

    //Thông tin ghi chú ghi nhận
    public class CommentRecordResponse
    {
        public Guid? AuthId { get; set; }
        public string AuthName { get; set; }
        public string StepCode { get; set; }
        public string StepName { get; set; }
        public string Content { get; set; }
        public DateTime? TimeComment { get; set; }
        public string TimeCommnetStr => TimeComment.HasValue ? TimeComment.Value.ToString("HH:mm ngày dd/MM/yyyy") : string.Empty; 
    }
}
