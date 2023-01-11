using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.PurchasingGroups
{
    public class DeletePurchasingGroupCommand : IRequest<bool>
    {
        public string PurchasingGroup { get; set; }
    }
    public class DeletePurchasingGroupCommandHandler : IRequestHandler<DeletePurchasingGroupCommand, bool>
    {
        private readonly IRepository<PurchasingGroupModel> _purGroupRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePurchasingGroupCommandHandler(IRepository<PurchasingGroupModel> purGroupRep, IUnitOfWork unitOfWork)
        {
            _purGroupRep = purGroupRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePurchasingGroupCommand request, CancellationToken cancellationToken)
        {
            //Xóa Purchasing Group
            var purGroup = await _purGroupRep.FindOneAsync(x => x.PurchasingGroupCode == request.PurchasingGroup);
            if (purGroup is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Purchasing Group {request.PurchasingGroup}");

            _purGroupRep.Remove(purGroup);
            await _unitOfWork.SaveChangesAsync();

            return true;
        } 
    }
}
