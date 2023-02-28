using Authentication.Application.DTOs;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Application.Queries
{
    public interface IAuthQuery
    {
        Task<List<PlantByUserResponse>> GetPlantByUserName(string username);
    }

    public class AuthQuery : IAuthQuery
    {
        private readonly IRepository<AccountModel> _accRep;

        public AuthQuery(IRepository<AccountModel> accRep)
        {
            _accRep = accRep;
        }
        public async Task<List<PlantByUserResponse>> GetPlantByUserName(string username)
        {
            var accountOfStore = await _accRep.GetQuery(x => x.UserName == username)
                                 .Include(x => x.Store).FirstOrDefaultAsync();

            return accountOfStore?.Store?.Select(x => new PlantByUserResponse
            {
                PlantCode = x.SaleOrgCode,
                PlantName = $"{x.SaleOrgCode}|{x.StoreName}"
            }).ToList();
        }
    }
}
