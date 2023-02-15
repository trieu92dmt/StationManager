using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.SalesDocument
{
    public class DeleteSalesDocumentNSCommand : IRequest<bool>
    {
        public string SalesDocumentCode { get; set; }
    }
    public class DeleteSalesDocumentNSCommandHandler : IRequestHandler<DeleteSalesDocumentNSCommand, bool>
    {
        private readonly IRepository<SalesDocumentModel> _saleRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DetailSalesDocumentModel> _saleDetailRepo;

        public DeleteSalesDocumentNSCommandHandler(IRepository<SalesDocumentModel> saleRep, IUnitOfWork unitOfWork, IRepository<DetailSalesDocumentModel> saleDetailRepo)
        {
            _saleRep = saleRep;
            _unitOfWork = unitOfWork;
            _saleDetailRepo = saleDetailRepo;
        }
        public async Task<bool> Handle(DeleteSalesDocumentNSCommand request, CancellationToken cancellationToken)
        {
            //Sale Document
            var saleDoc = await _saleRep.GetQuery(x => x.SalesDocumentCode == request.SalesDocumentCode)
                                      .Include(x => x.DetailSalesDocumentModel)
                                      .FirstOrDefaultAsync();
            if (saleDoc is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"Sales Document {request.SalesDocumentCode}");

            //Xóa data sales document detail
            _saleDetailRepo.RemoveRange(saleDoc.DetailSalesDocumentModel);

            //Xóa sales document
            _saleRep.Remove(saleDoc);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
