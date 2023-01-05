using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands
{
    public class SaleOrgIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<SaleOrgIntegration> SaleOrgs { get; set; } = new List<SaleOrgIntegration>();
    }

    public class SaleOrgIntegration
    {
        public string SalesOrganization { get; set; }
        public string SalesOrganizationName { get; set; }
    }

    public class SaleOrgIntegrationCommandHandler : IRequestHandler<SaleOrgIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<SaleOrgModel> _saleOrgRep;
        private readonly IUnitOfWork _unitOfWork;

        public SaleOrgIntegrationCommandHandler(IRepository<SaleOrgModel> saleOrgRep, IUnitOfWork unitOfWork)
        {
            _saleOrgRep = saleOrgRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(SaleOrgIntegrationCommand request, CancellationToken cancellationToken)
        {

            var response = new IntegrationNSResponse();

            if (!request.SaleOrgs.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.SaleOrgs.Count();

            foreach (var saleOrgIntegration in request.SaleOrgs)
            {
                try
                {
                    //Check tồn tại
                    var saleOrg = await _saleOrgRep.FindOneAsync(x => x.SaleOrgCode == saleOrgIntegration.SalesOrganization);

                    if (saleOrg is null)
                    {
                        _saleOrgRep.Add(new SaleOrgModel
                        {
                            SaleOrgId = Guid.NewGuid(),
                            SaleOrgCode = saleOrgIntegration.SalesOrganization,
                            SaleOrgName = saleOrgIntegration.SalesOrganizationName,

                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        saleOrg.SaleOrgName = saleOrgIntegration.SalesOrganizationName;

                        //Common
                        saleOrg.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add($"{saleOrgIntegration.SalesOrganization}");
                }
            }

            return response;

        }
    }
}
