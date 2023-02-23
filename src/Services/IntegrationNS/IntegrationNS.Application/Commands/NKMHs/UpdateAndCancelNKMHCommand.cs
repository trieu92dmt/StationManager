using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Newtonsoft.Json;

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
                    if (!string.IsNullOrEmpty(nkmh.MaterialDocument))// && string.IsNullOrEmpty(nkmh.ReverseDocument))
                        nkmh.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nkmh.ReverseDocument))
                    //    nkmh.Status = "NOT";
                    nkmh.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone class
                    var serialized = JsonConvert.SerializeObject(nkmh);
                    var nkmhNew = JsonConvert.DeserializeObject<GoodsReceiptModel>(serialized);
                    //Khác id
                    nkmhNew.GoodsReceiptId = Guid.NewGuid();
                    nkmhNew.MaterialDocument = null;
                    nkmhNew.ReverseDocument = null;

                    #region code cũ
                    //var nkmhNew = new GoodsReceiptModel
                    //{
                    //    GoodsReceiptId = Guid.NewGuid(),
                    //    PlantCode = nkmh.PlantCode,
                    //    PurchaseOrderDetailId = nkmh.PurchaseOrderDetailId,
                    //    WeightId = nkmh.WeightId,
                    //    WeitghtVote = nkmh.WeitghtVote,
                    //    BagQuantity = nkmh.BagQuantity,
                    //    SingleWeight = nkmh.SingleWeight,
                    //    WeightHeadCode = nkmh.WeightHeadCode,
                    //    Weight = nkmh.Weight,
                    //    ConfirmQty = nkmh.ConfirmQty,
                    //    QuantityWithPackaging = nkmh.QuantityWithPackaging,
                    //    VehicleCode = nkmh.VehicleCode,
                    //    QuantityWeitght = nkmh.QuantityWeitght,
                    //    TruckQuantity = nkmh.TruckQuantity,
                    //    InputWeight = nkmh.InputWeight,
                    //    OutputWeight = nkmh.OutputWeight,
                    //    Description = nkmh.Description,
                    //    MaterialCode = nkmh.MaterialCode,
                    //    MaterialCodeInt = nkmh.MaterialCodeInt,
                    //    SlocCode = nkmh.SlocCode,
                    //    SlocName = nkmh.SlocName,   
                    //    Img = nkmh.Img,
                    //    Status = nkmh.Status,
                    //    StartTime = nkmh.StartTime,
                    //    EndTime = nkmh.EndTime,
                    //    DocumentDate = nkmh.DocumentDate,
                    //    DateKey = nkmh.DateKey,
                    //    CreateTime = DateTime.Now,
                    //    Actived = true
                    //};
                    #endregion

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
                    if (!string.IsNullOrEmpty(nkmh.MaterialDocument))// && string.IsNullOrEmpty(nkmh.ReverseDocument))
                        nkmh.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nkmh.ReverseDocument))
                    //    nkmh.Status = "NOT";
                }             
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
