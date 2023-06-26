namespace StationManager.Application.DTOs.CarCompany
{
    public class DetailCarResponse
    {
        public Guid CarId { get; set; }
        public string CarNumber { get; set; }
        public string CarTypeCode { get; set; }
        public string CarTypeName { get; set; }
        public string Description { get; set; }
        public int Levels { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public List<SeatResponse> Seats { get; set; } = new List<SeatResponse>();
    }

    public class SeatResponse
    {
        public Guid SeatId { get; set; }
        public string SeatNumber { get; set; }
        public int Levels { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public string Status { get; set; }
        public bool Actived { get; set; }
    }
}
