using ISD.API.Core.Exceptions;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using ISD.API.Resources;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.Commands.Catalog
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
        private readonly IISDUnitOfWork _unitOfWork;

        public CatalogTypeCreateCommandHandler(IRepository<CatalogTypeModel> cataTypeRepo, IISDUnitOfWork unitOfWork)
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
