using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class DistributionChannelIntegrationCommand : IRequest<bool>
    {
        public string DistributionChannel { get; set; }
        public string Name { get; set; }
    }
    public class DistributionChannelIntegrationCommandHandler : IRequestHandler<DistributionChannelIntegrationCommand, bool>
    {
        private readonly IRepository<DistributionChannelModel> _disChanelRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public DistributionChannelIntegrationCommandHandler(IRepository<DistributionChannelModel> disChanelRep, IISDUnitOfWork unitOfWork)
        {
            _disChanelRep = disChanelRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DistributionChannelIntegrationCommand request, CancellationToken cancellationToken)
        {

            //Check tồn tại
            var distributionChannel = await _disChanelRep.FindOneAsync(x => x.DistributionChannelCode == request.DistributionChannel);

            if (distributionChannel is null)
            {
                _disChanelRep.Add(new DistributionChannelModel
                {
                    DistributionChannelId = Guid.NewGuid(),
                    DistributionChannelCode = request.DistributionChannel,
                    DistributionChannelName = request.Name,

                    //Common
                    CreateTime = DateTime.Now,
                    Actived = true
                });
            }
            else
            {
                distributionChannel.DistributionChannelName = request.Name;

                //Common
                distributionChannel.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
