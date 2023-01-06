using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.CustmdSales
{
    public class CustmdSaleIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<CustmdSaleIntegration> CustmdSales { get; set; } = new List<CustmdSaleIntegration>();
    }

    public class CustmdSaleIntegration
    {
        public string Division { get; set; }
        public string SalesOrganization { get; set; }
        public string DistributionChannel { get; set; }
        public string CustomerGroup { get; set; }
        public string SalesGroup { get; set; }
        public string SaleOrgCode { get; set; }
        public string SalesOffice { get; set; }
    }

    public class CustmdSaleIntegrationCommandHandler : IRequestHandler<CustmdSaleIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<CustmdSaleModel> _custmdSaleRep;
        private readonly IUnitOfWork _unitOfWork;

        public CustmdSaleIntegrationCommandHandler(IRepository<CustmdSaleModel> custmdSaleRep, IUnitOfWork unitOfWork)
        {
            _custmdSaleRep = custmdSaleRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(CustmdSaleIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.CustmdSales.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.CustmdSales.Count();

            foreach (var custmdSale in request.CustmdSales)
            {
                try
                {
                    _custmdSaleRep.Add(new CustmdSaleModel
                    {
                        CustmdSaleId = Guid.NewGuid(),
                        DivisionCode = custmdSale.Division,
                        SaleOrgCode = custmdSale.SalesOrganization,
                        DistributionChannelCode = custmdSale.DistributionChannel,
                        CustomerGroup = custmdSale.CustomerGroup,
                        SalesGroup = custmdSale.SalesGroup,
                        SalesOffice = custmdSale.SalesOffice,

                        //Common
                        CreateTime = DateTime.Now,
                        Actived = true
                    });

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add($"{custmdSale.SalesOffice}");
                }
            }

            return response;
        }
    }
}
