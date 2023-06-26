using Core.SeedWork;

namespace StationManager.Application.Commands.Admin
{
    public class SearchCarCompanyCommand
    {
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Hotline { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
    }
}
