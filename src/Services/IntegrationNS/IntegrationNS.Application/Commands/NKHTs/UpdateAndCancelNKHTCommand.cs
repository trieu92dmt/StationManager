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

namespace IntegrationNS.Application.Commands.NKHTs
{
    public class UpdateAndCancelNKHTCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelNKHT> NKHTs { get; set; } = new List<UpdateAndCancelNKHT>();
    }

    public class UpdateAndCancelNKHT
    {
        public Guid NkhtId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelNKHTCommandHandler : IRequestHandler<UpdateAndCancelNKHTCommand, bool>
    {
        private readonly IRepository<GoodsReturnModel> _nkhtRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DetailOutboundDeliveryModel> _obDetailRepo;

        public UpdateAndCancelNKHTCommandHandler(IRepository<GoodsReturnModel> nkhtRep, IUnitOfWork unitOfWork,
                                                 IRepository<DetailOutboundDeliveryModel> obDetailRepo)
        {
            _nkhtRep = nkhtRep;
            _unitOfWork = unitOfWork;
            _obDetailRepo = obDetailRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelNKHTCommand request, CancellationToken cancellationToken)
        {
            if (request.IsCancel == true)
            {

                if (!request.NKHTs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NKHT");

                foreach (var item in request.NKHTs)
                {
                    //Phiếu nhập kho mua hàng
                    var nkht = await _nkhtRep.FindOneAsync(x => x.GoodsReturnId == item.NkhtId);

                    //Check
                    if (nkht is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho hàng trả");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    nkht.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(nkht.MaterialDocument))// && string.IsNullOrEmpty(nkht.ReverseDocument))
                        nkht.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nkht.ReverseDocument))
                    //    nkht.Status = "NOT";
                    nkht.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone class
                    var serialized = JsonConvert.SerializeObject(nkht);
                    var nkhtNew = JsonConvert.DeserializeObject<GoodsReturnModel>(serialized);

                    nkhtNew.GoodsReturnId = Guid.NewGuid();
                    nkhtNew.Status = "NOT";
                    nkhtNew.TotalQuantity = 0;
                    nkhtNew.DeliveredQuantity = 0;
                    nkht.OpenQuantity = 0;
                    nkhtNew.MaterialDocument = null;
                    nkhtNew.ReverseDocument = null;
                    #region code cũ
                    //var nkhtNew = new GoodsReturnModel
                    //{
                    //    GoodsReturnId = Guid.NewGuid(),
                    //    PlantCode = nkht.PlantCode,
                    //    DetailODId = nkht.DetailODId,
                    //    ShipToParty = nkht.ShipToParty,
                    //    ShipToPartyName = nkht.ShipToPartyName,
                    //    WeightSessionId = nkht.WeightSessionId,
                    //    WeightVote = nkht.WeightVote,
                    //    BagQuantity = nkht.BagQuantity,
                    //    SingleWeight = nkht.SingleWeight,
                    //    WeightHeadCode = nkht.WeightHeadCode,
                    //    Weight = nkht.Weight,
                    //    ConfirmQty = nkht.ConfirmQty,
                    //    QuantityWithPackaging = nkht.QuantityWithPackaging,
                    //    VehicleCode = nkht.VehicleCode,
                    //    QuantityWeitght = nkht.QuantityWeitght,
                    //    TruckNumber = nkht.TruckNumber,
                    //    InputWeight = nkht.InputWeight,
                    //    OutputWeight = nkht.OutputWeight,
                    //    Description = nkht.Description,
                    //    MaterialCode = nkht.MaterialCode,
                    //    SlocCode = nkht.SlocCode,
                    //    Image = nkht.Image,
                    //    Status = nkht.Status,
                    //    StartTime = nkht.StartTime,
                    //    EndTime = nkht.EndTime,
                    //    DocumentDate = nkht.DocumentDate,
                    //    CreateTime = DateTime.Now,
                    //    Actived = true
                    //};
                    #endregion

                    _nkhtRep.Add(nkhtNew);
                }
            }
            else
            {

                if (!request.NKHTs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NKHT");

                foreach (var item in request.NKHTs)
                {
                    //Phiếu nhập kho mua hàng
                    var nkht = await _nkhtRep.GetQuery().Include(x => x.DetailOD).FirstOrDefaultAsync(x => x.GoodsReturnId == item.NkhtId);

                    //Check
                    if (nkht is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho hàng trả");

                    //Cập nhật Batch và MaterialDocument
                    nkht.Batch = item.Batch;
                    nkht.MaterialDocument = item.MaterialDocument;
                    nkht.TotalQuantity = nkht.DetailODId.HasValue ? nkht.DetailOD.DeliveryQuantity : 0;
                    nkht.DeliveredQuantity = nkht.DetailODId.HasValue ? nkht.DetailOD.PickedQuantityPUoM : 0;
                    nkht.OpenQuantity = nkht.TotalQuantity - nkht.DeliveredQuantity;
                    if (!string.IsNullOrEmpty(nkht.MaterialDocument))// && string.IsNullOrEmpty(nkht.ReverseDocument))
                        nkht.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nkht.ReverseDocument))
                    //    nkht.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
