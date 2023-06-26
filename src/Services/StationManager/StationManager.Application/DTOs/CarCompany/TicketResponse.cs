namespace StationManager.Application.DTOs.CarCompany
{
    public class TicketResponse
    {
        public Guid TicketId { get; set; }
        public string TicketCode { get; set; }
        public List<string> Seats { get; set; } = new List<string>();
        public string Status { get; set; }
        public decimal? Price { get; set; }
        public DateTime? BookDate { get; set; }
        public DateTime? BuyDate { get; set; }
        public Guid? BuyerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
