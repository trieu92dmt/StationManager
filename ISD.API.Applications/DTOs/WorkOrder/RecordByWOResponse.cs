using ISD.API.EntityModels.Models;

namespace ISD.API.Applications.DTOs.WorkOrder
{
    public class RecordByWOResponse
    {
        public string StepCode { get; set; }
        public decimal? QuantityByPallet { get; set; }
        public string QuantityByPalletStr { get; set; }
        public decimal? QuantityOk { get; set; }
        public decimal? QuantityNotOk { get; set; }
        public decimal? QuantityByWO { get; set; }
        public bool? IsFininsh { get; set; }
        public List<DetailRecordOutput> DetailRecordOutputs { get; set; } = new List<DetailRecordOutput>();
    }

    public class RecordByWOOrderByStepResponse
    {
        public string StepCode { get; set; }
        public int? OrderIndex { get; set; }
        public List<OutputRecordModel> ListRecord { get; set; } = new List<OutputRecordModel>();
    }

    public class DetailRecordOutput
    {
        public string StepCode { get; set; }
        public decimal? QuantityByPallet { get; set; }
        public string QuantityByPalletStr { get; set; }
        public decimal? QuantityOk { get; set; }
        public decimal? QuantityNotOk { get; set; }
        public string TimeRecord { get; set; }
        public string RecordBy { get; set; }

    }
}
