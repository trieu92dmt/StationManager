using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Newtonsoft.Json;
using System.Reflection.Metadata;

namespace IntegrationNS.Application.Commands.NKPPPPs
{
    public class UpdateAndCancelNKPPPPCommand : IRequest<bool>
    {
        public bool? IsCancel { get; set; }
        public List<UpdateAndCancelNKPPPP> NKPPPPs { get; set; } = new List<UpdateAndCancelNKPPPP>();
    }

    public class UpdateAndCancelNKPPPP
    {
        public Guid NkppppId { get; set; }
        public string Batch { get; set; }
        public string MaterialDocument { get; set; }
        public string ReverseDocument { get; set; }
    }



    public class UpdateAndCancelNKPPPPCommandHandler : IRequestHandler<UpdateAndCancelNKPPPPCommand, bool>
    {
        private readonly IRepository<ScrapFromProductionModel> _nkppppRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AccountModel> _userRepo;
        private readonly IRepository<DetailWorkOrderModel> _dtWoRepo;

        public UpdateAndCancelNKPPPPCommandHandler(IRepository<ScrapFromProductionModel> nkppppRepo, IUnitOfWork unitOfWork, IRepository<AccountModel> userRepo, 
                                                   IRepository<DetailWorkOrderModel> dtWoRepo)
        {
            _nkppppRepo = nkppppRepo;
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
        public async Task<bool> Handle(UpdateAndCancelNKPPPPCommand request, CancellationToken cancellationToken)
        {
            //Query user
            var users = _userRepo.GetQuery().AsNoTracking();

            if (request.IsCancel == true)
            {
                //Get query chứng từ
                var documentQuery = _dtWoRepo.GetQuery().AsNoTracking();


                if (!request.NKPPPPs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NKPPPP");

                foreach (var item in request.NKPPPPs)
                {
                    //Phiếu nhập kho phụ phâm phế phẩm
                    var nkpppp = await _nkppppRepo.FindOneAsync(x => x.ScFromProductiontId == item.NkppppId);

                    //Check
                    if (nkpppp is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho phụ phẩm phế phẩm");

                    //Cập nhật Batch và MaterialDocument và ReverseDocument
                    nkpppp.ReverseDocument = item.ReverseDocument;
                    if (!string.IsNullOrEmpty(nkpppp.MaterialDocument))// && string.IsNullOrEmpty(nkpppp.ReverseDocument))
                        nkpppp.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nkpppp.ReverseDocument))
                    //    nkpppp.Status = "NOT";
                    nkpppp.LastEditTime = DateTime.Now;


                    //Clone object
                    var serialized = JsonConvert.SerializeObject(nkpppp);
                    var nkppppNew = JsonConvert.DeserializeObject<ScrapFromProductionModel>(serialized);

                    //Chứng từ
                    var document = documentQuery.FirstOrDefault(x => x.DetailWorkOrderId == nkpppp.DetailWorkOrderId);

                    nkppppNew.ScFromProductiontId = Guid.NewGuid();
                    //Sau khi reverse line được tạo mới sẽ lấy số batch theo chứng từ. Line được tạo mới chỉ bị mất matdoc và reverse doc
                    nkppppNew.Batch = document.Batch;
                    //Dòng cũ có change by --> Dòng mới sẽ không có
                    nkppppNew.LastEditBy = null;
                    nkppppNew.LastEditTime = null;
                    //Created By sẽ được tạo bởi sysadmin và Created On sẽ cập nhật theo ngày tạo, không lấy created on của line cũ
                    nkppppNew.CreateBy = users.FirstOrDefault(x => x.UserName == "sysadmin").AccountId;
                    nkppppNew.CreateTime = DateTime.Now;
                    //-------------------------//
                    nkppppNew.Status = "NOT";
                    nkppppNew.TotalQuantity = 0;
                    nkppppNew.RequirementQuantiy = 0;
                    nkppppNew.QuantityWithdrawn = 0;
                    nkppppNew.MaterialDocument = null;
                    nkppppNew.ReverseDocument = null;

                    #region code cũ
                    //Tạo line mới
                    //var nkppppNew = new ScrapFromProductionModel
                    //{
                    //    ScFromProductiontId = Guid.NewGuid(),
                    //    PlantCode = nkpppp.PlantCode,
                    //    DetailWorkOrderId = nkpppp.DetailWorkOrderId,
                    //    WeightVote = nkpppp.WeightVote,
                    //    BagQuantity = nkpppp.BagQuantity,
                    //    SingleWeight = nkpppp.SingleWeight,
                    //    WeightHeadCode = nkpppp.WeightHeadCode,
                    //    Weight = nkpppp.Weight,
                    //    ConfirmQty = nkpppp.ConfirmQty,
                    //    QuantityWithPackaging = nkpppp.QuantityWithPackaging,
                    //    QuantityWeitght = nkpppp.QuantityWeitght,
                    //    SlocCode = nkpppp.SlocCode,
                    //    Image = nkpppp.Image,
                    //    Status = nkpppp.Status,
                    //    StartTime = nkpppp.StartTime,
                    //    EndTime = nkpppp.EndTime,
                    //    CreateTime = DateTime.Now,
                    //    Actived = true,
                    //    ComponentCode = nkpppp.ComponentCode,
                    //    ComponentCodeInt = nkpppp.ComponentCodeInt,
                    //    WeightId = nkpppp.WeightId,
                    //    SlocName = nkpppp.SlocName 
                    //};
                    #endregion

                    _nkppppRepo.Add(nkppppNew);
                }
            }
            else
            {

                if (!request.NKPPPPs.Any())
                    throw new ISDException(CommonResource.Msg_NotFound, "Dữ liệu NKPPPP");

                foreach (var item in request.NKPPPPs)
                {
                    //Phiếu nhập kho phụ phẩm phế phẩm
                    var nkpppp = await _nkppppRepo.GetQuery().Include(x => x.DetailWorkOrder).ThenInclude(x => x.WorkOrder).FirstOrDefaultAsync(x => x.ScFromProductiontId == item.NkppppId);

                    //Check
                    if (nkpppp is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho phụ phẩm phế phẩm");

                    //Cập nhật Batch và MaterialDocument
                    nkpppp.Batch = item.Batch;
                    nkpppp.MaterialDocument = item.MaterialDocument;
                    nkpppp.TotalQuantity = nkpppp.DetailWorkOrderId.HasValue ? nkpppp.DetailWorkOrder.WorkOrder.TargetQuantity : 0;
                    nkpppp.RequirementQuantiy = nkpppp.DetailWorkOrderId.HasValue ? nkpppp.DetailWorkOrder.RequirementQuantiy : 0;
                    nkpppp.QuantityWithdrawn = nkpppp.DetailWorkOrderId.HasValue ? nkpppp.DetailWorkOrder.QuantityWithdrawn : 0;
                    if (!string.IsNullOrEmpty(nkpppp.MaterialDocument))// && string.IsNullOrEmpty(nkpppp.ReverseDocument))
                        nkpppp.Status = "POST";
                    //else if (!string.IsNullOrEmpty(nkpppp.ReverseDocument))
                    //    nkpppp.Status = "NOT";
                }             
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
