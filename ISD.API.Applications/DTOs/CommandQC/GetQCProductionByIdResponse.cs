namespace ISD.API.Applications.DTOs.CommandQC
{
    public class GetQCProductionByIdResponse
    {
        public Guid? qcCommandId { get; set; }
        public Guid? workOrderCardId { get; set; }
        public string StepCode { get; set; }
        public string SOCode { get; set; }
        public string Customer { get; set; }
        public string WorkOrderCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public InfoRecordResponse InfoRecord { get; set; } = new InfoRecordResponse();
        public InfoQCResponse InfoQC { get; set; } = new InfoQCResponse();
    }
    public class InfoRecordResponse
    {
        public string ManuDate { get; set; }
        public List<string> Equipments { get; set; } = new List<string>();
        public string Department { get; set; }
        public string StepFinish { get; set; }
        public string RecordBy { get; set; }
        public decimal? QuantityRecord { get; set; }
        public string RecordTimeFrom { get; set; }
        public string RecordTimeTo { get; set; }

    }

    public class InfoQCResponse
    {
        public int? QuantityVote { get; set; }
        public decimal? QuantityOk { get; set; }
        public decimal? QuantityErr { get; set; }
        public string QCTime { get; set; }
        public string QCStaff { get; set; }
        public string TypeQC { get; set; }
        public decimal? QuantitySample { get; set; }
        public string ImagePath { get; set; }
        public decimal? QuantityTranfered { get; set; }
        public string ResultQC { get; set; }
        public string Description { get; set; }
        public List<DetailQCResponse> DetailQCs { get; set; }

    }
    public class DetailQCResponse
    {
        public Guid? DetailQC { get; set; }
        public string QCTime { get; set; }
        public Guid? TestTargetId { get; set; }
        public string TestTargetName { get; set; }
        public decimal? QuantitySample { get; set; }
        public string ImagePath { get; set; }
        public Guid? ErrorId { get; set; }
        public string ErrorName { get; set; }
        public string ToKnow { get; set; }
        public string ReasonErr { get; set; }
        public decimal? QuantityOk { get; set; }
        public decimal? QuantityErr { get; set; }
        public string Description { get; set; }
        public string Conclusion { get; set; }
        public bool? IsDeleted { get; set; }
        public string DeleteBy { get; set; }
        public string DeleteTime { get; set; }

    }
}
