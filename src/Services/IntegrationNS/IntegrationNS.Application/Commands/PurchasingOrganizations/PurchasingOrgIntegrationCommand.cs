using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.PurchasingOrganizations
{
    public class PurchasingOrgIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<PurchasingOrgIntegration> PurchasingOrgs { get; set; } = new List<PurchasingOrgIntegration>();
    }
    public class PurchasingOrgIntegration
    {
        public string PurchasingOrganization { get; set; }
        public string PurchasingOrganizationDescription { get; set; }
    }

    public class PurchasingOrgIntegrationCommandHandler : IRequestHandler<PurchasingOrgIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<PurchasingOrgModel> _purOrgRep;
        private readonly IUnitOfWork _unitOfWork;

        public PurchasingOrgIntegrationCommandHandler(IRepository<PurchasingOrgModel> purOrgRep, IUnitOfWork unitOfWork)
        {
            _purOrgRep = purOrgRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(PurchasingOrgIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.PurchasingOrgs.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.PurchasingOrgs.Count();

            foreach (var purOrgIntegration in request.PurchasingOrgs)
            {
                try
                {
                    //Check tồn tại
                    var purOrg = await _purOrgRep.FindOneAsync(x => x.PurchasingOrgCode == purOrgIntegration.PurchasingOrganization);

                    if (purOrg is null)
                    {
                        _purOrgRep.Add(new PurchasingOrgModel
                        {
                            PurchasingOrgId = Guid.NewGuid(),
                            //PurchasingOrgCodeInt = int.Parse(request.PurchasingOrganization),
                            PurchasingOrgCode = purOrgIntegration.PurchasingOrganization,
                            PurchasingOrgName = purOrgIntegration.PurchasingOrganizationDescription,

                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        purOrg.PurchasingOrgName = purOrgIntegration.PurchasingOrganizationDescription;

                        //Common
                        purOrg.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add($"{purOrgIntegration.PurchasingOrganization}");
                }
            }

            return response;
        }
    }
}
