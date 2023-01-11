using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.StorageLocations
{
    public class DeleteStorageLocationCommand : IRequest<bool>
    {
        public string StorageLocation { get; set; }
    }

    public class DeleteStorageLocationCommandHandler : IRequestHandler<DeleteStorageLocationCommand, bool>
    {
        private readonly IRepository<StorageLocationModel> _storageLocationRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteStorageLocationCommandHandler(IRepository<StorageLocationModel> storageLocationRep, IUnitOfWork unitOfWork)
        {
            _storageLocationRep = storageLocationRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteStorageLocationCommand request, CancellationToken cancellationToken)
        {
            //Xóa Storage Location
            var storageLocation = await _storageLocationRep.FindOneAsync(x => x.StorageLocationName == request.StorageLocation);
            if (storageLocation is not null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Storage Location {request.StorageLocation}");


            _storageLocationRep.Remove(storageLocation);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
