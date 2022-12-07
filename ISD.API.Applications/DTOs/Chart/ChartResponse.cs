namespace ISD.API.Applications.DTOs.Chart
{
    public class ChartResponse
    {
        public string Type { get; set; }
        public decimal? Value { get; set; }
    }

    public class BarChartResponse : ChartResponse
    {
        public string Label { get; set; }
    }
}
