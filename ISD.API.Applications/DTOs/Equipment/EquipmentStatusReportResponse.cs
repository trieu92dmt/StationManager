using ISD.API.Applications.DTOs.Common;

namespace ISD.API.Applications.DTOs.Equipment
{
    public class EquipmentStatusReportResponse
    {     
        public int STT { get; set; }
        public Guid Id { get; set; }
        public string WorkShopName { get; set; }
        public Guid? WorkShopId { get; set; }
        public Guid? WorkOrderCardId { get; set; }
        public string EquipmentName { get; set; }
        public string Type { get; set; }
        public int? NumberWorker { get; set; }
        public string WorkOrderCode { get; set; }
        public string Quantity { get; set; }
        public string ProductCode { get; set; }
        public string KSSXNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public string HourStart => StartDate.HasValue ? $"{StartDate.Value.ToString("HH:mm")}" : null;
        public DateTime? EndDate { get; set; }
        public string HourEnd { get; set; }
        public string NextWorkOrder { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public string MachineChainGroup { get; set; }
        public int? OrderIndex { get; set; }
        public string StepCode { get; set; }
    }

    public class ListEquipmentStatusReportResponse
    {
        public List<EquipmentStatusReportResponse> EquipmentStatusReports { get; set; } = new List<EquipmentStatusReportResponse>();

        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }
}
