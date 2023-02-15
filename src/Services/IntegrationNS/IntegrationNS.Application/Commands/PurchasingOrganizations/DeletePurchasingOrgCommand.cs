using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.PurchasingOrganizations
{
    public class DeletePurchasingOrgCommand : IRequest<bool>
    {
        public string PurchasingOrg { get; set; }
    }

    public class DeletePurchasingOrgCommandHandler : IRequestHandler<DeletePurchasingOrgCommand, bool>
    {
        private readonly IRepository<PurchasingOrgModel> _purOrgRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePurchasingOrgCommandHandler(IRepository<PurchasingOrgModel> purOrgRep, IUnitOfWork unitOfWork)
        {
            _purOrgRep = purOrgRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeletePurchasingOrgCommand request, CancellationToken cancellationToken)
        {
            //Xóa Purchasing Org
            var purOrg = await _purOrgRep.FindOneAsync(x => x.PurchasingOrgCode == request.PurchasingOrg);
            if (purOrg is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Purchasing Org {request.PurchasingOrg}");

            _purOrgRep.Remove(purOrg);
            await _unitOfWork.SaveChangesAsync();

            return true;
        } 
    }
}
