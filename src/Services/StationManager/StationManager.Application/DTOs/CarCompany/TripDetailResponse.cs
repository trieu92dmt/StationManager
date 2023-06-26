namespace StationManager.Application.DTOs.CarCompany
{
    public class TripDetailResponse
    {
        public Guid TripId { get; set; }
        public string TripCode { get; set; }
        public string Route { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public decimal TicketPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public string CarNumber { get; set; }
        public string CarType { get; set; }
        public string Driver { get; set; }
        public string DriverName { get; set; }
        public string Description { get; set; }
        public CarDetail CarDetail { get; set; }
        public List<SeatResponse> Seats { get; set; } = new List<SeatResponse>();
    }

    public class CarDetail
    {
        public int? Levels { get; set; }
        public int? Columns { get; set; }
        public int? Rows { get; set; }
    }
}
