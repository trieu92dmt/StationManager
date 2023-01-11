using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.SalesOrgs
{
    public class DeleteSaleOrgCommand : IRequest<bool>
    {
        public string SaleOrg { get; set; } 
    }

    public class DeleteSaleOrgCommandHandler : IRequestHandler<DeleteSaleOrgCommand, bool>
    {
        private readonly IRepository<SaleOrgModel> _saleOrgRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSaleOrgCommandHandler(IRepository<SaleOrgModel> saleOrgRep, IUnitOfWork unitOfWork)
        {
            _saleOrgRep = saleOrgRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteSaleOrgCommand request, CancellationToken cancellationToken)
        {
            //Xóa Sale Org
            var saleOrg = await _saleOrgRep.FindOneAsync(x => x.SaleOrgCode == request.SaleOrg);
            if (saleOrg is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"SaleOrg {request.SaleOrg}");


            _saleOrgRep.Remove(saleOrg);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
