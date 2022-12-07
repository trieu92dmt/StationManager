namespace ISD.API.Applications.DTOs.EquipmentRecord
{
    public class GetEquipmentRecordResponse
    {
        public List<string> Equipments { get; set; } = new List<string>();
        public string StatusEquipment { get; set; }
        public string StatusEquipmentCode { get; set; }
        public string FromTime { get; set; }
        public decimal? QuantityOK { get; set; }
        public decimal? QuantityError { get; set; }
        public string EquipmentStopReason { get; set; }
        public List<WorkOrderPerformResponse> WorkOrderPerforms { get; set; } = new List<WorkOrderPerformResponse>();
    }

    public class WorkOrderPerformResponse 
    {
        public int STT { get; set; }
        public Guid Id { get; set; }
        public string WorkOrderCode { get; set; }
        public string ProductCode { get; set; }
        public string KSSXNumber { get; set; }
        public decimal? QuantityByPallet { get; set; }
    }
}
