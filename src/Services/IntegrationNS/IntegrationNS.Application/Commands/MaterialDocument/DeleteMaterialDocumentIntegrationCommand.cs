using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.MaterialDocument
{

    public class DeleteMaterialDocumentIntegrationCommand : IRequest<bool>
    {
        public string MaterialDocCode { get; set; }
    }

    public class DeleteMaterialDocumentIntegrationCommandHandler : IRequestHandler<DeleteMaterialDocumentIntegrationCommand, bool>
    {
        private readonly IRepository<MaterialDocumentModel> _mateRep;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteMaterialDocumentIntegrationCommandHandler(IRepository<MaterialDocumentModel> mateRep, IUnitOfWork unitOfWork)
        {
            _mateRep = mateRep;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteMaterialDocumentIntegrationCommand request, CancellationToken cancellationToken)
        {
            //Xóa Material doc
            var mateDoc = await _mateRep.FindOneAsync(x => x.MaterialDocCode == request.MaterialDocCode);
            if (mateDoc is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Material Document {request.MaterialDocCode}");

            _mateRep.Remove(mateDoc);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
