using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class CustmdSaleIntegrationCommand : IRequest<bool>
    {
        public string Division { get; set; }
        public string SalesOrganization { get; set; }
        public string DistributionChannel { get; set; }
        public string CustomerGroup { get; set; }
        public string SalesGroup { get; set; }
        public string SaleOrgCode { get; set; }
        public string SalesOffice { get; set; }
    }
    public class CustmdSaleIntegrationCommandHandler : IRequestHandler<CustmdSaleIntegrationCommand, bool>
    {
        private readonly IRepository<CustmdSaleModel> _custmdSaleRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public CustmdSaleIntegrationCommandHandler(IRepository<CustmdSaleModel> custmdSaleRep, IISDUnitOfWork unitOfWork)
        {
            _custmdSaleRep = custmdSaleRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(CustmdSaleIntegrationCommand request, CancellationToken cancellationToken)
        {
            _custmdSaleRep.Add(new CustmdSaleModel
            {
                CustmdSaleId = Guid.NewGuid(),
                DivisionCode = request.Division,
                SaleOrgCode = request.SalesOrganization,
                DistributionChannelCode = request.DistributionChannel,
                CustomerGroup = request.CustomerGroup,
                SalesGroup = request.SalesGroup,
                SalesOffice = request.SalesOffice,

                //Common
                CreateTime = DateTime.Now,
                Actived = true
            });

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
