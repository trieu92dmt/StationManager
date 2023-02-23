using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.NKDCNBs
{
    public class UpdateAndCancelNKDCNBCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelNKDCNB> NKDCNBs { get; set; } = new List<UpdateAndCancelNKDCNB>();
    }

    public class UpdateAndCancelNKDCNB
    {
        public Guid NkdcnbId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelNKDCNBCommandHandler : IRequestHandler<UpdateAndCancelNKDCNBCommand, bool>
    {
        private readonly IRepository<InhouseTransferModel> _nkdcnbRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DetailOutboundDeliveryModel> _obDetailRepo;

        public UpdateAndCancelNKDCNBCommandHandler(IRepository<InhouseTransferModel> nkdcnbRep, IUnitOfWork unitOfWork,
                                                 IRepository<DetailOutboundDeliveryModel> obDetailRepo)
        {
            _nkdcnbRep = nkdcnbRep;
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
        public async Task<bool> Handle(UpdateAndCancelNKDCNBCommand request, CancellationToken cancellationToken)
        {
            if (request.IsCancel == true)
            {

                if (!request.NKDCNBs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu nhập kho điều chuyển nội bộ");

                foreach (var item in request.NKDCNBs)
                {
                    //Phiếu nhập kho dcnb
                    var nkdcnb = await _nkdcnbRep.FindOneAsync(x => x.InhouseTransferId == item.NkdcnbId);

                    //Check
                    if (nkdcnb is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho điều chuyển nội bộ");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    nkdcnb.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(nkdcnb.MaterialDocument)//) && string.IsNullOrEmpty(nkdcnb.ReverseDocument))
                        nkdcnb.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nkdcnb.ReverseDocument))
                    //    nkdcnb.Status = "NOT";
                    nkdcnb.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(nkdcnb);
                    var nkdcnbNew = JsonConvert.DeserializeObject<InhouseTransferModel>(serialized);

                    nkdcnbNew.InhouseTransferId = Guid.NewGuid();
                    nkdcnbNew.MaterialDocument = null;
                    nkdcnbNew.ReverseDocument = null;

                    #region code cũ
                    //var nkdcnbNew = new InhouseTransferModel
                    //{
                    //    InhouseTransferId = Guid.NewGuid(),
                    //    PlantCode = nkdcnb.PlantCode,
                    //    DetailODId = nkdcnb.DetailODId,
                    //    WeightSessionId = nkdcnb.WeightSessionId,
                    //    MaterialCodeInt = nkdcnb.MaterialCodeInt,
                    //    WeightVote = nkdcnb.WeightVote,
                    //    BagQuantity = nkdcnb.BagQuantity,
                    //    SingleWeight = nkdcnb.SingleWeight,
                    //    WeightHeadCode = nkdcnb.WeightHeadCode,
                    //    Weight = nkdcnb.Weight,
                    //    ConfirmQty = nkdcnb.ConfirmQty,
                    //    QuantityWithPackaging = nkdcnb.QuantityWithPackaging,
                    //    VehicleCode = nkdcnb.VehicleCode,
                    //    QuantityWeitght = nkdcnb.QuantityWeitght,
                    //    TruckInfoId = nkdcnb.TruckInfoId,
                    //    TruckNumber = nkdcnb.TruckNumber,
                    //    InputWeight = nkdcnb.InputWeight,
                    //    OutputWeight = nkdcnb.OutputWeight,
                    //    Description = nkdcnb.Description,
                    //    MaterialCode = nkdcnb.MaterialCode,
                    //    SlocCode = nkdcnb.SlocCode,
                    //    SlocName = nkdcnb.SlocName,
                    //    Image = nkdcnb.Image,
                    //    Status = nkdcnb.Status,
                    //    StartTime = nkdcnb.StartTime,
                    //    EndTime = nkdcnb.EndTime,
                    //    CreateTime = DateTime.Now,
                    //    Actived = true
                    //};
                    #endregion

                    _nkdcnbRep.Add(nkdcnbNew);
                }
            }
            else
            {

                if (!request.NKDCNBs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NKDCNB");

                foreach (var item in request.NKDCNBs)
                {
                    //Phiếu nhập kho điều chuyển nội bộ
                    var nkdcnb = await _nkdcnbRep.FindOneAsync(x => x.InhouseTransferId == item.NkdcnbId);

                    //Check
                    if (nkdcnb is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho điều chuyển nội bộ");

                    //Cập nhật Batch và MaterialDocument
                    nkdcnb.Batch = item.Batch;
                    nkdcnb.MaterialDocument = item.MaterialDocument;
                    if (!string.IsNullOrEmpty(nkdcnb.MaterialDocument))// && string.IsNullOrEmpty(nkdcnb.ReverseDocument))
                        nkdcnb.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nkdcnb.ReverseDocument))
                    //    nkdcnb.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
