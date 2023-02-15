using Core.SeedWork;

namespace MasterData.Applications.Commands.Company
{
    public class CompanySearchCommand
    {
        public PagingQuery Paging { get; set; } = new PagingQuery();
        public string CompanyName { get; set; }
        public bool? Actived { get; set; }
    }
}
