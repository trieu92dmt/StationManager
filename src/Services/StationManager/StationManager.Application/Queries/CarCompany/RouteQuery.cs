using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.DTOs.CarCompany.Request;

namespace StationManager.Application.Queries.CarCompany
{
    public interface IRouteQuery
    {
        Task<List<RouteResponse>> GetListRoute(GetListRouteRequest request);

        Task<DetailRouteResponse> GetDetailRoute(Guid routeId);
    }

    public class RouteQuery : IRouteQuery
    {
        private readonly IRepository<RouteModel> _routeRepo;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<districts> _districtRepo;
        private readonly IRepository<provinces> _provinceRepo;

        public RouteQuery(IRepository<RouteModel> routeRepo, IRepository<CarCompanyModel> carCompanyRepo,
                          IRepository<districts> districtRepo, IRepository<provinces> provinceRepo)
        {
            _routeRepo = routeRepo;
            _carCompanyRepo = carCompanyRepo;
            _districtRepo = districtRepo;
            _provinceRepo = provinceRepo;
        }

        public async Task<DetailRouteResponse> GetDetailRoute(Guid routeId)
        {
            var route = await _routeRepo.FindOneAsync(x => x.RouteId == routeId);

            var provinces = _provinceRepo.GetQuery().AsNoTracking();

            return new DetailRouteResponse
            {
                RouteId = route.RouteId,
                RouteCode = route.RouteCode,
                StartPoint = provinces.FirstOrDefault(d => d.code == route.StartPoint)?.name,
                EndPoint = provinces.FirstOrDefault(d => d.code == route.EndPoint)?.name,
                Description = route.Description,
                Distance = route.Distance ?? 0
            };
        }

        public async Task<List<RouteResponse>> GetListRoute(GetListRouteRequest request)
        {
            var carCompany = await _carCompanyRepo.FindOneAsync(x => x.AccountId == request.AccountId);

            var provinces = _provinceRepo.GetQuery().AsNoTracking();

            var routes = await _routeRepo.GetQuery(x => x.CarCompanyId == carCompany.CarCompanyId &&
                                    (!string.IsNullOrEmpty(request.StartPoint) ? x.StartPoint == request.StartPoint : true) &&
                                    (!string.IsNullOrEmpty(request.EndPoint) ? x.EndPoint == request.EndPoint : true))
                                         .Select(x => new RouteResponse
                                         {
                                             RouteId = x.RouteId,
                                             EndPoint = provinces.FirstOrDefault(d => d.code == x.EndPoint).name,
                                             StartPoint = provinces.FirstOrDefault(d => d.code == x.StartPoint).name,
                                             Distance = x.Distance ?? 0,
                                             Description = x.Description,
                                             RouteCode = x.RouteCode
                                         })
                                         .ToListAsync();
            return routes;
        }
    }
}
