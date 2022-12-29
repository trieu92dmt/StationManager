﻿using ISD.API.Core.Exceptions;
using ISD.API.EntityModels.Models;
using ISD.API.Repositories.Infrastructure.Database;
using ISD.API.Repositories.Infrastructure.Repositories;
using ISD.API.Resources;
using MediatR;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace ISD.API.Applications.Commands.IntegrationNS
{
    public class UpdateAndCancelNKMHCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public Guid NkmhId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }

    public class UpdateAndCancelNKMHCommandHandler : IRequestHandler<UpdateAndCancelNKMHCommand, bool>
    {
        private readonly IRepository<GoodsReceiptModel> _nkmhRep;
        private readonly IISDUnitOfWork _unitOfWork;

        public UpdateAndCancelNKMHCommandHandler(IRepository<GoodsReceiptModel> nkmhRep, IISDUnitOfWork unitOfWork)
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
                //Phiếu nhập kho mua hàng
                var nkmh = await _nkmhRep.FindOneAsync(x => x.GoodsReceiptId == request.NkmhId);

                //Check
                if (nkmh is null)
                    throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho mua hàng");

                //Cập nhật Batch và MaterialDocument và ReverseDocument
                nkmh.Batch = request.Batch;
                nkmh.MaterialDocument = request.MaterialDocument;
                nkmh.ReverseDocument = request.ReverseDocument;
                nkmh.LastEditTime = DateTime.Now;

                //Tạo line mới
                var nkmhNew = new GoodsReceiptModel
                {
                    GoodsReceiptId = Guid.NewGuid(),
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
                    QuantityWeitght = nkmh.QuantityWeitght,
                    TotalQuantity = nkmh.TotalQuantity,
                    DeliveredQuantity = nkmh.DeliveredQuantity,
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
                    Batch = nkmh.Batch,
                    MaterialDocument = nkmh.MaterialDocument,
                    CreateTime = DateTime.Now,
                    Actived = true
                };
            }
            else
            {
                //Phiếu nhập kho mua hàng
                var nkmh = await _nkmhRep.FindOneAsync(x => x.GoodsReceiptId == request.NkmhId);

                //Check
                if (nkmh is null)
                    throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho mua hàng");

                //Cập nhật Batch và MaterialDocument
                nkmh.Batch = request.Batch;
                nkmh.MaterialDocument = request.MaterialDocument;

            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}