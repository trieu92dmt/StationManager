using Core.Interfaces.Databases;
using Core.SeedWork;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using StationManager.Application.Commands.Admin;
using StationManager.Application.DTOs.CarCompany.Admin;

namespace StationManager.Application.Queries.Admin
{
    public interface ICarCompanyQuery
    {
        Task<List<CarCompanyResponse>> SearchCarCompany(SearchCarCompanyCommand command);
        Task<CarCompanyDetailResponse> GetCarCompanyDetail(Guid carCompanyId);
    }

    public class CarCompanyQuery : ICarCompanyQuery
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<AccountModel> _accRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<TripModel> _tripRepo;
        private readonly IRepository<provinces> _proviceRepo;

        public CarCompanyQuery(IUnitOfWork unitOfWork, IRepository<CarCompanyModel> carCompanyRepo, 
                               IRepository<AccountModel> accRepo, IRepository<CatalogModel> cataRepo,
                               IRepository<TripModel> tripRepo, IRepository<provinces> proviceRepo)
        {
            _unitOfWork = unitOfWork;
            _carCompanyRepo = carCompanyRepo;
            _accRepo = accRepo;
            _cataRepo = cataRepo;
            _tripRepo = tripRepo;
            _proviceRepo = proviceRepo;
        }

        public async Task<CarCompanyDetailResponse> GetCarCompanyDetail(Guid carCompanyId)
        {
            //Get CarCompany
            var carCompany = await _carCompanyRepo.GetQuery()
                                                  .Include(x => x.CarCompany_Package_MappingModel).ThenInclude(x => x.Package)
                                                  .Include(x => x.RateModel)
                                                  .Include(x => x.RouteModel)
                                                  .Include(x => x.Account)
                                                  .Include(x => x.CarModel)
                                                  .FirstOrDefaultAsync(x => x.CarCompanyId == carCompanyId);

            //Get catalog
            var catalogs = _cataRepo.GetQuery(x => x.CatalogTypeCode == "CarCompanyStatus").AsNoTracking();

            //Get list province
            var provinces = _proviceRepo.GetQuery().AsNoTracking();

            //Get list Trip
            var trips = _tripRepo.GetQuery().AsNoTracking();

            //Get trip max a day
            var tripMaxs = await _tripRepo.GetQuery(x => x.CarCompanyId == carCompany.CarCompanyId) 
                                 .GroupBy(x => x.CreateTime.Value.Date)
                                 .Select(x => new
                                 {
                                     key = x.Key,
                                     value = x.Count()
                                 }).OrderBy(x => x.value).FirstOrDefaultAsync();

            //Đếm thời gian còn có thể sử dụng
            //Lấy ra gói đăng ký sớm nhất còn hạn sử dụng
            var packageFirst = carCompany.CarCompany_Package_MappingModel
                                      .OrderBy(x => x.CreatedTime)
                                      .Where(x => x.ExpireTime.Value >= DateTime.Now).FirstOrDefault();
            //Tính expireDate
            DateTime? expireDate = packageFirst != null ?
                              packageFirst.ExpireTime.Value.AddDays(
                              carCompany.CarCompany_Package_MappingModel
                                        .Where(x => x.CreatedTime.Value > packageFirst.CreatedTime.Value)
                                        .Sum(x => (double)x.Package.Duration)) : null;

            //Select data
            var carCompanyDetail = new CarCompanyDetailResponse();
            carCompanyDetail.CarCompanyId = carCompanyId;
            carCompanyDetail.CarCompanyCode = carCompany.CarCompanyCode;
            carCompanyDetail.CarCompanyName = carCompany.CarCompanyName;
            carCompanyDetail.Username = carCompany.Account.UserName;
            carCompanyDetail.Email = carCompany.Email;
            carCompanyDetail.Hotline = carCompany.Hotline;
            carCompanyDetail.PhoneNumber = carCompany.PhoneNumber;
            carCompanyDetail.OfficeAddress = carCompany.OfficeAddress;
            carCompanyDetail.Description = carCompany.Description;
            carCompanyDetail.CreateTime = carCompany.CreateTime;
            carCompanyDetail.Status = catalogs.FirstOrDefault(x => x.CatalogCode == carCompany.Status).CatalogName;
            carCompanyDetail.CarQuantity = carCompany.CarModel.Count();
            carCompanyDetail.RouteQuantity = carCompany.RouteModel.Count();
            carCompanyDetail.MaxTripQuantity = tripMaxs != null ? tripMaxs.value : 0;
            carCompanyDetail.ExpireDate = expireDate != null ? expireDate : null;
            carCompanyDetail.Packages = carCompany.CarCompany_Package_MappingModel
                                                  .Select(x => new Package
                                                  {
                                                      PackageId = x.CarCompany_Package_MappingId,
                                                      PackageCode = x.Package.PackageCode,
                                                      PackageName = x.Package.PackageName,
                                                      Duration = x.Package.Duration ?? 0,
                                                      Price = x.Package.Price ?? 0,
                                                      CarLimit = x.Package.CarQuantity ?? 0,
                                                      RouteLimit = x.Package.RouteQuantity ?? 0,
                                                      TripPerDay = x.Package.TripPerDay ?? 0,
                                                      CreateTime = x.CreatedTime,
                                                  }).OrderByDescending(x => x.CreateTime).ToList();
            carCompanyDetail.Routes = carCompany.RouteModel
                                                .Select(x => new Route
                                                {
                                                    RouteId = x.RouteId,
                                                    RouteCode = x.RouteCode,
                                                    StartPoint = provinces.FirstOrDefault(p => p.code == x.StartPoint).full_name,
                                                    EndPoint = provinces.FirstOrDefault(p => p.code == x.EndPoint).full_name,
                                                    CreateTime = x.CreateTime,
                                                    Active = x.Actived,
                                                    TripQuantity = trips.Where(t => t.RouteId == x.RouteId).Count(),
                                                }).ToList();
            carCompanyDetail.Rates = carCompany.RateModel
                                                .OrderByDescending(x => x.CreatedTime)
                                                .Select(x => new Rate
                                                {
                                                    RateId = x.RateId,
                                                    Content = x.Content,
                                                    CreateTime = x.CreatedTime,
                                                    RatePoint = x.Rate ?? 0
                                                }).ToList();

            return carCompanyDetail;
        }

