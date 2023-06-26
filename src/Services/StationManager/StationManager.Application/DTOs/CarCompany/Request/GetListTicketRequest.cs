namespace StationManager.Application.DTOs.CarCompany.Request
{
    public class GetListTicketRequest
    {
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public DateTime StartDate { get; set; }
    }
}
