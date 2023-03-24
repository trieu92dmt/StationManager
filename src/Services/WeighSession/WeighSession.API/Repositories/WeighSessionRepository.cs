using Core.Properties;
using Core.SeedWork;
using Core.SeedWork.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.WeighSession;
using System.Threading;
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
        private readonly DataCollectionContext _context;

        public WeighSessionRepository(IRepository<ScaleModel, DataCollectionContext> scaleRepo, 
                                      IRepository<WeighSessionModel, DataCollectionContext> weiSsRepo,
                                      IRepository<WeightMonitorModel, DataCollectionContext> weiMonitorRepo,
                                      DataCollectionContext context)
        {
            _scaleRepo = scaleRepo;
            _weiSsRepo = weiSsRepo;
            _weiMonitorRepo = weiMonitorRepo;
            _context = context;
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
                Weight = weighSs != null ? weighSs.TotalWeight/1000 : 0,
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

            if (weighSession == null)
                return null;

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

        public async Task<ApiResponse> SaveScale(SaveScaleRequest request)
        {
            //Tạo response
            var response = new ApiResponse()
            {
                Code = 200,
                IsSuccess = true,
                Message = String.Format(CommonResource.Msg_Success, "Thêm mới cân"),
                Data = true
            };

            //Duyệt list scale đầu vào
            //Checkk tồn tại
            var scale = await _scaleRepo.FindOneAsync(x => x.ScaleCode == request.ScaleCode);

            //Query screen
            //var screens = _screenRepo.GetQuery().AsNoTracking();

            if (scale != null)
            {
                response.IsSuccess = false;
                response.Message = $"Scale {request.ScaleCode} đã tồn tại";
                response.Data = false;
                return response;
            }

            //Không tồn tại thì tạo mới
            scale = new Infrastructure.Models.ScaleModel
            {
                //ScaleId = Guid.NewGuid(),
                Plant = request.Plant,
                ScaleCode = request.ScaleCode,
                ScaleName = request.ScaleName,
                ScaleType = request.isIntegrated,
                IsCantai = request.isTruckScale,
                Actived = true
            };

            //var mapping = new List<Screen_Scale_MappingModel>();
            ////Thêm mapping cân và màn hình
            //foreach (var item in request.Screens)
            //{
            //    mapping.Add(new Screen_Scale_MappingModel
            //    {
            //        Screen_Scale_Mapping_Id = Guid.NewGuid(),
            //        ScreenId = screens.FirstOrDefault(x => x.ScreenCode == item).ScreenId,
            //        ScaleId = scale.ScaleId
            //    });
            //}


            _scaleRepo.Add(scale);
            //_screenScaleRepo.AddRange(mapping);
            await _context.SaveChangesAsync();

            //Trả response
            return response;
        }

        public async Task<List<ScaleStatusReportResponse>> ScaleStatusReport(ScaleStatusReportRequest request)
        {
            //Get query theo cân
            var scaleQuery = await _scaleRepo.GetQuery(x => (!string.IsNullOrEmpty(request.Plant) ? x.Plant == request.Plant : true) &&
                                            (request.ScaleCode != null && request.ScaleCode.Any() ? request.ScaleCode.Contains(x.ScaleCode) : true))
                                       .Select(x => new ScaleStatusReportResponse()
                                       {
                                           Plant = x.Plant,
                                           ScaleCode = x.ScaleCode,
                                           isIntegrate = x.ScaleType.HasValue ? x.ScaleType.Value : false,
                                           //Status = scMonitor.Type == "C" ? "Connect" : scMonitor.Type == "D" ? "Disconnect" : scMonitor.Type == "S" ? "Start" : "Reset",
                                           //StartTime = scMonitor.StartTime,
                                           //WeighSession = scMonitor.WeightSessionCode
                                           Status = "",
                                           StartTime = null,
                                           WeighSession = ""
                                       }).ToListAsync();
            //Gán số thứ tự
            int index = 0;
            foreach (var scale in scaleQuery)
            {
                scale.STT = ++index;
            };

            return scaleQuery;
        }

        public async Task<ScaleListResponse> SearchScale(SearchScaleRequest request)
        {
            var query = await _scaleRepo.GetQuery(x =>
                                                 //Lọc theo plant
                                                 (!string.IsNullOrEmpty(request.Plant) ? x.Plant == request.Plant : true) &&
                                                 //Lọc theo mã đầu cân
                                                 (!string.IsNullOrEmpty(request.ScaleCode) ? x.ScaleCode == request.ScaleCode : true) &&
                                                 (request.Status.HasValue ? x.Actived == request.Status : true))
                                  .OrderBy(x => x.Plant).ThenBy(x => x.ScaleCode)
                                  .Select(x => new ScaleDataResponse
                                  {
                                      //ScaleId = x.ScaleId,
                                      //Plant
                                      Plant = x.Plant,
                                      //Mã đầu cân
                                      ScaleCode = x.ScaleCode,
                                      //Tên đầu cân
                                      ScaleName = x.ScaleName,
                                      //Cân tích hợp
                                      isIntegrated = x.ScaleType == true ? true : false,
                                      //Cân xe tải
                                      isTruckScale = x.IsCantai == true ? true : false,
                                      //Trạng thái
                                      Status = x.Actived == true ? true : false
                                  }).ToListAsync();

            int filterResultsCount = 0;
            int totalResultsCount = 0;
            int totalPagesCount = 0;

            var response = new ScaleListResponse();


            //Lấy danh sách TestTarget có phân trang
            var res = CustomSearch.CustomSearchFunc<ScaleDataResponse>(request.Paging, out filterResultsCount, out totalResultsCount, out totalPagesCount, query.AsQueryable(), "STT");

            //Đánh số thứ tự
            if (res != null && res.Count() > 0)
            {
                int i = request.Paging.offset;
                foreach (var item in res)
                {
                    i++;
                    item.STT = i;
                }
            }

            response.Scales = res;
            response.FilterResultsCount = filterResultsCount;
            response.TotalResultsCount = totalResultsCount;
            response.TotalPagesCount = totalPagesCount;

            return response;

        }

        public async Task<SearchScaleMonitorResponse2> SearchScaleMonitor(SearchScaleMinitorRequest request)
        {
            var result = new SearchScaleMonitorResponse2();

            //Get query đợt cân
            var weiSsQuery = _weiSsRepo.GetQuery().AsNoTracking();

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
                //StartTime = weiSsQuery.FirstOrDefault(w => w.WeighSessionCode == x.WeightSessionCode) != null ?
                            //weiSsQuery.FirstOrDefault(w => w.WeighSessionCode == x.WeightSessionCode).StartTime : null,
                //TG kết thúc
                //EndTime = weiSsQuery.FirstOrDefault(w => w.WeighSessionCode == x.WeightSessionCode) != null ?
                          //weiSsQuery.FirstOrDefault(w => w.WeighSessionCode == x.WeightSessionCode).EndTime : null,
                //Thời gian ghi nhận
                RecordTime = x.CreateTime,
                //Loại
                Type = x.Type
            }).ToListAsync();

            #region Phân trang
            var totalRecords = data.Count();

            //Sorting
            var dataSorting = PagingSorting.Sorting(request.Paging, data.AsQueryable());
            //Phân trang
            var responsePaginated = PaginatedList<SearchScaleMonitorResponse>.Create(dataSorting, request.Paging.Offset, request.Paging.PageSize);
            var res = new PagingResultSP<SearchScaleMonitorResponse>(responsePaginated, totalRecords, request.Paging.PageIndex, request.Paging.PageSize);

            //Đánh số thứ tự
            if (res.Data.Any())
            {
                int i = request.Paging.Offset;
                foreach (var item in res.Data)
                {
                    i++;
                    item.STT = i;
                }
            }

            result.Data = res.Data.ToList();
            result.TotalCount = res.Paging.TotalCount;
            result.TotalPages = res.Paging.TotalPages;
            result.PageSize = res.Paging.PageSize;
            result.PageIndex = res.Paging.PageIndex;

            #endregion
            return result;
        }

        public async Task<bool> UpdateScale(UpdateScaleRequest request)
        {
            //Get query screen
            //var screenQuery = _screenRepo.GetQuery().AsNoTracking();

            //Lấy ra scale cần chỉnh sửa
            var scale = await _scaleRepo.FindOneAsync(x => x.ScaleCode == request.Scales[0].ScaleCode);

            //cập nhật
            //Tên đầu cân
            scale.ScaleName = request.Scales[0].ScaleName;
            //Loại cân
            scale.ScaleType = request.Scales[0].isIntegrated;
            //Là cân xe tải ?
            scale.IsCantai = request.Scales[0].isTruckScale;
            scale.Actived = request.Scales[0].isActived;


            //Thêm mapping giữa màn hình và cân
            //Lấy ra danh sách đã mapping
            //var mappings = _mappingRepo.GetQuery().Where(x => x.ScaleId == scale.ScaleId);
            //Xóa những mapping đã có
            //_mappingRepo.RemoveRange(mappings);
            //Tạo mới mapping
            //var listMap = new List<Screen_Scale_MappingModel>();
            //foreach (var item in request.Scales[0].Screens)
            //{
            //    listMap.Add(new Screen_Scale_MappingModel
            //    {
            //        Screen_Scale_Mapping_Id = Guid.NewGuid(),
            //        ScaleId = request.Scales[0].ScaleId,
            //        ScreenId = screenQuery.FirstOrDefault(x => x.ScreenCode == item).ScreenId,
            //        Actived = true
            //    });
            //}
            //Thêm mới
            //_mappingRepo.AddRange(listMap);

            await _context.SaveChangesAsync();

            //Trả response
            return true;
        }
    }
}
