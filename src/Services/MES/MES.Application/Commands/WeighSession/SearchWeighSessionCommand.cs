using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using MES.Application.DTOs.MES.WeighSession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.WeighSession
{
    public class SearchWeighSessionCommand : IRequest<List<WeighSessionResponse>>
    {
        //Đầu cân
        public string ScaleCode { get; set; }
        //Ngày thực hiện
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        //Id đợt cân
        public string WeighSessionId { get; set; }
    }

    public class SearchWeighSessionCommandHandler : IRequestHandler<SearchWeighSessionCommand, List<WeighSessionResponse>>
    {
        public SearchWeighSessionCommandHandler()
        {
        }

        public Task<List<WeighSessionResponse>> Handle(SearchWeighSessionCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
