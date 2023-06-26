using Core.Extensions;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MES.Application.DTOs.Common;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.DTOs.Common;

namespace StationManager.Application.Queries.Common
{
    public interface ICommonQuery
    {
        Task<List<CommonResponse>> ListProvince();
        Task<List<Common4Response>> ListProvinceV2(string keyw);
        Task<List<Common4Response>> ListAllProvince();
        Task<List<ProviceDistrictResponse>> ListProvinceDistrict(string keyword);

        Task<List<DistrictByProvinceResponse>> ListDistrictByProvince(string keyword);

        Task<List<CommonResponse>> ListCarType(string keyword, Guid? accountId);

        Task<List<CommonResponse>> ListCarNumber(string keyword, Guid? accountId);

        Task<List<CommonResponse>> ListPosition();

        Task<List<CommonResponse>> ListDriver(Guid? accountId);

        Task<List<Common2Response>> ListRoute(Guid? accountId);

        Task<List<Common2Response>> ListTripByRouteId(Guid routeId, DateTime startDate);
        Task<List<CommonResponse>> ListCarCompany();

        Task<List<CommonResponse>> ListCarCompanyAdmin(string keyword);
        Task<List<CommonResponse>> ListRole();
    }

    public class CommonQuery : ICommonQuery
    {
        private readonly IRepository<provinces> _provinceRepo;
        private readonly IRepository<districts> _districtRepo;
        private readonly IRepository<CarTypeModel> _carTypeRepo;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<CarModel> _carRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<EmployeeModel> _eeRepo;
        private readonly IRepository<RouteModel> _routeRepo;
        private readonly IRepository<TripModel> _tripRepo;
        private readonly IRepository<RolesModel> _roleRepo;

        public CommonQuery(IRepository<provinces> provinceRepo, IRepository<districts> districtRepo, 
                           IRepository<CarTypeModel> carTypeRepo, IRepository<CarCompanyModel> carCompanyRepo,
                           IRepository<CarModel> carRepo, IRepository<CatalogModel> cataRepo,
                           IRepository<EmployeeModel> eeRepo, IRepository<RouteModel> routeRepo,
                           IRepository<TripModel> tripRepo, IRepository<RolesModel> roleRepo)
        {
            _provinceRepo = provinceRepo;
            _districtRepo = districtRepo;
            _carTypeRepo = carTypeRepo;
            _carCompanyRepo = carCompanyRepo;
            _carRepo = carRepo;
            _cataRepo = cataRepo;
            _eeRepo = eeRepo;
            _routeRepo = routeRepo;
            _tripRepo = tripRepo;
            _roleRepo = roleRepo;
        }

