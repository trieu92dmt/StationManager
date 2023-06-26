namespace StationManager.Application.DTOs.CarCompany
{
    public class SeatDiagramResponse
    {
        public int? Levels { get; set; }
        public int? Columns { get; set; }
        public int? Rows { get; set; }
        public List<SeatResponse> Seats { get; set; } = new List<SeatResponse>();
    }
}
