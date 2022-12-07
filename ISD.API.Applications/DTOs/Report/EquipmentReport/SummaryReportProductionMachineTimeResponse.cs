using ISD.API.Core.SeedWork;

namespace ISD.API.Applications.DTOs.Report.EquipmentReport
{
    public class DashboadSummaryReportProductionMachineTimeResponse
    {
        public decimal? TotalAllRatedTime { get; set; } = 0;
        public decimal? TotalAllUptime { get; set; } = 0;
        public decimal? TotalAllUptimeCM { get; set; } = 0;
        public decimal? TotalAllPerCM { get; set; } = 0;
        public decimal? TotalAllUptimeHD { get; set; } = 0;
        public decimal? TotalAllPerHD { get; set; } = 0;
        public decimal? TotalAllUptimeVS { get; set; } = 0;
        public decimal? TotalAllPerVS { get; set; } = 0;
        public decimal? TotalAllUptimeNM { get; set; } = 0;
        public decimal? TotalAllPerNM { get; set; } = 0;
        public List<object> Charts { get; set; } = new List<object>();
        public PagingSP Paging { get; set; }
        public List<SummaryReportProductionMachineTimeResponse> SummaryReportProductions { get; set; } = new List<SummaryReportProductionMachineTimeResponse>();

    }
    public class SummaryReportProductionMachineTimeResponse
    {
        public int STT { get; set; }
        public Guid? Id { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentGroup { get; set; }
        public decimal? RatedTime { get; set; }
        public decimal? TotalUptime { get; set; }
        public List<UptimeEquipment> UptimeEquipments { get; set; } = new List<UptimeEquipment>();

    }
    public class UptimeEquipment
    {
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public decimal? ExecutionTime { get; set; }
        public decimal? Percent { get; set; }
        public int? OrderIndex { get; set; }
    }

    public class EquipmentCardLogResponse
    {
        public string StatusCode { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }

    }
}
