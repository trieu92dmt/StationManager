using Core.Exceptions;
using Core.Extensions;
using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Core.Utilities;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MES.Application.Commands.OutboundDelivery
{
    public class SaveGoodsReturnCommand : IRequest<bool>
    {
        public List<SaveGoodsReturn> SaveGoodsReturns { get; set; } = new List<SaveGoodsReturn>();
    }

    public class SaveGoodsReturn
    {
        //public Guid Id { get; set; }
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
        ////Id cân xe tải
        //public Guid? TruckInfoId { get; set; }
        ////Số xe tải
        //public string TruckQuantity { get; set; }
        ////Số cân đầu vào
        //public decimal? InputWeight { get; set; }
        ////Số cân đầu ra
        //public decimal? OutputWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        public List<string> ListImage { get; set; } = new List<string>();
        //Trạng thái
        public string Status { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Số batch
        public string Batch { get; set; }
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
        private readonly IRepository<ProductModel> _prodRepo;
        private readonly IRepository<WeighSessionChoseModel> _weightSsChoseRepo;

        public SaveGoodsReturnCommandHandler(IUnitOfWork unitOfWork, IRepository<GoodsReturnModel> nkhtRepo, IRepository<WeighSessionModel> weightSsRepo,
                                             IRepository<ScaleModel> scaleRepo, IRepository<DetailOutboundDeliveryModel> detailODRepo,
                                             IRepository<OutboundDeliveryModel> obRepo, IUtilitiesService utilitiesService, IRepository<ProductModel> prodRepo,
                                             IRepository<WeighSessionChoseModel> weightSsChoseRepo)
        {
            _unitOfWork = unitOfWork;
            _nkhtRepo = nkhtRepo;
            _weightSsRepo = weightSsRepo;
            _scaleRepo = scaleRepo;
            _detailODRepo = detailODRepo;
            _obRepo = obRepo;
            _utilitiesService = utilitiesService;
            _prodRepo = prodRepo;
            _weightSsChoseRepo = weightSsChoseRepo;
        }

        public async Task<bool> Handle(SaveGoodsReturnCommand request, CancellationToken cancellationToken)
        {
            //Get query đợt cân
            var weightSs = _weightSsRepo.GetQuery().AsNoTracking();

            //Get query cân
            var scales = _scaleRepo.GetQuery(x => x.ScaleType == true).AsNoTracking();

            //Products
            var prods = _prodRepo.GetQuery().AsNoTracking();

            //Danh sách nhập kho mua hàng
            var nkhts = await _nkhtRepo.GetQuery().ToListAsync();
            //Last index dùng để tạo số phiếu cân tự sinh
            var lastIndex = nkhts.Count() > 0 ? nkhts.OrderBy(x => x.WeightVote).LastOrDefault().WeightVote.Substring(1) : "1000000";

            //Query od
            var detailODs = _detailODRepo.GetQuery().Include(x => x.OutboundDelivery).AsNoTracking();
            var ods = _obRepo.GetQuery().AsNoTracking();

            var index = 1;
            foreach (var item in request.SaveGoodsReturns)
            {

                ////Lấy ra dòng dữ liệu đã lưu
                //var record = await _nkhtRepo.FindOneAsync(n => n.GoodsReturnId == item.Id);

                ////Lấy ra dòng dữ liệu mapping với đợt cân
                //var weightSsChose = await _weightSsChoseRepo.FindOneAsync(w => w.RecordId == item.Id);

                ////Check status
                //if (item.Status == "DAXOA")
                //{

                //    _weightSsChoseRepo.Remove(weightSsChose);

                //    _nkhtRepo.Remove(record);

                //    await _unitOfWork.SaveChangesAsync();

                //    continue;
                //}

                //Check điều kiện lưu
                #region Check điều kiện lưu

                if (!item.ConfirmQty.HasValue || item.ConfirmQty <= 0)
                {
                    throw new ISDException("Confirm Quantity phải lớn hơn 0");
                }

                if (string.IsNullOrEmpty(item.WeightHeadCode))
                {
                    if (!item.BagQuantity.HasValue || item.BagQuantity <= 0)
                    {
                        throw new ISDException("Số lượng bao phải lớn hơn 0");
                    }
                    if (!item.SingleWeight.HasValue || item.SingleWeight <= 0)
                    {
                        throw new ISDException("Đơn trọng phải lớn hơn 0");
                    }
                }
                #endregion

                var GoodsReturnId = Guid.NewGuid();

                var imgPath ="";
                for (int i = 1; i <= item.ListImage.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(item.ListImage[i]))
                    {
                        //Convert Base64 to Iformfile
                        byte[] bytes = Convert.FromBase64String(item.ListImage[i].Substring(item.ListImage[i].IndexOf(',') + 1));
                        MemoryStream stream = new MemoryStream(bytes);

                        IFormFile file = new FormFile(stream, 0, bytes.Length, $"{GoodsReturnId.ToString()}_{i}", $"{GoodsReturnId.ToString()}_{i}.jpg");
                        //Save image to server
                        imgPath = await _utilitiesService.UploadFile(file, "NKHT") + ",";
                    }
                }
                //if (!string.IsNullOrEmpty(item.Image))
                //{
                //    //Convert Base64 to Iformfile
                //    byte[] bytes = Convert.FromBase64String(item.Image.Substring(item.Image.IndexOf(',') + 1));
                //    MemoryStream stream = new MemoryStream(bytes);

                //    IFormFile file = new FormFile(stream, 0, bytes.Length, GoodsReturnId.ToString(), $"{GoodsReturnId.ToString()}.jpg");
                //    //Save image to server
                //    imgPath = await _utilitiesService.UploadFile(file, "NKHT");
                //}

                //Lấy ra cân hiện tại
                var scale = scales.FirstOrDefault(x => x.ScaleCode == item.WeightHeadCode);

                //Lấy ra đợt cân
                var weightSession = !string.IsNullOrEmpty(item.WeightHeadCode) && scale != null ?
                                 weightSs.Where(x => x.ScaleCode == scale.ScaleCode).OrderByDescending(x => x.OrderIndex).FirstOrDefault() : null;


                //Lấy ra outbound detail
                var detailOb = !string.IsNullOrEmpty(item.ODCode) ?
                               detailODs.FirstOrDefault(d => d.OutboundDelivery.DeliveryCodeInt == long.Parse(item.ODCode) && d.OutboundDeliveryItem == item.ODItem) : null;

                //Nếu có đợt cân thì lưu vào bảng mapping
                //if (weightSession != null)
                //{
                //    if (weightSsChose != null)
                //        _weightSsChoseRepo.Add(new WeighSessionChoseModel
                //        {
                //            Id = Guid.NewGuid(),
                //            DateKey = weightSession.DateKey,
                //            OrderIndex = weightSession.OrderIndex,
                //            ScaleCode = weightSession.ScaleCode,
                //            RecordId = item.Id
                //        });
                //}

                _nkhtRepo.Add(new GoodsReturnModel
                {
                    //1 GoodsReturnId
                    GoodsReturnId = GoodsReturnId,
                    //3 WeightHeadCode
                    WeightHeadCode = item.WeightHeadCode,
                    DateKey = weightSession != null ?
                               weightSession.DateKey : null,
                    OrderIndex = weightSession != null ?
                               weightSession.OrderIndex : null,
                    //4 WeightVote
                    WeightVote = $"N{long.Parse(lastIndex) + index}",
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
                    MaterialCode =  prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.MaterialCode)).ProductCode,
                    MaterialCodeInt = long.Parse(item.MaterialCode),
                    //14  ShipToParty
                    ShipToParty = !string.IsNullOrEmpty(item.ODCode) ?
                             detailOb.OutboundDelivery.ShiptoParty : null,
                    //15  ShipToPartyName
                    ShipToPartyName = !string.IsNullOrEmpty(item.ODCode) ?
                             detailOb.OutboundDelivery.ShiptoPartyName : null,
                    //16  SlocCode
                    SlocCode = item.SlocCode,
                    //20  ConfirmQty
                    ConfirmQty = item.ConfirmQty,
                    //21  UOM
                    UOM = prods.FirstOrDefault(x => x.ProductCodeInt == long.Parse(item.MaterialCode)).Unit,
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
                    StartTime = weightSession != null ?
                               weightSession.StartTime : null,
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
                    //Batch
                    Batch = item.Batch,
                    ////33  InputWeight
                    //InputWeight = item.InputWeight,
                    ////34  OutputWeight
                    //OutputWeight = item.OutputWeight,
                    //40  Description
                    Description = item.Description,
                    //41  Image
                    Image = string.IsNullOrEmpty(imgPath) ? null : imgPath,
                    //42  Status
                    Status = "NOT",
                    ////43  TruckInfoId
                    //TruckInfoId = item.TruckInfoId,
                    ////44  TruckNumber
                    //TruckNumber = item.TruckQuantity,
                    //48  CreateTime
                    CreateTime = DateTime.Now,
                    //49  CreateBy
                    CreateBy = TokenExtensions.GetAccountId(),
                    //52  Actived
                    Actived = true

                });

                index++;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
