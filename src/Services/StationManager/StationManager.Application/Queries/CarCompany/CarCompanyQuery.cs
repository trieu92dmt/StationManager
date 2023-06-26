using Core.Extensions;
using Core.SeedWork;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using StationManager.Application.DTOs.CarCompany;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StationManager.Application.Queries.CarCompany
{
    public interface ICarCompanyQuery
    {
        Task<DetailCarCompanyResponse> GetDetailCarCompany(Guid AccountId);
        Task<DetailCarCompanyResponse> GetDetailCarCompanyByUserSide(Guid CarCompanyId);
        Task<PagingResultSP<CarCompanyItemResponse>> GetListCarCompany(int page);
    }

    public class CarCompanyQuery : ICarCompanyQuery
    {
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IRepository<CatalogModel> _cataRepo;
        private readonly IRepository<RateModel> _rateRepo;
        private readonly IRepository<UserModel> _userRepo;

        public CarCompanyQuery(IRepository<CarCompanyModel> carCompanyRepo, IRepository<CatalogModel> cataRepo,
                               IRepository<RateModel> rateRepo, IRepository<UserModel> userRepo)
        {
            _carCompanyRepo = carCompanyRepo;
            _cataRepo = cataRepo;
            _rateRepo = rateRepo;
            _userRepo = userRepo;
        }

        public async Task<DetailCarCompanyResponse> GetDetailCarCompany(Guid AccountId)
        {
            var carCompanyInfo = await _carCompanyRepo.GetQuery(x => x.AccountId.HasValue &&
                                                                     x.AccountId == AccountId)
                                                      .Include(x => x.CarCompany_SocialMedia_MappingModel)
                                                      .FirstOrDefaultAsync();

            var rateQuery = _rateRepo.GetQuery().AsNoTracking();

            var catalogQuery = _cataRepo.GetQuery().AsNoTracking();

            if (carCompanyInfo == null)
            {
                throw new Exception("Không thể tìm thấy nhà xe");
            }


            //Social Media
            var socialMediaCatalog = _cataRepo.GetQuery(x => x.CatalogTypeCode == "SocialMedia");

            var listSocialMedia = new List<SocialMediaResponse>();

            foreach (var item in socialMediaCatalog)
            {
                listSocialMedia.Add(new SocialMediaResponse
                {
                    SocialMediaCode = item.CatalogCode,
                    SocialMediaName = item.CatalogName,
                    Link = carCompanyInfo.CarCompany_SocialMedia_MappingModel.FirstOrDefault(x => x.SocialMediaCode == item.CatalogCode)?.Link
                });
            }

            return new DetailCarCompanyResponse
            {
                CarCompanyId = carCompanyInfo.CarCompanyId,
                CarCompanyCode = carCompanyInfo.CarCompanyCode,
                CarCompanyName = carCompanyInfo.CarCompanyName,
                Email = carCompanyInfo.Email,
                Hotline = carCompanyInfo.Hotline,
                OfficeAddress = carCompanyInfo.OfficeAddress,
                PhoneNumber = carCompanyInfo.PhoneNumber,
                Image = carCompanyInfo.Image,
                SocialMediaResponses = listSocialMedia,
                Description = carCompanyInfo.Description,
                Thumnail = carCompanyInfo.Thumnail,
                Rate = rateQuery.Where(x => x.CarCompanyId == carCompanyInfo.CarCompanyId).Count() > 0 ?
                       Math.Round((decimal)(rateQuery.Where(x => x.CarCompanyId == carCompanyInfo.CarCompanyId).Sum(x => x.Rate)/ 
                       rateQuery.Where(x => x.CarCompanyId == carCompanyInfo.CarCompanyId).Count()),1) : 0,
                RateCount = rateQuery.Where(x => x.CarCompanyId == carCompanyInfo.CarCompanyId).Count()
            };
        }

        public async Task<DetailCarCompanyResponse> GetDetailCarCompanyByUserSide(Guid CarCompanyId)
        {
            var carCompanyInfo = await _carCompanyRepo.GetQuery(x => x.CarCompanyId == CarCompanyId)
                                                      .Include(x => x.CarCompany_SocialMedia_MappingModel)
                                                      .FirstOrDefaultAsync();

            var rateQuery = _rateRepo.GetQuery().AsNoTracking();

            var userQuery = _userRepo.GetQuery().AsNoTracking();

            var catalogQuery = _cataRepo.GetQuery().AsNoTracking();

            if (carCompanyInfo == null)
            {
                throw new Exception("Không thể tìm thấy nhà xe");
            }


            //Social Media
            var socialMediaCatalog = _cataRepo.GetQuery(x => x.CatalogTypeCode == "SocialMedia");

            var listSocialMedia = new List<SocialMediaResponse>();

            foreach (var item in socialMediaCatalog)
            {
                listSocialMedia.Add(new SocialMediaResponse
                {
                    SocialMediaCode = item.CatalogCode,
                    SocialMediaName = item.CatalogName,
                    Link = carCompanyInfo.CarCompany_SocialMedia_MappingModel.FirstOrDefault(x => x.SocialMediaCode == item.CatalogCode)?.Link
                });
            }

            return new DetailCarCompanyResponse
            {
                CarCompanyId = carCompanyInfo.CarCompanyId,
                CarCompanyCode = carCompanyInfo.CarCompanyCode,
                CarCompanyName = carCompanyInfo.CarCompanyName,
                Email = carCompanyInfo.Email,
                Hotline = carCompanyInfo.Hotline,
                OfficeAddress = carCompanyInfo.OfficeAddress,
                PhoneNumber = carCompanyInfo.PhoneNumber,
                Image = carCompanyInfo.Image,
                SocialMediaResponses = listSocialMedia,
                Description = carCompanyInfo.Description,
                Thumnail = carCompanyInfo.Thumnail,
                Rate = rateQuery.Where(x => x.CarCompanyId == carCompanyInfo.CarCompanyId).Count() > 0 ? Math.Round((decimal)(rateQuery.Where(x => x.CarCompanyId == carCompanyInfo.CarCompanyId).Sum(x => x.Rate)/ rateQuery.Where(x => x.CarCompanyId == carCompanyInfo.CarCompanyId).Count()), 1) : 0,
                RateCount = rateQuery.Where(x => x.CarCompanyId == carCompanyInfo.CarCompanyId).Count(),
                RatingList = rateQuery.Where(x => x.CarCompanyId == carCompanyInfo.CarCompanyId)
                                      .Select(x => new RatingResponse
                                      {
                                          RateId = x.RateId,
                                          SenderId = x.SenderId,
                                          Content = x.Content,
                                          Rate = x.Rate ?? 0,
                                          CreateTime = x.CreatedTime,
                                          SenderName = userQuery.FirstOrDefault(u => u.UserId == x.SenderId).Fullname
                                      }).OrderByDescending(x => x.CreateTime).ToList(),
            };
        }

        public async Task<PagingResultSP<CarCompanyItemResponse>> GetListCarCompany(int page)
        {
            var response = _carCompanyRepo.GetQuery(x => x.Actived == true).Include(x => x.RateModel)
                                                 .Select(x => new CarCompanyItemResponse
                                                 {
                                                     CarCompanyId = x.CarCompanyId,
                                                     CarCompanyName = x.CarCompanyName,
                                                     Image = x.Image,
                                                     PhoneNumber = x.PhoneNumber,
                                                     Rate = x.RateModel.Count > 0 ? Math.Round((decimal)(x.RateModel.Sum(x => x.Rate)/x.RateModel.Count),1) : 0,
                                                     RateCount = x.RateModel.Count(),
                                                     Description = x.Description,
                                                 });

            #region Phân trang
            var totalRecords = response.Count();

            var paging = new PagingQuery
            {
                PageSize = 9,
                PageIndex = page,
                OrderByDesc = "Rate"
            };

            //Sorting
            var dataSorting = PagingSorting.Sorting(paging, response);
            //Phân trang
            var responsePaginated = PaginatedList<CarCompanyItemResponse>.Create(dataSorting, paging.Offset, paging.PageSize);
            var res = new PagingResultSP<CarCompanyItemResponse>(responsePaginated, totalRecords, paging.PageIndex, paging.PageSize);
            #endregion

            return await Task.FromResult(res);
        }
    }
}
