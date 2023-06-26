namespace StationManager.Application.DTOs.CarCompany
{
    public class CarCompanyItemResponse
    {
        public Guid CarCompanyId { get; set; }
        public string Image { get; set; }
        public string CarCompanyName { get; set; }
        public string Description { get; set; }
        public decimal? Rate { get; set; }
        public int RateCount { get; set; }
        public string PhoneNumber { get; set; }
    }
}
