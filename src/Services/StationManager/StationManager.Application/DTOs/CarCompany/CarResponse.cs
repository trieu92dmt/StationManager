namespace StationManager.Application.DTOs.CarCompany
{
    public class CarResponse
    {
        public Guid CarId { get; set; }
        public string CarNumber { get; set; }
        public string CarType { get; set; }
        public int SeatQuantity { get; set; }
        public string Description { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
