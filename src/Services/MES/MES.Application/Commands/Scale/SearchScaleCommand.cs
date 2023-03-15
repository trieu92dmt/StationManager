using Core.Models;
using Core.SeedWork;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using MES.Application.DTOs.MES.Scale;

namespace MES.Application.Commands.Scale
{
    public class SearchScaleCommand : IRequest<ScaleListResponse>
    {
        public DatatableViewModel Paging { get; set; }
        //Nhà máy
        public string Plant { get; set; }
        //Đầu cân
        public string ScaleCode { get; set; }
        //Trạng thái
        public bool? Status { get; set; }
    }

    public class SearchScaleCommandHandler : IRequestHandler<SearchScaleCommand, ScaleListResponse>
    {
        private readonly IRepository<ScaleModel> _scaleRepo;

        public SearchScaleCommandHandler(IRepository<ScaleModel> scaleRepo)
        {
            _scaleRepo = scaleRepo;
        }

        public Task<ScaleListResponse> Handle(SearchScaleCommand request, CancellationToken cancellationToken)
        {
            var query = _scaleRepo.GetQuery(x => 
                                                 //Lọc theo plant
                                                 (!string.IsNullOrEmpty(request.Plant) ? x.Plant == request.Plant : true) &&
                                                 //Lọc theo mã đầu cân
                                                 (!string.IsNullOrEmpty(request.ScaleCode) ? x.ScaleCode == request.ScaleCode : true) &&
                                                 (request.Status.HasValue ? x.Actived == request.Status : true))
                                  .OrderBy(x => x.Plant).ThenBy(x => x.ScaleCode)
                                  .Select(x => new ScaleDataResponse
                                  {
                                      ScaleId = x.ScaleId,
                                      //Plant
                                      Plant = x.Plant,
                                      //Mã đầu cân
                                      ScaleCode = x.ScaleCode,
                                      //Tên đầu cân
                                      ScaleName = x.ScaleName,
                                      //Cân tích hợp
                                      isIntegrated = x.ScaleType == true ? true : false,
                                      //Cân xe tải
                                      isTruckScale = x.isCantai == true ? true : false,
                                      //Trạng thái
                                      Status = x.Actived == true ? true : false
                                  });

            int filterResultsCount = 0;
            int totalResultsCount = 0;
            int totalPagesCount = 0;

            var response = new ScaleListResponse();


            //Lấy danh sách TestTarget có phân trang
            var res = CustomSearch.CustomSearchFunc<ScaleDataResponse>(request.Paging, out filterResultsCount, out totalResultsCount, out totalPagesCount, query, "STT");

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
            response.PagingRep.FilterResultsCount = filterResultsCount;
            response.PagingRep.TotalResultsCount = totalResultsCount;
            response.PagingRep.TotalPagesCount = totalPagesCount;

            return Task.FromResult(response);
        }

    }
}
