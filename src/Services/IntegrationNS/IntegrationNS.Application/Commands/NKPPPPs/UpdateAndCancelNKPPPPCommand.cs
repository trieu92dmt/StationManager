using Core.Exceptions;
using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Newtonsoft.Json;

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

        public UpdateAndCancelNKPPPPCommandHandler(IRepository<ScrapFromProductionModel> nkppppRepo, IUnitOfWork unitOfWork)
        {
            _nkppppRepo = nkppppRepo;
            _unitOfWork = unitOfWork;
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
            if (request.IsCancel == true)
            {

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
                    if (!string.IsNullOrEmpty(nkpppp.MaterialDocument) && string.IsNullOrEmpty(nkpppp.ReverseDocument))
                        nkpppp.Status = "POST";
                    else if (!string.IsNullOrEmpty(nkpppp.ReverseDocument))
                        nkpppp.Status = "NOT";
                    nkpppp.LastEditTime = DateTime.Now;


                    //Clone object
                    var serialized = JsonConvert.SerializeObject(nkpppp);
                    var nkppppNew = JsonConvert.DeserializeObject<ScrapFromProductionModel>(serialized);

                    nkppppNew.ScFromProductiontId = Guid.NewGuid();
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
                    var nkpppp = await _nkppppRepo.FindOneAsync(x => x.ScFromProductiontId == item.NkppppId);

                    //Check
                    if (nkpppp is null)
                        throw new ISDException(CommonResource.Msg_NotFound, "Phiếu nhập kho phụ phẩm phế phẩm");

                    //Cập nhật Batch và MaterialDocument
                    nkpppp.Batch = item.Batch;
                    nkpppp.MaterialDocument = item.MaterialDocument;
                    if (!string.IsNullOrEmpty(nkpppp.MaterialDocument) && string.IsNullOrEmpty(nkpppp.ReverseDocument))
                        nkpppp.Status = "POST";
                    else if (!string.IsNullOrEmpty(nkpppp.ReverseDocument))
                        nkpppp.Status = "NOT";
                }             
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
