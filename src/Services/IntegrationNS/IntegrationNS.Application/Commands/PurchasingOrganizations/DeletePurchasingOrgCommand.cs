using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.PurchasingOrganizations
{
    public class DeletePurchasingOrgCommand : IRequest<DeleteNSResponse>
    {
        public List<string> PurchasingOrgs { get; set; } = new List<string>();
    }

    public class DeletePurchasingOrgCommandHandler : IRequestHandler<DeletePurchasingOrgCommand, DeleteNSResponse>
    {
        private readonly IRepository<PurchasingOrgModel> _purOrgRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePurchasingOrgCommandHandler(IRepository<PurchasingOrgModel> purOrgRep, IUnitOfWork unitOfWork)
        {
            _purOrgRep = purOrgRep;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteNSResponse> Handle(DeletePurchasingOrgCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.PurchasingOrgs.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.PurchasingOrgs.Count();

            foreach (var purOrgDelete in request.PurchasingOrgs)
            {
                try
                {
                    //Xóa Purchasing Org
                    var purOrg = await _purOrgRep.FindOneAsync(x => x.PurchasingOrgCode == purOrgDelete);
                    if (purOrg is not null)
                    {
                        _purOrgRep.Remove(purOrg);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(purOrgDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(purOrgDelete);
                }

            }
            return response;
        }
    }
}
