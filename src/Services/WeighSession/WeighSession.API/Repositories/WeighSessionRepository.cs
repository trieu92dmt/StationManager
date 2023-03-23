using Core.SeedWork;
using Core.SeedWork.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.WeighSession;
using WeighSession.API.DTOs;
using WeighSession.API.Repositories.Interfaces;
using WeighSession.Infrastructure.Models;

namespace WeighSession.API.Repositories
{
    public class WeighSessionRepository : IWeighSessionRepository
    {
        private readonly IRepository<ScaleModel, DataCollectionContext> _scaleRepo;
        private readonly IRepository<WeighSessionModel, DataCollectionContext> _weiSsRepo;
        private readonly IRepository<WeightMonitorModel, DataCollectionContext> _weiMonitorRepo;

        public WeighSessionRepository(IRepository<ScaleModel, DataCollectionContext> scaleRepo, 
                                      IRepository<WeighSessionModel, DataCollectionContext> weiSsRepo,
                                      IRepository<WeightMonitorModel, DataCollectionContext> weiMonitorRepo)
        {
            _scaleRepo = scaleRepo;
            _weiSsRepo = weiSsRepo;
            _weiMonitorRepo = weiMonitorRepo;
        }

        public async Task<ScaleDetailResponse> GetScaleByCode(string scaleCode)
        {
            //Lấy đầu cân
            var scale = await _scaleRepo.FindOneAsync(x => x.ScaleCode == scaleCode);

            var result = new ScaleDetailResponse
            {
                ScaleCode = scale.ScaleCode,
                ScaleName = scale.ScaleName,
                Plant = scale.Plant,
                Note = scale.Note
            };

            return result;
        }

        public async Task<GetWeighNumResponse> GetWeighNum(string scaleCode)
        {
            //Lấy đầu cân
            var scale = await _scaleRepo.FindOneAsync(x => x.ScaleCode == scaleCode && x.ScaleType == true);

            var Now = DateTime.Now.ToString("yyyyMMdd");

            //Lấy ra số cân của đầu cân có trạng thái đầu cân trong po
            var weighSs = _weiSsRepo.GetQuery(x => x.ScaleCode == scale.ScaleCode && x.DateKey == Now && x.SessionCheck  == 0)
                                    .OrderByDescending(x => x.OrderIndex)
                                    .FirstOrDefault();

            //Check đầu đã được chọn
            //var weighSsChose = weighSs != null ? _weighSsChoseRepo.FindOneAsync(x => x.ScaleCode == weighSs.ScaleCode && x.DateKey == Now && x.OrderIndex == weighSs.OrderIndex) : null;

            var result = new GetWeighNumResponse
            {
                Weight = weighSs != null ? weighSs.TotalWeight : 0,
                WeightQuantity = weighSs != null ? weighSs.TotalNumberOfWeigh : 0,
                StartTime = weighSs != null ? weighSs.StartTime : null,
                Status = weighSs != null ? weighSs.SessionCheck == 0 ? "DANGCAN" : "DACAN" : "",
                isSuccess = weighSs != null ? true : false
            };

            return result;
        }

