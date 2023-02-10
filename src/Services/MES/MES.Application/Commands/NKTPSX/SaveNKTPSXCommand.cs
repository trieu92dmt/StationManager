using ISD.Core.Extensions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.SeedWork.Repositories;
using ISD.Core.Utilities;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.OutboundDelivery
{
    public class SaveNKTPSXCommand : IRequest<bool>
    {
        public List<SaveNKTPSX> SaveNKTPSXs { get; set; } = new List<SaveNKTPSX>();
    }

    public class SaveNKTPSX
    {
        //Plant
        public string Plant { get; set; }
        //Production Order
        public string WorkOrder { get; set; }
        //Material
        public string MaterialCode { get; set; }
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
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
    }

    public class SaveNKTPSXCommandHandler : IRequestHandler<SaveNKTPSXCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IUtilitiesService _utilitiesService;
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<ReceiptFromProductionModel> _nktpsxRepo;
        private readonly IRepository<WorkOrderModel> _woRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public SaveNKTPSXCommandHandler(IUnitOfWork unitOfWork, IRepository<WeighSessionModel> weightSsRepo,
                                             IRepository<ScaleModel> scaleRepo,IUtilitiesService utilitiesService,
                                             IRepository<ProductModel> prodRepo, IRepository<ReceiptFromProductionModel> nktpsxRepo,
                                             IRepository<WorkOrderModel> woRepo, IRepository<StorageLocationModel> slocRepo)
        {
            _unitOfWork = unitOfWork;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _utilitiesService = utilitiesService;
            _prodRepo = prodRepo;
            _nktpsxRepo = nktpsxRepo;
            _woRepo = woRepo;
            _slocRepo = slocRepo;
        }

        public async Task<bool> Handle(SaveNKTPSXCommand request, CancellationToken cancellationToken)
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
            var nkhts = await _nktpsxRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = nkhts.Count() > 0 ? nkhts.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            //Query wo
            var wos = _woRepo.GetQuery().AsNoTracking();

            var index = 1;
            foreach (var item in request.SaveNKTPSXs)
            {
                var RcFromProductiontId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, RcFromProductiontId.ToString(), $"{RcFromProductiontId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NKTPSX");
                }

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                //Lấy product
                var material = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.MaterialCode) && x.PlantCode == item.Plant).ProductCode;

                //Lấy ra workorder
                var wo = !string.IsNullOrEmpty(item.WorkOrder) ? wos.FirstOrDefault(d => d.WorkOrderCodeInt == long.Parse(item.WorkOrder)) : null;

                _nktpsxRepo.Add(new ReceiptFromProductionModel
                {
                    //1 RcFromProductiontId
                    RcFromProductiontId = RcFromProductiontId,
                    //2 WorkOrderId
                    WorkOrderId = wo != null ? wo.WorkOrderId : null,
                    //3 PlantCode
                    PlantCode = item.Plant,
                    //4   MaterialCode
                    MaterialCode = material,
                    MaterialCodeInt = long.Parse(material),
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
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
