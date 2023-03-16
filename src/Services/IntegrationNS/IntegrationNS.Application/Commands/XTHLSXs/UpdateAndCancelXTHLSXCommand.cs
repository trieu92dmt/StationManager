using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection.Metadata;

namespace IntegrationNS.Application.Commands.XTHLSXs
{
    public class UpdateAndCancelXTHLSXCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelXTHLSX> XTHLSXs { get; set; } = new List<UpdateAndCancelXTHLSX>();
    }

    public class UpdateAndCancelXTHLSX
    {
        public Guid XthlsxId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelXTHLSXCommandHandler : IRequestHandler<UpdateAndCancelXTHLSXCommand, bool>
    {
        private readonly IRepository<IssueForProductionModel> _xthlsxRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<DetailWorkOrderModel> _dtWoRepo;

        public UpdateAndCancelXTHLSXCommandHandler(IRepository<IssueForProductionModel> xthlsxRepo, IUnitOfWork unitOfWork, IRepository<AccountModel> userRepo, 
                                                   IRepository<DetailWorkOrderModel> dtWoRepo)
        {
            _xthlsxRepo = xthlsxRepo;
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
            _dtWoRepo = dtWoRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Handle(UpdateAndCancelXTHLSXCommand request, CancellationToken cancellationToken)
        {
            //Query user
            var users = _userRepo.GetQuery().AsNoTracking();

            if (request.IsCancel == true)
            {

                //Get query chứng từ
                var documentQuery = _dtWoRepo.GetQuery().AsNoTracking();

                if (!request.XTHLSXs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất tiêu hao lệnh sản xuất");

                foreach (var item in request.XTHLSXs)
                {
                    //Phiếu nhập kho phụ phâm phế phẩm
                    var xthlsx = await _xthlsxRepo.FindOneAsync(x => x.IssForProductiontId == item.XthlsxId);

                    //Check
                    if (xthlsx is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất tiêu hao lệnh sản xuất");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    xthlsx.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(xthlsx.MaterialDocument))// && string.IsNullOrEmpty(xthlsx.ReverseDocument))
                        xthlsx.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xthlsx.ReverseDocument))
                    //    xthlsx.Status = "NOT";
                    xthlsx.LastEditTime = DateTime.Now;

                    //Tạo line mới
                    //Clone object
                    var serialized = JsonConvert.SerializeObject(xthlsx);
                    var xthlsxNew = JsonConvert.DeserializeObject<IssueForProductionModel>(serialized);

                    //Chứng từ
                    var document = documentQuery.FirstOrDefault(x => x.DetailWorkOrderId == xthlsx.DetailWorkOrderId);

                    xthlsxNew.IssForProductiontId = Guid.NewGuid();
                    //Sau khi reverse line được tạo mới sẽ lấy số batch theo chứng từ. Line được tạo mới chỉ bị mất matdoc và reverse doc
                    xthlsxNew.Batch = document.Batch;
                    //Dòng cũ có change by --> Dòng mới sẽ không có
                    xthlsxNew.LastEditBy = null;
                    xthlsxNew.LastEditTime = null;
                    //Created By sẽ được tạo bởi sysadmin và Created On sẽ cập nhật theo ngày tạo, không lấy created on của line cũ
                    xthlsxNew.CreateBy = users.FirstOrDefault(x => x.UserName == "sysadmin").AccountId;
                    xthlsxNew.CreateTime = DateTime.Now;
                    //-------------------------//
                    xthlsxNew.Status = "NOT";
                    xthlsxNew.TotalQuantity = 0;
                    xthlsxNew.RequirementQuantiy = 0;
                    xthlsxNew.QuantityWithdrawn = 0;
                    xthlsxNew.MaterialDocument = null;
                    xthlsxNew.ReverseDocument = null;

                    #region code cũ
                    //var xthlsxNew = new IssueForProductionModel
                    //{
                    //    IssForProductiontId = Guid.NewGuid(),
                    //    PlantCode = xthlsx.PlantCode,
                    //    DetailWorkOrderId = xthlsx.DetailWorkOrderId,
                    //    WeightVote = xthlsx.WeightVote,
                    //    BagQuantity = xthlsx.BagQuantity,
                    //    SingleWeight = xthlsx.SingleWeight,
                    //    WeightHeadCode = xthlsx.WeightHeadCode,
                    //    Weight = xthlsx.Weight,
                    //    ConfirmQty = xthlsx.ConfirmQty,
                    //    QuantityWithPackaging = xthlsx.QuantityWithPackaging,
                    //    QuantityWeitght = xthlsx.QuantityWeitght,
                    //    SlocCode = xthlsx.SlocCode,
                    //    Image = xthlsx.Image,
                    //    Status = xthlsx.Status,
                    //    StartTime = xthlsx.StartTime,
                    //    EndTime = xthlsx.EndTime,
                    //    CreateTime = DateTime.Now,
                    //    Actived = true,
                    //    ComponentCode = xthlsx.ComponentCode,
                    //    ComponentCodeInt = xthlsx.ComponentCodeInt,
                    //    WeightId = xthlsx.WeightId,
                    //    SlocName = xthlsx.SlocName 
                    //};
                    #endregion

                    _xthlsxRepo.Add(xthlsxNew);
                }
            }
            else
            {

                if (!request.XTHLSXs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu xuất tiêu hao lệnh sản xuất");

                foreach (var item in request.XTHLSXs)
                {
                    //Phiếu xuất tiêu hao lsx
                    var xthlsx = await _xthlsxRepo.GetQuery().Include(x => x.DetailWorkOrder).ThenInclude(x => x.WorkOrder).FirstOrDefaultAsync(x => x.IssForProductiontId == item.XthlsxId);

                    //Check
                    if (xthlsx is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu xuất tiêu hao lệnh sản xuất");

                    //Cập nhật Batch và MaterialDocument
                    xthlsx.Batch = item.Batch;
                    xthlsx.MaterialDocument = item.MaterialDocument;
                    xthlsx.TotalQuantity = xthlsx.DetailWorkOrder.WorkOrder.TargetQuantity;
                    xthlsx.RequirementQuantiy = xthlsx.RequirementQuantiy;
                    xthlsx.QuantityWithdrawn = xthlsx.QuantityWithdrawn;
                    if (!string.IsNullOrEmpty(xthlsx.MaterialDocument))// && string.IsNullOrEmpty(xthlsx.ReverseDocument))
                        xthlsx.Status = "POST";
                    //else if (!string.IsNullOrEmpty(xthlsx.ReverseDocument))
                    //    xthlsx.Status = "NOT";
                }             
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
