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
    public class SaveGoodsReturnCommand : IRequest<bool>
    {
        public List<SaveGoodsReturn> SaveGoodsReturns { get; set; } = new List<SaveGoodsReturn>();
    }

    public class SaveGoodsReturn
    {
        //Plant
        public string PlantCode { get; set; }
        //Od code
        public string ODCode { get; set; }
        //Od item
        public string ODItem { get; set; }
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
        //Id cân xe tải
        public Guid? TruckInfoId { get; set; }
        //Số xe tải
        public string TruckQuantity { get; set; }
        //Số cân đầu vào
        public decimal? InputWeight { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
    }

    public class SaveGoodsReturnCommandHandler : IRequestHandler<SaveGoodsReturnCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<GoodsReturnModel> _nkhtRepo;
        private readonly IRepository<WeighSessionModel> _weightSsRepo;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<DetailOutboundDeliveryModel> _detailODRepo;
        private readonly IRepository<OutboundDeliveryModel> _obRepo;
        private readonly IUtilitiesService _utilitiesService;

        public SaveGoodsReturnCommandHandler(IUnitOfWork unitOfWork, IRepository<GoodsReturnModel> nkhtRepo, IRepository<WeighSessionModel> weightSsRepo,
                                             IRepository<ScaleModel> scaleRepo, IRepository<DetailOutboundDeliveryModel> detailODRepo,
                                             IRepository<OutboundDeliveryModel> obRepo, IUtilitiesService utilitiesService)
        {
            _unitOfWork = unitOfWork;
            _nkhtRepo = nkhtRepo;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _detailODRepo = detailODRepo;
            _obRepo = obRepo;
            _utilitiesService = utilitiesService;
        }

        public async Task<bool> Handle(SaveGoodsReturnCommand request, CancellationToken cancellationToken)
        {
            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Danh sách nhập kho mua hàng
            var nkhts = await _nkhtRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = nkhts.Any() ? nkhts.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote : "1000000";

            //Query od
            var detailODs = _detailODRepo.GetQuery().Include(x => x.OutboundDelivery).AsNoTracking();
            var ods = _obRepo.GetQuery().AsNoTracking();

            var index = 1;
            foreach (var item in request.SaveGoodsReturns)
            {
                var GoodsReturnId = Guid.NewGuid();

                var imgPath = "";
                if (!string.IsNullOrEmpty(item.Image))
                {
                    //Convert Base64 to Iformfile
                    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                    MemoryStream stream = new MemoryStream(bytes);

                    IFormFile file = new FormFile(stream, 0, bytes.Length, GoodsReturnId.ToString(), $"{GoodsReturnId.ToString()}.jpg");
                    //Save image to server
                    imgPath = await _utilitiesService.UploadFile(file, "NKHT");
                }

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                //Lấy ra outbound detail
                var detailOb = !string.IsNullOrEmpty(item.ODCode) ?
                               detailODs.FirstOrDefault(d => d.OutboundDelivery.DeliveryCodeInt == long.Parse(item.ODCode) && d.OutboundDeliveryItem == item.ODItem) : null;

                _nkhtRepo.Add(new GoodsReturnModel
                {
                    //1 GoodsReturnId
                    GoodsReturnId = GoodsReturnId,
                    //2 WeightSession Id
                    WeightSessionId = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ? 
                                      weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN").WeighSessionID : null,
                    //3 WeightHeadCode
                    WeightHeadCode = item.WeightHeadCode,   
                    //4 WeightVote
                    WeightVote = $"N{long.Parse(lastIndex.Substring(1)) + index}",
                    //5   DetailODId
                    DetailODId = !string.IsNullOrEmpty(item.ODCode) ?
                                 detailOb.DetailOutboundDeliveryId : null,
                    //6   PlantCode
                    PlantCode = item.PlantCode,
                    //7   SOType
                    SOType = !string.IsNullOrEmpty(item.ODCode) ?
                             detailOb.OutboundDelivery.SalesDocumentType : null,
                    //8   SalesOrg
                    SalesOrg = !string.IsNullOrEmpty(item.ODCode) ?
                             detailOb.OutboundDelivery.SaleOrgCode : null,
                    //9   DistributionChannel
                    DistributionChannel = !string.IsNullOrEmpty(item.ODCode) ?
                             detailOb.OutboundDelivery.DistribChannelCode : null,
                    //10  Division
                    Division = !string.IsNullOrEmpty(item.ODCode) ?
                             detailOb.OutboundDelivery.DivisionCode : null,
                    //11  SalesOrder
                    SalesOrder = !string.IsNullOrEmpty(item.ODCode) ?
                                 detailOb.SalesOrder : null,
                    //12  MaterialCode
                    MaterialCode = item.MaterialCode,
                    //14  ShipToParty
                    ShipToParty = !string.IsNullOrEmpty(item.ODCode) ?
                             detailOb.OutboundDelivery.ShiptoParty : null,
                    //15  ShipToPartyName
                    ShipToPartyName = !string.IsNullOrEmpty(item.ODCode) ?
                             detailOb.OutboundDelivery.ShiptoPartyName : null,
                    //16  SlocCode
                    SlocCode = item.SlocCode,
                    //17  Total Quantity
                    TotalQuantity = !string.IsNullOrEmpty(item.ODCode) ?
                                 detailOb.DeliveryQuantity : null,
                    //18  DeliveredQuantity
                    DeliveredQuantity = !string.IsNullOrEmpty(item.ODCode) ?
                                 detailOb.PickedQuantityPUoM : null,
                    //19  Open Quantity
                    OpenQuantity = 0,
                    //20  ConfirmQty
                    ConfirmQty = item.ConfirmQty,
                    //21  UOM
                    UOM = !string.IsNullOrEmpty(item.ODCode) ?
                          detailOb.Unit : null,
                    //22  NetWeight
                    NetWeight = !string.IsNullOrEmpty(item.ODCode) ?
                          detailOb.NetWeight : null,
                    //23  CrossWeight
                    GrossWeight = !string.IsNullOrEmpty(item.ODCode) ?
                          detailOb.GrossWeight : null,
                    //24  DocumentDate
                    DocumentDate = !string.IsNullOrEmpty(item.ODCode) ?
                          detailOb.OutboundDelivery.DocumentDate : null,
                    //25  QuantityWithPackaging
                    QuantityWithPackaging = item.QuantityWithPackaging,
                    //26  StartTime
                    StartTime = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                      weightSs.FirstOrDefault(x => x.ScaleId == scale.ScaleId && x.Status == "DANGCAN").StartTime : null,
                    //27  EndTime
                    EndTime = DateTime.Now,
                    //28  QuantityWeitght
                    QuantityWeitght = item.QuantityWeight,
                    //29  Weight
                    Weight = item.Weight,
                    //30  BagQuantity
                    BagQuantity = item.BagQuantity,
                    //31  SingleWeight
                    SingleWeight = item.SingleWeight,
                    //32  VehicleCode
                    VehicleCode = item.VehicleCode,
                    //33  InputWeight
                    InputWeight = item.InputWeight,
                    //34  OutputWeight
                    OutputWeight = item.OutputWeight,
                    //40  Description
                    Description = item.Description,
                    //41  Image
                    Image = !string.IsNullOrEmpty(imgPath) ? imgPath : null,
                    //42  Status
                    Status = "NOT",
                    //43  TruckInfoId
                    TruckInfoId = item.TruckInfoId,
                    //44  TruckNumber
                    TruckNumber = item.TruckQuantity,
                    //48  CreateTime
                    CreateTime = DateTime.Now,
                    //49  CreateBy
                    CreateBy = TokenExtensions.GetAccountId(),
                    //52  Actived
                    Actived = true

                });
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
