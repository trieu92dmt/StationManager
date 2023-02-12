using ISD.Core.Extensions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.SeedWork.Repositories;
using ISD.Core.Utilities;
using ISD.Infrastructure.Models;
using MediatR;
using MES.Application.Commands.OutboundDelivery;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NKPPPP
{
    public class SaveNKPPPPCommand: IRequest<bool>
    {
        public List<SaveNKPPPP> SaveNKPPPPs { get; set; } = new List<SaveNKPPPP>();
    }

    public class SaveNKPPPP
    {
        //Plant
        public string Plant { get; set; }
        //Production Order
        public string WorkOrder { get; set; }
        //Material
        public string Material { get; set; }
        //Component
        public string Component { get; set; }
        //Sloc
        public string SlocCode { get; set; }
        //SL bao
        public decimal? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confỉm Qty
        public decimal? ConfirmQty { get; set; }
        //Số lần cân
        public int QuantityWeight { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackaging { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
    }

    public class SaveNKPPPPCommandHandler : IRequestHandler<SaveNKPPPPCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<ScrapFromProductionModel> _nkppppRepo;
        private readonly IRepository<DetailWorkOrderModel> _woDetailRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public SaveNKPPPPCommandHandler(IUnitOfWork unitOfWork, IRepository<WeighSessionModel> weightSsRepo,
                                             IRepository<ScaleModel> scaleRepo, IUtilitiesService utilitiesService,
                                             IRepository<ProductModel> prodRepo, IRepository<ScrapFromProductionModel> nkppppRepo,
                                             IRepository<DetailWorkOrderModel> woDetailRepo, IRepository<StorageLocationModel> slocRepo)
        {
            _unitOfWork = unitOfWork;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _utilitiesService = utilitiesService;
            _prodRepo = prodRepo;
            _nkppppRepo = nkppppRepo;
            _woDetailRepo = woDetailRepo;
            _slocRepo = slocRepo;
        }

        public async Task<bool> Handle(SaveNKPPPPCommand request, CancellationToken cancellationToken)
        {
            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Get query sloc
            var slocs = _slocRepo.GetQuery().AsNoTracking();

            //Danh sách nhập kho tpsx
            var nkhts = await _nkppppRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = nkhts.Count() > 0 ? nkhts.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            //Query wo
            var detailWos = _woDetailRepo.GetQuery().Include(x => x.WorkOrder).AsNoTracking();

            var index = 1;
            foreach (var item in request.SaveNKPPPPs)
            {
                var ScFromProductiontId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, ScFromProductiontId.ToString(), $"{ScFromProductiontId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NKPPPP");
                }

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                //Lấy ra component
                var detailWo = !string.IsNullOrEmpty(item.WorkOrder) && !string.IsNullOrEmpty(item.Material) && string.IsNullOrEmpty(item.Component) ?
                                    detailWos.FirstOrDefault(d => d.WorkOrder.WorkOrderCodeInt == long.Parse(item.WorkOrder) &&
                                                             d.WorkOrder.ProductCodeInt == long.Parse(item.Material) &&
                                                             d.ProductCodeInt == long.Parse(item.Component)) : null;

                _nkppppRepo.Add(new ScrapFromProductionModel
                {
                    //1 ScFromProductiontId
                    ScFromProductiontId = ScFromProductiontId,
                    //2 DetailWorkOrderId
                    DetailWorkOrderId = detailWo != null ? detailWo.DetailWorkOrderId : null,
                    //3 PlantCode
                    PlantCode = item.Plant,
                    //4   MaterialCode
                    ComponentCode = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.Component) && x.PlantCode == item.Plant).ProductCode,
                    ComponentCodeInt = long.Parse(item.Component),
                    //5   WeightId
                    WeightId = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                               weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.WeighSessionID : null,
                    //6 WeightVote
                    WeightVote = $"N{long.Parse(lastIndex) + index}",
                    //7   BagQuantity
                    BagQuantity = item.BagQuantity,
                    //8   SingleWeight
                    SingleWeight = item.SingleWeight,
                    //9  WeightHeadCode
                    WeightHeadCode = item.WeightHeadCode,
                    //10  Weight
                    Weight = item.Weight,
                    //11  ConfirmQty
                    ConfirmQty = item.ConfirmQty,
                    //12  QuantityWithPackaging
                    QuantityWithPackaging = item.QuantityWithPackaging,
                    //13  QuantityWeitght
                    QuantityWeitght = item.QuantityWeight,
                    //14  Description
                    Description = item.Description,
                    //15  Image
                    Image = !string.IsNullOrEmpty(imgPath) ? imgPath : null,
                    //16  Status
                    Status = "NOT",
                    //17  StartTime
                    StartTime = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                      weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN")?.StartTime : null,
                    //18  EndTime
                    EndTime = DateTime.Now,
                    //21  SlocCode
                    SlocCode = item.SlocCode,
                    //22  SlocName
                    SlocName = slocs.FirstOrDefault(x => x.StorageLocationCode == item.SlocCode)?.StorageLocationName,
                    //24  CreateTime
                    CreateTime = DateTime.Now,
                    //25  CreateBy
                    CreateBy = TokenExtensions.GetAccountId(),
                    //28  Actived
                    Actived = true

                });

                index++;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
