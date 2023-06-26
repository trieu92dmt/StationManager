namespace StationManager.Application.DTOs.CarCompany
{
    public class DeliveryResponse
    {
        public Guid DeliveryId { get; set; }
        public string DeliveryCode { get; set; }
        public string TripInfo { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool isShipAtHome { get; set; }
        public decimal Cost { get; set; }
        public string Status { get; set; }
    }
}
