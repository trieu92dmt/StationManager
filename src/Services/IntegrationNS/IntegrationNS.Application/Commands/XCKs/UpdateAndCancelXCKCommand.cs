using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.XCKs
{
    public class UpdateAndCancelXCKCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelXCK> XCKs { get; set; } = new List<UpdateAndCancelXCK>();
    }

    public class UpdateAndCancelXCK
    {
        public Guid XckId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelXCKCommandHandler : IRequestHandler<UpdateAndCancelXCKCommand, bool>
    {
        private readonly IRepository<WarehouseExportTransferModel> _xckRep;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAndCancelXCKCommandHandler(IRepository<WarehouseExportTransferModel> xckRep, IUnitOfWork unitOfWork)
        {
            _xckRep = xckRep;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelXCKCommand request, CancellationToken cancellationToken)
        {
            if (request.IsCancel == true)
            {

                if (!request.XCKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất chuyển kho");

                foreach (var item in request.XCKs)
                {
                    //Phiếu xuất chuyển kho
                    var xck = await _xckRep.FindOneAsync(x => x.WarehouseTransferId == item.XckId);

                    //Check
                    if (xck is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất chuyển kho");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    xck.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(xck.MaterialDocument))// && string.IsNullOrEmpty(xck.ReverseDocument))
                        xck.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xck.ReverseDocument))
                    //    xck.Status = "NOT";
                    xck.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(xck);
                    var xckNew = JsonConvert.DeserializeObject<WarehouseExportTransferModel>(serialized);

                    xckNew.WarehouseTransferId = Guid.NewGuid();
                    xckNew.Status = "NOT";
                    xckNew.TotalQuantity = 0;
                    xckNew.DeliveredQuantity = 0;
                    xckNew.OpenQuantity = 0;
                    xckNew.MaterialDocument = null;
                    xckNew.ReverseDocument = null;

                    #region Code cũ
                    //var xckNew = new WarehouseExportTransferModel
                    //{
                    //    WarehouseTransferId = Guid.NewGuid(),
                    //    PlantCode = xck.PlantCode,
                    //    PlantName = xck.PlantName,
                    //    DetailReservationId = xck.DetailReservationId,
                    //    WeightId = xck.WeightId,
                    //    MaterialName = xck.MaterialName,
                    //    MaterialCodeInt = xck.MaterialCodeInt,
                    //    WeightVote = xck.WeightVote,
                    //    BagQuantity = xck.BagQuantity,
                    //    SingleWeight = xck.SingleWeight,
                    //    WeightHeadCode = xck.WeightHeadCode,
                    //    Weight = xck.Weight,
                    //    ConfirmQty = xck.ConfirmQty,
                    //    QuantityWithPackaging = xck.QuantityWithPackaging,
                    //    VehicleCode = xck.VehicleCode,
                    //    QuantityWeitght = xck.QuantityWeitght,
                    //    TruckInfoId = xck.TruckInfoId,
                    //    TruckNumber = xck.TruckNumber,
                    //    InputWeight = xck.InputWeight,
                    //    OutputWeight = xck.OutputWeight,
                    //    Description = xck.Description,
                    //    MaterialCode = xck.MaterialCode,
                    //    SlocCode = xck.SlocCode,
                    //    SlocName = xck.SlocName,
                    //    ReceivingSlocCode = xck.ReceivingSlocCode,
                    //    ReceivingSlocName = xck.ReceivingSlocName,
                    //    MovementType = xck.MovementType,
                    //    StockType = xck.StockType,
                    //    Customer = xck.Customer,
                    //    Unit = xck.Unit,
                    //    Image = xck.Image,
                    //    Status = xck.Status,
                    //    StartTime = xck.StartTime,
                    //    EndTime = xck.EndTime,
                    //    CreateTime = DateTime.Now,
                    //    Actived = true
                    //};
                    #endregion

                    _xckRep.Add(xckNew);
                }
            }
            else
            {

                if (!request.XCKs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu XCK");

                foreach (var item in request.XCKs)
                {
                    //Phiếu xuất chuyển kho
                    var xck = await _xckRep.GetQuery().Include(x => x.DetailReservation).FirstOrDefaultAsync(x => x.WarehouseTransferId == item.XckId);

                    //Check
                    if (xck is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất chuyển kho");

                    //Cập nhật Batch và MaterialDocument
                    xck.Batch = item.Batch;
                    xck.MaterialDocument = item.MaterialDocument;
                    xck.TotalQuantity = xck.DetailReservationId.HasValue ? xck.DetailReservation.RequirementQty : 0;
                    xck.DeliveredQuantity = xck.DetailReservationId.HasValue ? xck.DetailReservation.QtyWithdrawn : 0;
                    xck.OpenQuantity = xck.TotalQuantity - xck.DeliveredQuantity;
                    if (!string.IsNullOrEmpty(xck.MaterialDocument))// && string.IsNullOrEmpty(xck.ReverseDocument))
                        xck.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xck.ReverseDocument))
                    //    xck.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
