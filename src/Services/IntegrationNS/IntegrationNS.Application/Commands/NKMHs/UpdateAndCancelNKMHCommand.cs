using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AccountModel> _userRepo;

        public UpdateAndCancelNKMHCommandHandler(IRepository<GoodsReceiptModel> nkmhRep, IRepository<PurchaseOrderDetailModel> poDetailRepo, IUnitOfWork unitOfWork,
                                                 IRepository<AccountModel> userRepo)
        {
            _nkmhRep = nkmhRep;
            _poDetailRepo = poDetailRepo;
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
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
            //Query user
            var users = _userRepo.GetQuery().AsNoTracking();

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

                    //Chứng từ
                    var document = await _poDetailRepo.FindOneAsync(x => x.PurchaseOrderDetailId == nkmh.PurchaseOrderDetailId);

                    //Khác id
                    nkmhNew.GoodsReceiptId = Guid.NewGuid();
                    //Sau khi reverse line được tạo mới sẽ lấy số batch theo chứng từ. Line được tạo mới chỉ bị mất matdoc và reverse doc
                    nkmhNew.Batch = document.Batch;
                    //Dòng cũ có change by --> Dòng mới sẽ không có
                    nkmhNew.LastEditBy = null;
                    nkmhNew.LastEditTime = null;
                    //Created By sẽ được tạo bởi sysadmin và Created On sẽ cập nhật theo ngày tạo, không lấy created on của line cũ
                    nkmhNew.CreateBy = users.FirstOrDefault(x => x.UserName == "sysadmin").AccountId;
                    nkmhNew.CreateTime = DateTime.Now;
                    //-------------------------//
                    nkmhNew.Batch = document != null ? document.Batch : null;
                    nkmhNew.Status = "NOT";
                    nkmhNew.TotalQuantity = 0;
                    nkmhNew.DeliveryQuantity = 0;
                    nkmhNew.OpenQuantity = 0;
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
                    var nkmh = await _nkmhRep.GetQuery().Include(x => x.PurchaseOrderDetail).FirstOrDefaultAsync(x => x.GoodsReceiptId == item.NkmhId);

                    //Check
                    if (nkmh is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho mua hàng");

                    //Cập nhật Batch và MaterialDocument
                    nkmh.Batch = item.Batch;
                    nkmh.MaterialDocument = item.MaterialDocument;
                    nkmh.TotalQuantity = nkmh.PurchaseOrderDetailId.HasValue ? nkmh.PurchaseOrderDetail.OrderQuantity : 0;
                    nkmh.DeliveryQuantity = nkmh.PurchaseOrderDetailId.HasValue ? nkmh.PurchaseOrderDetail.QuantityReceived : 0;
                    nkmh.OpenQuantity = nkmh.PurchaseOrderDetailId.HasValue ? nkmh.PurchaseOrderDetail.OpenQuantity : 0;
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
