using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.PurchaseOrders
{
    public class DeletePurchaseOrderCommand : IRequest<bool>
    {
        public string PurchaseOrder { get; set; }
    }
    public class DeletePurchaseOrderCommandHandler : IRequestHandler<DeletePurchaseOrderCommand, bool>
    {
        private readonly IRepository<PurchaseOrderMasterModel> _poRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRep;
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;

        public DeletePurchaseOrderCommandHandler(IRepository<PurchaseOrderMasterModel> poRep, IUnitOfWork unitOfWork, IRepository<PurchaseOrderDetailModel> poDetailRep,
                                                 IRepository<GoodsReceiptModel> nkmhRep)
        {
            _poRep = poRep;
            _unitOfWork = unitOfWork;
            _poDetailRep = poDetailRep;
            _nkmhRep = nkmhRep;
        }

        public async Task<bool> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            //PO
            var po = await _poRep.GetQuery(x => x.PurchaseOrderCode == request.PurchaseOrder)
                                      .Include(x => x.PurchaseOrderDetailModel).ThenInclude(x => x.GoodsReceiptModel)
                                      .FirstOrDefaultAsync();
            if (po is null)
                throw new ISDException(CommonResource.Msg_NotFound, $"PO {request.PurchaseOrder}");

            //Xóa data nhập kho mua hàng
            foreach (var poDetail in po.PurchaseOrderDetailModel)
            {
                _nkmhRep.RemoveRange(poDetail.GoodsReceiptModel);
            }

            //Xóa PO Detail
            _poDetailRep.RemoveRange(po.PurchaseOrderDetailModel);

            //Xóa PO
            _poRep.Remove(po);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
