using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.DistributionChannels
{
    public class DeleteDistributionChannelCommand : IRequest<bool>
    {
        public string DistributionChannel { get; set; }
    }

    public class DeleteDistributionChannelCommandHandler : IRequestHandler<DeleteDistributionChannelCommand, bool>
    {
        private readonly IRepository<DistributionChannelModel> _disChanelRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDistributionChannelCommandHandler(IRepository<DistributionChannelModel> disChanelRep, IUnitOfWork unitOfWork)
        {
            _disChanelRep = disChanelRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteDistributionChannelCommand request, CancellationToken cancellationToken)
        {
            //Xóa Distribution Channel
            var distributionChannel = await _disChanelRep.FindOneAsync(x => x.DistributionChannelCode == request.DistributionChannel);
            if (distributionChannel is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Customer {request.DistributionChannel}");

            _disChanelRep.Remove(distributionChannel);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
