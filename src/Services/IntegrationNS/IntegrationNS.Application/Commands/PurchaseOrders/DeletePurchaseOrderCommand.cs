using IntegrationNS.Application.DTOs;
using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.PurchaseOrders
{
    public class DeletePurchaseOrderCommand : IRequest<DeleteNSResponse>
    {
        public List<string> PurchaseOrders { get; set; } = new List<string>();
    }
    public class DeletePurchaseOrderCommandHandler : IRequestHandler<DeletePurchaseOrderCommand, DeleteNSResponse>
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

        public async Task<DeleteNSResponse> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteNSResponse();

            if (!request.PurchaseOrders.Any())
                throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xóa");

            response.TotalRecord = request.PurchaseOrders.Count();

            foreach (var poDelete in request.PurchaseOrders)
            {
                try
                {
                    //Xóa Disivision
                    var po = await _poRep.GetQuery(x => x.PurchaseOrderCode == poDelete)
                                              .Include(x => x.PurchaseOrderDetailModel).ThenInclude(x => x.GoodsReceiptModel)
                                              .FirstOrDefaultAsync();
                    if (po is not null)
                    {
                        foreach (var poDetail in po.PurchaseOrderDetailModel)
                        {
                            _nkmhRep.RemoveRange(poDetail.GoodsReceiptModel);
                        }

                        _poDetailRep.RemoveRange(po.PurchaseOrderDetailModel);

                        _poRep.Remove(po);
                        await _unitOfWork.SaveChangesAsync();

                        //Xóa thành công
                        response.RecordDeleteSuccess++;
                    }
                    else
                    {
                        //Xóa thất bại
                        response.RecordDeleteFail++;
                        response.ListRecordDeleteFailed.Add(poDelete);
                    }
                }
                catch (Exception)
                {
                    //Xóa thất bại
                    response.RecordDeleteFail++;
                    response.ListRecordDeleteFailed.Add(poDelete);
                }

            }
            return response;
        }
    }
}
