namespace ISD.API.Applications.DTOs.Chart
{
    public class ChartBarStackedJsResponse
    {
        public string Type { get; set; } = "bar";
        public List<string> Labels { get; set; } = new List<string>();
        public List<DatasetChartResponse> Datasets { get; set; } = new List<DatasetChartResponse>();
    }

    public class DatasetChartResponse
    {
        public string Label { get; set; }
        public string BackgroundColor { get; set; }
        public List<decimal> Data { get; set; } = new List<decimal>();
    }
}
