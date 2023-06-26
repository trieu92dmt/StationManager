using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.Commands.Permission;
using StationManager.Application.DTOs.CarCompany.Permission;

namespace StationManager.Application.Queries.Permission
{
    public interface IAccountQuery
    {
        Task<List<AccountResponse>> SearchAccount(SearchAccountCommand command);
    }

    public class AccountQuery : IAccountQuery
    {
        private readonly IRepository<AccountModel> _accRepo;

        public AccountQuery(IRepository<AccountModel> accRepo)
        {
            _accRepo = accRepo;
        }

        public async Task<List<AccountResponse>> SearchAccount(SearchAccountCommand command)
        {
            var response = await _accRepo.GetQuery().Include(x => x.Roles)
                                   .Where(x => (!string.IsNullOrEmpty(command.Username) ?
                                               x.UserName.Contains(command.Username.Trim()) : true) &&
                                               (!string.IsNullOrEmpty(command.FullName) ?
                                               x.FullName.Contains(command.FullName.Trim()) : true) &&
                                               (command.Active.HasValue ? x.Actived == command.Active : true) &&
                                               (!string.IsNullOrEmpty(command.Role) ?
                                               x.Roles.FirstOrDefault(r => r.RolesCode == command.Role) != null : true))
                                   .Select(x => new AccountResponse
                                   {
                                       AccountId = x.AccountId,
                                       Username = x.UserName,
                                       FullName = x.FullName,
                                       Active = x.Actived,
                                       Role = string.Join(',', x.Roles.Select(r => r.RolesName).ToList())
                                   }).ToListAsync();

            return response;
        }
    }
}
