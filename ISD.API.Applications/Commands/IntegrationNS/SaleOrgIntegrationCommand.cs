using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class SaleOrgIntegrationCommand : IRequest<bool>
    {
        public string SalesOrganization { get; set; }
        public string SalesOrganizationName { get; set; }
    }

    public class SaleOrgIntegrationCommandHandler : IRequestHandler<SaleOrgIntegrationCommand, bool>
    {
        private readonly IRepository<SaleOrgModel> _saleOrgRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public SaleOrgIntegrationCommandHandler(IRepository<SaleOrgModel> saleOrgRep, IISDUnitOfWork unitOfWork)
        {
            _saleOrgRep = saleOrgRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(SaleOrgIntegrationCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var saleOrg = await _saleOrgRep.FindOneAsync(x => x.SaleOrgCode == request.SalesOrganization);

            if (saleOrg is null)
            {
                _saleOrgRep.Add(new SaleOrgModel
                {
                    SaleOrgId = Guid.NewGuid(),
                    SaleOrgCode = request.SalesOrganization,
                    SaleOrgName = request.SalesOrganizationName,

                    //Common
                    CreateTime = DateTime.Now,
                    Actived = true
                });
            }
            else
            {
                saleOrg.SaleOrgName = request.SalesOrganizationName;

                //Common
                saleOrg.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;

        }
    }
}
