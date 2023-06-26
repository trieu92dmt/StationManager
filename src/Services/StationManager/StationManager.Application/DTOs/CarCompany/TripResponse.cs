namespace StationManager.Application.DTOs.CarCompany
{
    public class TripResponse
    {
        public Guid TripId { get; set; }
        public string TripCode { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public DateTime? StartDate { get; set; }
        public string CarNumber { get; set; }
        public string CarType { get; set; }
        public string Driver { get; set; }
        public string Description { get; set; }
        public bool isHaveEmptySeat => SeatQuantity == SeatUsing ? false : true;
        public int SeatQuantity { get; set; }
        public int SeatUsing { get; set; }
        public int SeatEmpty => SeatQuantity - SeatUsing;
    }
}
