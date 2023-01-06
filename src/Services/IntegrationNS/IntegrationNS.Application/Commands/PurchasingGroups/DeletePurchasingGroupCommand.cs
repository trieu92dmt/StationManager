using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.PurchasingGroups
{
    public class DeletePurchasingGroupCommand : IRequest<DeleteNSResponse>
    {
        public List<string> PurchasingGroups { get; set; } = new List<string>();
    }
    public class DeletePurchasingGroupCommandHandler : IRequestHandler<DeletePurchasingGroupCommand, DeleteNSResponse>
    {
        private readonly IRepository<PurchasingGroupModel> _purGroupRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePurchasingGroupCommandHandler(IRepository<PurchasingGroupModel> purGroupRep, IUnitOfWork unitOfWork)
        {
            _purGroupRep = purGroupRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteNSResponse> Handle(DeletePurchasingGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.PurchasingGroups.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.PurchasingGroups.Count();

            foreach (var purGroupDelete in request.PurchasingGroups)
            {
                try
                {
                    //Xóa Purchasing Group
                    var purGroup = await _purGroupRep.FindOneAsync(x => x.PurchasingGroupCode == purGroupDelete);
                    if (purGroup is not null)
                    {
                        _purGroupRep.Remove(purGroup);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(purGroupDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(purGroupDelete);
                }

            }
            return response;
        }
    }
}
