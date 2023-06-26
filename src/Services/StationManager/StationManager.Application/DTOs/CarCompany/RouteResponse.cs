namespace StationManager.Application.DTOs.CarCompany
{
    public class RouteResponse
    {
        public Guid RouteId { get; set; }
        public string RouteCode { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public decimal Distance { get; set; }
        public string Description { get; set; }
    }
}
