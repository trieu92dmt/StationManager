using Core.Models;
using MediatR;
using MES.Application.Services;
using Microsoft.IdentityModel.Tokens;
using Shared.WeighSession;

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
        private readonly IWeighSessionService _weiSsService;

        public SearchScaleCommandHandler(IWeighSessionService weiSsService)
        {
            _weiSsService = weiSsService;
        }

        public async Task<ScaleListResponse> Handle(SearchScaleCommand request, CancellationToken cancellationToken)
        {
            return await _weiSsService.SearchScale(request);
        }

    }
}
