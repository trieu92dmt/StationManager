using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.DTOs.CarCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Queries.CarCompany
{
    public interface IPackageQuery
    {
        Task<List<PackageResponse>> GetListPackage();
    }

    public class PackageQuery : IPackageQuery
    {
        private readonly IRepository<PackageModel> _packageRepo;

        public PackageQuery(IRepository<PackageModel> packageRepo)
        {
            _packageRepo = packageRepo;
        }

        public async Task<List<PackageResponse>> GetListPackage()
        {
            return await _packageRepo.GetQuery().OrderBy(x => x.Duration).Select(x => new PackageResponse
            {
                PackageId = x.PackageId,
                PackageCode = x.PackageCode,
                PackageName = x.PackageName,
                Duration = x.Duration ?? 0,
                CarQuantity = x.CarQuantity ?? 0,
                Price = x.Price ?? 0,
                RouteQuantity = x.RouteQuantity ?? 0,
                TripPerDay = x.TripPerDay ?? 0
            }).ToListAsync();
        }
    }
}
