using Azure.Core;
using ISD.API.Constant.Common;
using ISD.API.EntityModels.Models;
using ISD.API.Extensions.Jwt;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.Applications.Commands.MES
{
    public class SaveNKMHCommand : IRequest<bool>
    {
        public List<NKMHRequest> NKMHRequests { get; set; } = new List<NKMHRequest>();
    }

    public class NKMHRequest
    {
        public Guid PoDetailId { get; set; }
        public string WeightHeadCode { get; set; }
    }

    public class SaveNKMHCommandHandler : IRequestHandler<SaveNKMHCommand, bool>
    {
        private readonly IRepository<GoodsReceiptModel> _nkRep;
        private readonly IISDUnitOfWork _unitOfWork;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRep;

        public SaveNKMHCommandHandler(IRepository<GoodsReceiptModel> nkRep, IISDUnitOfWork unitOfWork, 
                                      IRepository<PurchaseOrderDetailModel> poDetailRep)
        {
            _nkRep = nkRep;
            _unitOfWork = unitOfWork;
            _poDetailRep = poDetailRep;
        }
        public async Task<bool> Handle(SaveNKMHCommand request, CancellationToken cancellationToken)
        {

            foreach (var x in request.NKMHRequests)
            {

                var poLine = await _poDetailRep.GetQuery(p => p.PurchaseOrderDetailId == x.PoDetailId)
                                               .Include(x => x.PurchaseOrder)
                                               .FirstOrDefaultAsync();

                //Save data nhập kho mua hàng
                _nkRep.Add(new GoodsReceiptModel
                {
                    GoodsReceiptId = Guid.NewGuid(),
                    //POLine
                    PurchaseOrderDetailId = poLine?.PurchaseOrderDetailId,
                    //Mã đầu cân
                    WeightHeadCode = x.WeightHeadCode,
                    //Số phương tiện
                    VehicleCode = poLine.VehicleCode,

                    DocumentDate = DateTime.Now,

                    //Common
                    DateKey = int.Parse(DateTime.Now.ToString(ISDDateTimeFormat.DateKey)),

                    CreateTime = DateTime.Now,
                    CreateBy = TokenExtensions.GetAccountId(),
                    Actived = true

                });
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
