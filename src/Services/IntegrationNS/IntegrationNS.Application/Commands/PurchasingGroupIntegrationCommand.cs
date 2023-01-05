using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands
{
    public class PurchasingGroupIntegrationCommand : IRequest<IntegrationNSResponse>
    {
        public List<PurchasingGroupIntegration> PurchasingGroups { get; set; } = new List<PurchasingGroupIntegration>();
    }
    public class PurchasingGroupIntegration
    {
        public string PurchasingGroup { get; set; }
        public string PurchasingGroupDescription { get; set; }
    }

    public class PurchasingGroupIntegrationCommandHandler : IRequestHandler<PurchasingGroupIntegrationCommand, IntegrationNSResponse>
    {
        private readonly IRepository<PurchasingGroupModel> _purGroupRep;
        private readonly IUnitOfWork _unitOfWork;

        public PurchasingGroupIntegrationCommandHandler(IRepository<PurchasingGroupModel> purGroupRep, IUnitOfWork unitOfWork)
        {
            _purGroupRep = purGroupRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntegrationNSResponse> Handle(PurchasingGroupIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new IntegrationNSResponse();

            if (!request.PurchasingGroups.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu đồng bộ");

            response.TotalRecord = request.PurchasingGroups.Count();

            foreach (var purGroupIntegration in request.PurchasingGroups)
            {
                try
                {
                    //Check tồn tại
                    var purGroup = await _purGroupRep.FindOneAsync(x => x.PurchasingGroupCode == purGroupIntegration.PurchasingGroup);

                    if (purGroup is null)
                    {
                        _purGroupRep.Add(new PurchasingGroupModel
                        {
                            PurchasingGroupId = Guid.NewGuid(),
                            PurchasingGroupCode = purGroupIntegration.PurchasingGroup,
                            PurchasingGroupName = purGroupIntegration.PurchasingGroupDescription,

                            //Common
                            CreateTime = DateTime.Now,
                            Actived = true
                        });
                    }
                    else
                    {
                        purGroup.PurchasingGroupName = purGroupIntegration.PurchasingGroupDescription;

                        //Common
                        purGroup.LastEditTime = DateTime.Now;
                    }

                    await _unitOfWork.SaveChangesAsync();

                    response.RecordSyncSuccess++;
                }
                catch (Exception)
                {
                    response.RecordSyncFailed++;
                    response.ListRecordSyncFailed.Add($"{purGroupIntegration.PurchasingGroup}");
                }
            }

            return response;
        }
    }
}
