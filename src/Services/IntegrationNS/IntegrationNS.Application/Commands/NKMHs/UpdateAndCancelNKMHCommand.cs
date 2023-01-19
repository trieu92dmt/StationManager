using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;

namespace IntegrationNS.Application.Commands.NKMHs
{
    public class UpdateAndCancelNKMHCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelNKMH> NKMHs { get; set; } = new List<UpdateAndCancelNKMH>();
    }

    public class UpdateAndCancelNKMH
    {
        public Guid NkmhId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelNKMHCommandHandler : IRequestHandler<UpdateAndCancelNKMHCommand, bool>
    {
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAndCancelNKMHCommandHandler(IRepository<GoodsReceiptModel> nkmhRep, IUnitOfWork unitOfWork)
        {
            _nkmhRep = nkmhRep;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelNKMHCommand request, CancellationToken cancellationToken)
        {
            if (request.IsCancel == true)
            {

                if (!request.NKMHs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NKMH");

                foreach (var item in request.NKMHs)
                {
                    //Phiếu nhập kho mua hàng
                    var nkmh = await _nkmhRep.FindOneAsync(x => x.GoodsReceiptId == item.NkmhId);

                    //Check
                    if (nkmh is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho mua hàng");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    nkmh.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(nkmh.MaterialDocument) && string.IsNullOrEmpty(nkmh.ReverseDocument))
                        nkmh.Status = "POST";
                    else if (!string.IsNullOrEmpty(nkmh.ReverseDocument))
                        nkmh.Status = "NOT";
                    nkmh.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    var nkmhNew = new GoodsReceiptModel
                    {
                        GoodsReceiptId = Guid.NewGuid(),
                        PlantCode = nkmh.PlantCode,
                        PurchaseOrderDetailId = nkmh.PurchaseOrderDetailId,
                        WeightId = nkmh.WeightId,
                        WeitghtVote = nkmh.WeitghtVote,
                        BagQuantity = nkmh.BagQuantity,
                        SingleWeight = nkmh.SingleWeight,
                        WeightHeadCode = nkmh.WeightHeadCode,
                        Weight = nkmh.Weight,
                        ConfirmQty = nkmh.ConfirmQty,
                        QuantityWithPackaging = nkmh.QuantityWithPackaging,
                        VehicleCode = nkmh.VehicleCode,
                        Batch = nkmh.Batch,
                        QuantityWeitght = nkmh.QuantityWeitght,
                        TruckQuantity = nkmh.TruckQuantity,
                        InputWeight = nkmh.InputWeight,
                        OutputWeight = nkmh.OutputWeight,
                        Description = nkmh.Description,
                        Image = nkmh.Image,
                        Status = nkmh.Status,
                        StartTime = nkmh.StartTime,
                        EndTime = nkmh.EndTime,
                        DocumentDate = nkmh.DocumentDate,
                        DateKey = nkmh.DateKey,
                        CreateTime = DateTime.Now,
                        Actived = true
                    };

                    _nkmhRep.Add(nkmhNew);
                }
            }
            else
            {

                if (!request.NKMHs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NKMH");

                foreach (var item in request.NKMHs)
                {
                    //Phiếu nhập kho mua hàng
                    var nkmh = await _nkmhRep.FindOneAsync(x => x.GoodsReceiptId == item.NkmhId);

                    //Check
                    if (nkmh is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho mua hàng");

                    //Cập nhật Batch và MaterialDocument
                    nkmh.Batch = item.Batch;
                    nkmh.MaterialDocument = item.MaterialDocument;
                    if (!string.IsNullOrEmpty(nkmh.MaterialDocument) && string.IsNullOrEmpty(nkmh.ReverseDocument))
                        nkmh.Status = "POST";
                    else if (!string.IsNullOrEmpty(nkmh.ReverseDocument))
                        nkmh.Status = "NOT";
                }             
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
