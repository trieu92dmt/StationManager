using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class PurchasingOrgIntegrationCommand : IRequest<bool>
    {
        public string PurchasingOrganization { get; set; }
        public string PurchasingOrganizationDescription { get; set; }
    }
    public class PurchasingOrgIntegrationCommandHandler : IRequestHandler<PurchasingOrgIntegrationCommand, bool>
    {
        private readonly IGeneRepo<PurchasingOrgModel> _purOrgRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public PurchasingOrgIntegrationCommandHandler(IGeneRepo<PurchasingOrgModel> purOrgRep, IISDUnitOfWork unitOfWork)
        {
            _purOrgRep = purOrgRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(PurchasingOrgIntegrationCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var purOrg = await _purOrgRep.FindOneAsync(x => x.PurchasingOrgCode == request.PurchasingOrganization);

            if (purOrg is null)
            {
                _purOrgRep.Add(new PurchasingOrgModel
                {
                    PurchasingOrgId = Guid.NewGuid(),
                    PurchasingOrgCode = request.PurchasingOrganization,
                    PurchasingOrgName = request.PurchasingOrganizationDescription,

                    //Common
                    CreateTime = DateTime.Now,
                    Actived = true
                });
            }
            else
            {
                purOrg.PurchasingOrgName = request.PurchasingOrganizationDescription;

                //Common
                purOrg.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
