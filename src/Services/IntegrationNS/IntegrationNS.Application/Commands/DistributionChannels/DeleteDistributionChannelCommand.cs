using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.DistributionChannels
{
    public class DeleteDistributionChannelCommand : IRequest<DeleteNSResponse>
    {
        public List<string> DistributionChannels { get; set; } = new List<string>();
    }

    public class DeleteDistributionChannelCommandHandler : IRequestHandler<DeleteDistributionChannelCommand, DeleteNSResponse>
    {
        private readonly IRepository<DistributionChannelModel> _disChanelRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDistributionChannelCommandHandler(IRepository<DistributionChannelModel> disChanelRep, IUnitOfWork unitOfWork)
        {
            _disChanelRep = disChanelRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteNSResponse> Handle(DeleteDistributionChannelCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.DistributionChannels.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.DistributionChannels.Count();

            foreach (var distributionChannelDelete in request.DistributionChannels)
            {
                try
                {
                    //Xóa Distribution Channel
                    var distributionChannel = await _disChanelRep.FindOneAsync(x => x.DistributionChannelCode == distributionChannelDelete);
                    if (distributionChannel is not null)
                    {
                        _disChanelRep.Remove(distributionChannel);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(distributionChannelDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(distributionChannelDelete);
                }

            }
            return response;
        }
    }
}
