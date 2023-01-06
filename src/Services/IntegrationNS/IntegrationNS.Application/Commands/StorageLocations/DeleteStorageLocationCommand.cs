using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.StorageLocations
{
    public class DeleteStorageLocationCommand : IRequest<DeleteNSResponse>
    {
        public List<string> StorageLocations { get; set; } = new List<string>();
    }

    public class DeleteStorageLocationCommandHandler : IRequestHandler<DeleteStorageLocationCommand, DeleteNSResponse>
    {
        private readonly IRepository<StorageLocationModel> _storageLocationRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteStorageLocationCommandHandler(IRepository<StorageLocationModel> storageLocationRep, IUnitOfWork unitOfWork)
        {
            _storageLocationRep = storageLocationRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteNSResponse> Handle(DeleteStorageLocationCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.StorageLocations.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.StorageLocations.Count();

            foreach (var storageLocationDelete in request.StorageLocations)
            {
                try
                {
                    //Xóa Storage Location
                    var storageLocation = await _storageLocationRep.FindOneAsync(x => x.StorageLocationName == storageLocationDelete);
                    if (storageLocation is not null)
                    {
                        _storageLocationRep.Remove(storageLocation);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(storageLocationDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(storageLocationDelete);
                }

            }
            return response;
        }
    }
}