        public async Task<List<CarCompanyResponse>> SearchCarCompany(SearchCarCompanyCommand command)
        {
            var users = _accRepo.GetQuery().AsNoTracking();

            var catalogs = _cataRepo.GetQuery().AsNoTracking();

            var response = await _carCompanyRepo.GetQuery(x => (!string.IsNullOrEmpty(command.CompanyName) ? x.CarCompanyName.Contains(command.CompanyName.Trim()) : true) &&
                                                         (!string.IsNullOrEmpty(command.Email) ? x.Email.Contains(command.Email.Trim()) : true) &&
                                                         (!string.IsNullOrEmpty(command.Hotline) ? x.Hotline.Contains(command.Hotline.Trim()) : true) &&
                                                         (!string.IsNullOrEmpty(command.PhoneNumber) ? x.PhoneNumber.Contains(command.PhoneNumber.Trim()) : true) &&
                                                         (!string.IsNullOrEmpty(command.Status) ? x.Status == command.Status : true))
                                          .Select(x => new CarCompanyResponse
                                          {
                                              CarCompanyId = x.CarCompanyId,
                                              CarCompanyCode = x.CarCompanyCode,
                                              CarCompanyName = x.CarCompanyName,
                                              Username = users.FirstOrDefault(u => u.AccountId == x.AccountId).UserName,
                                              PhoneNumber = x.PhoneNumber,
                                              Email = x.Email,
                                              Hotline = x.Hotline,
                                              OfficeAddress = x.OfficeAddress,
                                              Description = x.Description,
                                              CreateTime = x.CreateTime,
                                              Status = catalogs.FirstOrDefault(c => c.CatalogCode == x.Status && 
                                                                               c.CatalogTypeCode == "CarCompanyStatus").CatalogName
                                          }).OrderByDescending(x => x.CarCompanyCode).ToListAsync();
            var index = 1;
            foreach(var item in response)
            {
                item.STT = index;
                index++;
            }

            return response;
        }
    }
}
