namespace ISD.API.Applications.DTOs.StageTranfer
{
    public class GetNextStepByWOResponse
    {
        public string SOCode { get; set; }
        public string WorkOrderCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string StepCode { get; set; }
        public decimal? QuantityCommand { get; set; }
        public List<QuantityOutputRecordResponse> QuantityOutputRecords { get; set; } = new List<QuantityOutputRecordResponse>();
        public List<HistoryConfirmStage> HistoryConfirmStages { get; set; } = new List<HistoryConfirmStage>();
    }
    public class HistoryConfirmStage
    {
        public string StepCode { get; set; }
        public DateTime? TimeConfirm { get; set; }
        public string ConfirmBy { get; set; }
    }
    public class QuantityOutputRecordResponse
    {
        public decimal? QuantityRecord { get; set; }
        public string Type { get; set; }
    }
}
