using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Models;
using Core.Properties;
using Core.SeedWork.Repositories;
using Core.Commons;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationNS.Application.Commands.NKTPSXs
{
    public class UpdateAndCancelNKTPSXCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelNKTPSX> NKTPSXs { get; set; } = new List<UpdateAndCancelNKTPSX>();
    }

    public class UpdateAndCancelNKTPSX
    {
        public Guid NktpsxId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelNKTPSXCommandHandler : IRequestHandler<UpdateAndCancelNKTPSXCommand, bool>
    {
        private readonly IRepository<ReceiptFromProductionModel> _nktpsxRep;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WorkOrderModel> _woRepo;
        private readonly IRepository<AccountModel> _userRepo;

        public UpdateAndCancelNKTPSXCommandHandler(IRepository<ReceiptFromProductionModel> nktpsxRep, IUnitOfWork unitOfWork,
                                                   IRepository<WorkOrderModel> woRepo, IRepository<AccountModel> userRepo)
        {
            _nktpsxRep = nktpsxRep;
            _unitOfWork = unitOfWork;
            _woRepo = woRepo;
            _userRepo = userRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelNKTPSXCommand request, CancellationToken cancellationToken)
        {
            //Query user
            var users = _userRepo.GetQuery().AsNoTracking();

            if (request.IsCancel == true)
            {

                //Get query chứng từ
                var documentQuery = _woRepo.GetQuery().AsNoTracking();

                if (!request.NKTPSXs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NKTPSX");

                foreach (var item in request.NKTPSXs)
                {
                    //Phiếu nhập kho tp sx
                    var nktpsx = await _nktpsxRep.FindOneAsync(x => x.RcFromProductiontId == item.NktpsxId);

                    //Check
                    if (nktpsx is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho tp sản xuất");

                    //Nếu đã reverse thì không reverse nữa
                    if (!string.IsNullOrEmpty(nktpsx.ReverseDocument))
                    {
                        throw new ISDException(CommonResource.Msg_Canceled, $"Phiếu nhập kho {item.NktpsxId}");
                    }

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    nktpsx.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(nktpsx.MaterialDocument))// && string.IsNullOrEmpty(nktpsx.ReverseDocument))
                        nktpsx.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nktpsx.ReverseDocument))
                    //    nktpsx.Status = "NOT";
                    nktpsx.LastEditTime = DateTime.Now;

                    //Clone object
                    var serialized = JsonConvert.SerializeObject(nktpsx);
                    var nktpsxNew = JsonConvert.DeserializeObject<ReceiptFromProductionModel>(serialized);

                    //Chứng từ
                    var document = documentQuery.FirstOrDefault(x => x.WorkOrderId == nktpsx.WorkOrderId);

                    nktpsxNew.RcFromProductiontId = Guid.NewGuid();
                    //Sau khi reverse line được tạo mới sẽ lấy số batch theo chứng từ. Line được tạo mới chỉ bị mất matdoc và reverse doc
                    nktpsxNew.Batch = document.Batch;
                    //Dòng cũ có change by --> Dòng mới sẽ không có
                    nktpsxNew.LastEditBy = null;
                    nktpsxNew.LastEditTime = null;
                    //Created By sẽ được tạo bởi sysadmin và Created On sẽ cập nhật theo ngày tạo, không lấy created on của line cũ
                    nktpsxNew.CreateBy = users.FirstOrDefault(x => x.UserName == "sysadmin").AccountId;
                    nktpsxNew.CreateTime = DateTime.Now;
                    //-------------------------//
                    nktpsxNew.Status = "NOT";
                    nktpsxNew.TotalQuantity = 0;
                    nktpsxNew.DeliveryQuantity = 0;
                    nktpsxNew.OpenQuantity = 0;
                    nktpsxNew.MaterialDocument = null;
                    nktpsxNew.ReverseDocument = null;


                    //Tạo line mới
                    #region code cũ
                    //var nktpsxNew = new ReceiptFromProductionModel
                    //{
                    //    RcFromProductiontId = Guid.NewGuid(),
                    //    PlantCode = nktpsx.PlantCode,
                    //    WorkOrderId = nktpsx.WorkOrderId,
                    //    WeightId = nktpsx.WeightId,
                    //    WeightVote = nktpsx.WeightVote,
                    //    BagQuantity = nktpsx.BagQuantity,
                    //    SingleWeight = nktpsx.SingleWeight,
                    //    WeightHeadCode = nktpsx.WeightHeadCode,
                    //    Weight = nktpsx.Weight,
                    //    ConfirmQty = nktpsx.ConfirmQty,
                    //    QuantityWithPackaging = nktpsx.QuantityWithPackaging,
                    //    QuantityWeitght = nktpsx.QuantityWeitght,
                    //    Description = nktpsx.Description,
                    //    MaterialCode = nktpsx.MaterialCode,
                    //    MaterialCodeInt = nktpsx.MaterialCodeInt,
                    //    SlocCode = nktpsx.SlocCode,
                    //    SlocName = nktpsx.SlocName,
                    //    Image = nktpsx.Image,
                    //    Status = nktpsx.Status,
                    //    StartTime = nktpsx.StartTime,
                    //    EndTime = nktpsx.EndTime,
                    //    CreateTime = DateTime.Now,
                    //    Actived = true
                    //};
                    #endregion

                    _nktpsxRep.Add(nktpsxNew);
                }
            }
            else
            {

                if (!request.NKTPSXs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NKTPSX");

                foreach (var item in request.NKTPSXs)
                {
                    //Phiếu nhập kho mua hàng
                    var nktpsx = await _nktpsxRep.GetQuery().Include(x => x.WorkOrder).FirstOrDefaultAsync(x => x.RcFromProductiontId == item.NktpsxId);

                    //Check
                    if (nktpsx is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho TP sản xuất");

                    //Cập nhật Batch và MaterialDocument
                    nktpsx.Batch = item.Batch;
                    nktpsx.MaterialDocument = item.MaterialDocument;
                    nktpsx.TotalQuantity = nktpsx.WorkOrderId.HasValue ? nktpsx.WorkOrder.TargetQuantity : 0;
                    nktpsx.DeliveryQuantity = nktpsx.WorkOrderId.HasValue ? nktpsx.WorkOrder.DeliveredQuantity : 0;
                    nktpsx.OpenQuantity = nktpsx.TotalQuantity - nktpsx.DeliveryQuantity;
                    if (!string.IsNullOrEmpty(nktpsx.MaterialDocument))// && string.IsNullOrEmpty(nktpsx.ReverseDocument))
                        nktpsx.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nktpsx.ReverseDocument))
                    //    nktpsx.Status = "NOT";
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
