using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class PurchasingGroupIntegrationCommand : IRequest<bool>
    {
        public string PurchasingGroup { get; set; }
        public string PurchasingGroupDescription { get; set; }

    }
    public class PurchasingGroupIntegrationCommandHandler : IRequestHandler<PurchasingGroupIntegrationCommand, bool>
    {
        private readonly IRepository<PurchasingGroupModel> _purGroupRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public PurchasingGroupIntegrationCommandHandler(IRepository<PurchasingGroupModel> purGroupRep, IISDUnitOfWork unitOfWork)
        {
            _purGroupRep = purGroupRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(PurchasingGroupIntegrationCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại
            var purGroup = await _purGroupRep.FindOneAsync(x => x.PurchasingGroupCode == request.PurchasingGroup);

            if (purGroup is null)
            {
                _purGroupRep.Add(new PurchasingGroupModel
                {
                    PurchasingGroupId = Guid.NewGuid(),
                    PurchasingGroupCode = request.PurchasingGroup,
                    PurchasingGroupName = request.PurchasingGroupDescription,

                    //Common
                    CreateTime = DateTime.Now,
                    Actived = true
                });
            }
            else
            {
                purGroup.PurchasingGroupName = request.PurchasingGroupDescription;

                //Common
                purGroup.LastEditTime = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