        public async Task<List<WeightHeadResponse>> GetWeightHeadAsync(string keyWord, string plantCode, string type)
        {
            var result = await _scaleRepo.GetQuery(x =>
                                              //Lọc theo từ khóa
                                              (!string.IsNullOrEmpty(keyWord) ? x.ScaleCode.Contains(keyWord) || x.ScaleName.Contains(keyWord) : true) &&
                                              //Lấy theo mã nhà máy
                                              (!string.IsNullOrEmpty(plantCode) ? x.Plant == plantCode : true))
                                           .OrderBy(x => x.ScaleCode).Select(x => new WeightHeadResponse
                                           {
                                               //Mã đầu cân
                                               Key = x.ScaleCode,
                                               //Mã đầu cân | Tên đầu cân
                                               Value = $"{x.ScaleCode} | {x.ScaleName.Trim()}",
                                               Data = x.ScaleType.Value == true ? true : false,
                                               //Loại cân
                                               Type = x.IsCantai == true ? "CANXETAI" : (x.ScaleType == true ? "TICHHOP" : "KHONGTICHHOP")
                                           }).ToListAsync();

            //var response = await _scaleRepo.GetQuery(x =>
            //                                  //Lọc theo từ khóa
            //                                  (!string.IsNullOrEmpty(keyword) ? x.ScaleCode.Contains(keyword) || x.ScaleName.Contains(keyword) : true) &&
            //                                  //Lấy theo mã nhà máy
            //                                  (!string.IsNullOrEmpty(plantCode) ? x.Plant == plantCode : true))
            //            .OrderBy(x => x.ScaleCode)
            //            .Select(x => new DropdownWeightHeadResponse
            //            {
            //                //Mã đầu cân
            //                Key = x.ScaleCode,
            //                //Mã đầu cân | Tên đầu cân
            //                Value = $"{x.ScaleCode} | {x.ScaleName}",
            //                Data = x.ScaleType.Value == true ? true : false,
            //                //Loại cân
            //                Type = x.isCantai == true ? "CANXETAI" : (x.ScaleType == true ? "TICHHOP" : "KHONGTICHHOP")
            //            }).AsNoTracking().ToListAsync();

            return result;
        }

        public async Task<WeighSessionDetailResponse> GeWeighSessionByScaleCode(string scaleCode)
        {
            //Lấy đợt cân
            var weighSession = await _weiSsRepo.FindOneAsync(x => x.ScaleCode == scaleCode && x.SessionCheck == 0);

            return new WeighSessionDetailResponse
            {
                WeighSessionCode = weighSession.WeighSessionCode,
                DateKey = weighSession.DateKey,
                ScaleCode = weighSession.ScaleCode,
                OrderIndex = weighSession.OrderIndex,
                StartTime = weighSession.StartTime,
                EndTime = weighSession.EndTime,
                TotalNumberOfWeigh = weighSession.TotalNumberOfWeigh,
            };
        }

        public async Task<List<SearchScaleMonitorResponse>> SearchScaleMonitor(SearchScaleMinitorRequest request)
        {
            //Get query
            var query = _weiMonitorRepo.GetQuery().AsNoTracking();

            //Lọc theo plant
            if (!string.IsNullOrEmpty(request.PlantFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(request.PlantFrom))
                    request.PlantTo = request.PlantFrom;

                query = query.Where(x => x.PlantCode.CompareTo(request.PlantFrom) >= 0 &&
                                         x.PlantCode.CompareTo(request.PlantFrom) <= 0);
            }

            //Lọc theo đầu cân
            if (!string.IsNullOrEmpty(request.WeightHeadCodeFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(request.WeightHeadCodeFrom))
                    request.WeightHeadCodeTo = request.WeightHeadCodeFrom;

                query = query.Where(x => x.ScaleCode.CompareTo(request.WeightHeadCodeFrom) >= 0 &&
                                         x.ScaleCode.CompareTo(request.WeightHeadCodeTo) <= 0);
            }

            //Lọc theo loại
            if (!string.IsNullOrEmpty(request.Type))
            {
                query = query.Where(x => x.Type == request.Type);
            }

            //Lọc theo ngày giờ ghi nhận
            if (request.RecordTimeFrom.HasValue)
            {
                if (!request.RecordTimeTo.HasValue) request.RecordTimeTo = request.RecordTimeFrom.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.CreateTime >= request.RecordTimeFrom &&
                                         x.CreateTime <= request.RecordTimeTo);
            }

            //Lấy data
            var data = await query.OrderBy(x => x.ScaleCode).ThenByDescending(x => x.CreateTime).Select(x => new SearchScaleMonitorResponse
            {
                //Mã đầu cân
                WeightHeadCode = x.ScaleCode,
                //Id đợt cân
                WeightSessionId = x.WeightSessionCode,
                //Trọng lượng cân
                Weight = x.Weight,
                //Plant
                Plant = x.PlantCode,
                //Đơn vị
                Unit = "",
                //TG bắt đầu
                StartTime = x.StartTime,
                //TG kết thúc
                EndTime = x.EndTime,
                //Thời gian ghi nhận
                RecordTime = x.CreateTime,
                //Loại
                Type = x.Type
            }).ToListAsync();

            return data;
        }
    }
}