        public async Task<List<CommonResponse>> ListCarType(string keyword, Guid? accountId)
        {
            if (accountId.HasValue)
            {
                var company = await _carCompanyRepo.FindOneAsync(x => x.AccountId == accountId);

                return await _carTypeRepo.GetQuery(x => ((x.isCustomCarType == true && x.CarCompanyId == company.CarCompanyId) ||
                                                        (x.isCustomCarType == false)) &&
                                                        (!string.IsNullOrEmpty(keyword) ? x.CarTypeName.Contains(keyword) : true))
                                         .Select(x => new CommonResponse
                                        {
                                            Key = x.CarTypeCode,
                                            Value = x.CarTypeName
                                        }).AsNoTracking().ToListAsync();
            }


            return await _carTypeRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.CarTypeName.Contains(keyword) : true))
            .Select(x => new CommonResponse
            {
                Key = x.CarTypeCode,
                Value = x.CarTypeName
            }).AsNoTracking().ToListAsync();
        }

        public async Task<List<DistrictByProvinceResponse>> ListDistrictByProvince(string keyword)
        {
            return await _districtRepo.GetQuery().Include(x => x.province_codeNavigation)
                                      .Where(x => !string.IsNullOrEmpty(keyword) ? x.name.Contains(keyword) ||
                                                                                   x.province_codeNavigation.name.Contains(keyword) : true)
                                      .Select(x => new DistrictByProvinceResponse
                                      {
                                          ProvinceCode = x.province_codeNavigation.code,
                                          ProvinceName = x.province_codeNavigation.full_name,
                                          DistrictCode = x.code,
                                          DistrictName = x.full_name,
                                      }).AsNoTracking().ToListAsync();
        }

        public async Task<List<CommonResponse>> ListProvince()
        {
            return await _provinceRepo.GetQuery().Select(x => new CommonResponse
            {
                Key = x.code,
                Value = x.name
            }).AsNoTracking().ToListAsync();
        }

        public async Task<List<ProviceDistrictResponse>> ListProvinceDistrict(string keyword)
        {
            return await _provinceRepo.GetQuery().Include(x => x.districts)
                                      .Where(x => !string.IsNullOrEmpty(keyword) ? x.name.Contains(keyword) ||
                                                                                   x.districts.Where(d => d.name.Contains(keyword)).Any() : true)
                                      .Select(x => new ProviceDistrictResponse
            {
                ProvinceCode = x.code,
                ProvinceName = x.name,
                DistrictResponses = x.districts.Where(d => !string.IsNullOrEmpty(keyword) ? d.name.Contains(keyword) : true)
                .Select(d => new DistrictResponse
                {
                    DistrictCode = d.code,
                    DistrictName = d.name,
                }).ToList()
            }).AsNoTracking().ToListAsync();
        }

        public async Task<List<CommonResponse>> ListCarNumber(string keyword, Guid? accountId)
        {
            if (accountId.HasValue)
            {
                var company = await _carCompanyRepo.FindOneAsync(x => x.AccountId == accountId);

                return await _carRepo.GetQuery(x => (x.CarCompanyId == company.CarCompanyId) &&
                                                    (!string.IsNullOrEmpty(keyword) ? x.CarNumber.Contains(keyword) : true))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.CarNumber,
                                             Value = x.CarNumber
                                         }).AsNoTracking().ToListAsync();
            }


            return await _carRepo.GetQuery(x => (!string.IsNullOrEmpty(keyword) ? x.CarNumber.Contains(keyword) : true))
            .Select(x => new CommonResponse
            {
                Key = x.CarNumber,
                Value = x.CarNumber
            }).AsNoTracking().ToListAsync();
        }

        public async Task<List<CommonResponse>> ListPosition()
        {
            return await _cataRepo.GetQuery(x => x.CatalogTypeCode == "Position").Select(x => new CommonResponse
            {
                Key = x.CatalogCode,
                Value = x.CatalogName
            }).ToListAsync();
        }

        public async Task<List<CommonResponse>> ListDriver(Guid? accountId)
        {

            var company = await _carCompanyRepo.FindOneAsync(x => x.AccountId == accountId);

            return await _eeRepo.GetQuery(x => (accountId.HasValue ? x.CarCompanyId == company.CarCompanyId : true) &&
                                               (x.Position == "TAIXE"))
                                         .Select(x => new CommonResponse
                                         {
                                             Key = x.EmployeeCode.ToString(),
                                             Value = x.EmployeeName
                                         }).AsNoTracking().ToListAsync();

        }

        public async Task<List<Common2Response>> ListRoute(Guid? accountId)
        {
            var company = await _carCompanyRepo.FindOneAsync(x => x.AccountId == accountId);

            var provinces = _provinceRepo.GetQuery().AsNoTracking();

            return await _routeRepo.GetQuery(x => (accountId.HasValue ? x.CarCompanyId == company.CarCompanyId : true))
                                         .Select(x => new Common2Response
                                         {
                                             Key = x.RouteId,
                                             Value = $"{provinces.FirstOrDefault(p => p.code == x.StartPoint).name} - " +
                                             $"{provinces.FirstOrDefault(p => p.code == x.EndPoint).name}" 
                                         }).AsNoTracking().ToListAsync();
        }

        public async Task<List<Common2Response>> ListTripByRouteId(Guid routeId, DateTime startDate)
        {
            return await _tripRepo.GetQuery(x => x.RouteId == routeId && x.StartDate.Value.Day == startDate.Day &&
                                                                        x.StartDate.Value.Month == startDate.Month &&
                                                                        x.StartDate.Value.Year == startDate.Year)
                            .Select(x => new Common2Response
                            {
                                Key = x.TripId,
                                Value = x.StartDate.HasValue ? x.StartDate.Value.ToString("HH:mm") : null
                            })
                            .AsNoTracking().ToListAsync();
        }

        public async Task<List<CommonResponse>> ListCarCompany()
        {
            return await _carCompanyRepo.GetQuery(x => x.Actived == true)
                                  .Select(x => new CommonResponse
                                  {
                                      Key = x.CarCompanyCode.ToString(),
                                      Value = x.CarCompanyName
                                  }).ToListAsync();
        }

        public async Task<List<CommonResponse>> ListCarCompanyAdmin(string keyword)
        {
            return await _carCompanyRepo.GetQuery()
                                        .Select(x => new CommonResponse
                                        {
                                            Key = x.CarCompanyCode.ToString(),
                                            Value = $"{x.CarCompanyCode} | {x.CarCompanyName}" 
                                        }).ToListAsync();
        }

        public async Task<List<CommonResponse>> ListRole()
        {
            return await _roleRepo.GetQuery()
                            .OrderByDescending(x => x.OrderIndex)
                            .Select(x => new CommonResponse
                            {
                                Key = x.RolesCode,
                                Value = x.RolesName
                            }).ToListAsync();
        }

        public async Task<List<Common4Response>> ListProvinceV2(string keyword)
        {
            var response = new List<Common4Response>();


            //Bỏ dấu keyword
            var newKw = !string.IsNullOrEmpty(keyword) ? RemoveDiacriticsExtension.RemoveDiacritics(keyword.Trim()) : "";

            //Lấy tỉnh tp
            var listProvince = await _provinceRepo.GetQuery(x => (!string.IsNullOrEmpty(newKw) ?
                                                                 x.full_name_en.ToLower().Contains(newKw) : true))
                                                  .OrderBy(x => x.full_name)
                                                  .Select(x => new Common4Response
                                                  {
                                                      Key = x.code,
                                                      Value = x.full_name,
                                                      Group = "Tỉnh - Thành phố"
                                                  })
                                                  .Take(10).ToListAsync();

            //Lấy quận huyện
            var listDistrict = await _districtRepo.GetQuery(x => (!string.IsNullOrEmpty(newKw) ?
                                                              x.full_name_en.ToLower().Contains(newKw) : true))
                                                  .OrderBy(x => x.full_name)
                                                  .Select(x => new Common4Response
                                                  {
                                                      Key = x.code,
                                                      Value = x.full_name,
                                                      Group = "Quận - Huyện"
                                                  })
                                                  .Take(10).ToListAsync();

            response.AddRange(listProvince);
            response.AddRange(listDistrict);

            return response;
        }

        public async Task<List<Common4Response>> ListAllProvince()
        {
            var response = new List<Common4Response>();

            //Lấy tỉnh tp
            var listProvince = await _provinceRepo.GetQuery()
                                                  .Select(x => new Common4Response
                                                  {
                                                      Key = x.code,
                                                      Value = x.full_name,
                                                      Group = "Tỉnh - Thành phố"
                                                  })
                                                  .ToListAsync();

            //Lấy quận huyện
            var listDistrict = await _districtRepo.GetQuery()
                                                  .Select(x => new Common4Response
                                                  {
                                                      Key = x.code,
                                                      Value = x.full_name,
                                                      Group = "Quận - Huyện"
                                                  })
                                                  .ToListAsync();

            response.AddRange(listProvince);
            response.AddRange(listDistrict);

            return response;
        }
    }
}
