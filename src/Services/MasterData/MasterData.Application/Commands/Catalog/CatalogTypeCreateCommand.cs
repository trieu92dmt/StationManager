using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace MasterData.Applications.Commands.Catalog
{
    public class CatalogTypeCreateCommand : IRequest<bool>
    {
        //Loai danh muc
        public string CatalogTypeCode { get; set; }
        //Ten loai danh muc
        public string CatalogTypeName { get; set; }
    }

    public class CatalogTypeCreateCommandHandler : IRequestHandler<CatalogTypeCreateCommand, bool>
    {
        private readonly IRepository<CatalogTypeModel> _cataTypeRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CatalogTypeCreateCommandHandler(IRepository<CatalogTypeModel> cataTypeRepo, IUnitOfWork unitOfWork)
        {
            _cataTypeRepo = cataTypeRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CatalogTypeCreateCommand request, CancellationToken cancellationToken)
        {
            //Kiem tra da ton tai
            var catalogType = await _cataTypeRepo.FindOneAsync(x => x.CatalogTypeCode == request.CatalogTypeCode);
            if (catalogType != null)
            {
                throw new ISDException(CommonResource.Msg_Existed, "Loai danh muc");
            }

            _cataTypeRepo.Add(new CatalogTypeModel()
            {
                CatalogTypeCode = request.CatalogTypeCode,
                CatalogTypeName = request.CatalogTypeName,
                Actived = true
            });

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
