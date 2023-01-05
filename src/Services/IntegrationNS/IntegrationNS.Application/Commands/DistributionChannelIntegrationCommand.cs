using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands
{
    public class DistributionChannelIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<DistributionChannelIntegration> DistributionChannels { get; set; } = new List<DistributionChannelIntegration>();
    }

    public class DistributionChannelIntegration 
    {
        public string DistributionChannel { get; set; }
        public string Name { get; set; }
    }

    public class DistributionChannelIntegrationCommandHandler : IRequestHandler<DistributionChannelIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<DistributionChannelModel> _disChanelRep;
        private readonly IUnitOfWork _unitOfWork;

        public DistributionChannelIntegrationCommandHandler(IRepository<DistributionChannelModel> disChanelRep, IUnitOfWork unitOfWork)
        {
            _disChanelRep = disChanelRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(DistributionChannelIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.DistributionChannels.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.DistributionChannels.Count();

            var disChannels = _disChanelRep.GetQuery();

            foreach (var disChannelIntegration in request.DistributionChannels)
            {
                try
                {
                    //Check tồn tại
                    var distributionChannel = await disChannels.FirstOrDefaultAsync(x => x.DistributionChannelCode == disChannelIntegration.DistributionChannel);

                    if (distributionChannel is null)
                    {
                        _disChanelRep.Add(new DistributionChannelModel
                        {
                            DistributionChannelId = Guid.NewGuid(),
                            DistributionChannelCode = disChannelIntegration.DistributionChannel,
                            DistributionChannelName = disChannelIntegration.Name,

                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        distributionChannel.DistributionChannelName = disChannelIntegration.Name;

                        //Common
                        distributionChannel.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add($"{disChannelIntegration.DistributionChannel} | {disChannelIntegration.Name}");
                }
            }

            return response;
        }
    }
}
