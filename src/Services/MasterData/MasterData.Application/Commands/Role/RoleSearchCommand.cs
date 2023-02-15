using Core.SeedWork;

namespace MasterData.Applications.Commands.Role
{
    public class RoleSearchCommand
    {
        public PagingQuery Paging { get; set; } = new PagingQuery();
        public string RoleName { get; set; }
    }
}