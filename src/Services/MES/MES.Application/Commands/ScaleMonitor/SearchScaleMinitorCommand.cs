using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using MES.Application.DTOs.MES.ScaleMonitor;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MES.Application.Commands.ScaleMonitor
{
    public class SearchScaleMinitorCommand : IRequest<List<SearchScaleMonitorResponse>>
    {
        //Plant
        public string PlantFrom { get; set; }
        public string PlantTo { get; set; }
        //Đầu cân
        public string WeightHeadCodeFrom { get; set; }
        public string WeightHeadCodeTo { get; set; }
        //Loại
        public string Type { get; set; }
        //Ngày ghi nhận
        public DateTime? RecordTimeFrom { get; set; }
        public DateTime? RecordTimeTo { get; set; }
    }

    public class SearchScaleMinitorCommandHandler : IRequestHandler<SearchScaleMinitorCommand, List<SearchScaleMonitorResponse>>
    {
        private readonly IRepository<ScaleMonitorModel> _scaleMonitorRepo;

        public SearchScaleMinitorCommandHandler(IRepository<ScaleMonitorModel> scaleMonitorRepo)
        {
            _scaleMonitorRepo = scaleMonitorRepo;
        }

        public async Task<List<SearchScaleMonitorResponse>> Handle(SearchScaleMinitorCommand request, CancellationToken cancellationToken)
        {
            //Get query
            var query = _scaleMonitorRepo.GetQuery().AsNoTracking();

            //Lọc theo plant
            if (!string.IsNullOrEmpty(request.PlantFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(request.PlantFrom))
                    request.PlantTo = request.PlantFrom;

                query = query.Where(x => x.Plant.CompareTo(request.PlantFrom) >= 0 &&
                                         x.Plant.CompareTo(request.PlantFrom) <= 0);
            }

            //Lọc theo đầu cân
            if (!string.IsNullOrEmpty(request.WeightHeadCodeFrom))
            {
                //Không có to thì search 1
                if (string.IsNullOrEmpty(request.WeightHeadCodeFrom))
                    request.WeightHeadCodeTo = request.WeightHeadCodeFrom;

                query = query.Where(x => x.Scale.ScaleCode.CompareTo(request.WeightHeadCodeFrom) >= 0 &&
                                         x.Scale.ScaleCode.CompareTo(request.WeightHeadCodeTo) <= 0);
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
                query = query.Where(x => x.RecordTime >= request.RecordTimeFrom &&
                                         x.RecordTime <= request.RecordTimeTo);
            }

            //Lấy data
            var data = await query.OrderBy(x => x.Scale.ScaleCode).ThenByDescending(x => x.RecordTime).Select(x => new SearchScaleMonitorResponse
            {
                //Mã đầu cân
                WeightHeadCode = x.Scale.ScaleCode,
                //Id đợt cân
                WeightSessionId = x.WeightSessionCode,
                //Trọng lượng cân
                Weight = x.Weight,
                //Plant
                Plant = x.Plant,
                //Đơn vị
                Unit = "",
                //TG bắt đầu
                StartTime = x.StartTime,
                //TG kết thúc
                EndTime = x.EndTime,
                //Thời gian ghi nhận
                RecordTime = x.RecordTime,
                //Loại
                Type = x.Type
            }).ToListAsync();

            return data;
        }
    }
}
